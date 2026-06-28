using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StockExchange.Server.BackgroundJobs;
using StockExchange.Data.Context;
using StockExchange.Data.Repositories.Implementations;
using StockExchange.Data.Repositories.Interfaces;
using StockExchange.Data.Seed;
using StockExchange.Server.Services;
using StockExchange.Server.Handlers;
using StockExchange.Server.Network;
using StockExchange.Shared.DTOs;

namespace StockExchange.Server;

internal static class Program
{
    private static async Task<int> Main()
    {
        Console.Title = "Stock Exchange Server";
        Console.WriteLine("Starting Stock Exchange Server...");

        var services = new ServiceCollection();
        services.AddDbContext<StockExchangeDbContext>(options =>
            options.UseSqlite("Data Source=stock_exchange.db"));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<AuthService>();
        services.AddScoped<AuthMessageHandler>();
        services.AddScoped<PriceHistoryService>();
        services.AddScoped<ChartMessageHandler>();
        
        services.AddScoped<StockMessageHandler>(); // dùng để nhận diện StockMessageHandler trong MessageDispatcher để sử dụng MessageDispatcher
        services.AddScoped<MessageDispatcher>();
        services.AddScoped<StockService>();
        services.AddScoped<StockSimulationService>();
        services.AddScoped<StockBroadcastService>();
        services.AddScoped<WatchlistService>();
        services.AddSingleton<ClientManager>();
        services.AddSingleton<TcpServer>();
        services.AddSingleton<StockPriceUpdateJob>();

        await using var serviceProvider = services.BuildServiceProvider();

        try
        {
            await SeedDatabaseAsync(serviceProvider);
            await VerifyServicesAsync(serviceProvider);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Server startup failed: {ex}");
            return 1;
        }

        Console.WriteLine("Stock Exchange Server is running.");
        Console.WriteLine("Press Ctrl+C to stop.");

        using var shutdown = new CancellationTokenSource();
        Console.CancelKeyPress += (_, eventArgs) =>
        {
            eventArgs.Cancel = true;
            shutdown.Cancel();
        };

        try
        {
            var tcpServerTask = serviceProvider.GetRequiredService<TcpServer>().RunAsync(shutdown.Token);
            var priceUpdateTask = serviceProvider.GetRequiredService<StockPriceUpdateJob>().RunAsync(shutdown.Token);
            await Task.WhenAll(tcpServerTask, priceUpdateTask);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Stock Exchange Server stopped.");
        }

        return 0;
    }

    private static async Task SeedDatabaseAsync(IServiceProvider serviceProvider)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StockExchangeDbContext>();

        Console.WriteLine("Applying migrations and seeding data...");
        await DatabaseSeeder.SeedAllAsync(dbContext);
        Console.WriteLine("Database setup completed successfully.");
    }

    private static async Task VerifyServicesAsync(IServiceProvider serviceProvider)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StockExchangeDbContext>();

        var authService = scope.ServiceProvider.GetRequiredService<AuthService>();
        var login = await authService.ValidateLoginAsync(new LoginRequestDto
        {
            UsernameOrEmail = "admin",
            Password = "123456"
        });
        if (!login.Success)
        {
            throw new InvalidOperationException("AuthService rejected the seeded admin account.");
        }

        var stocks = await scope.ServiceProvider
            .GetRequiredService<StockService>()
            .GetAllStocksAsync();
        var stockCount = stocks.Count();
        if (stockCount == 0)
        {
            throw new InvalidOperationException("StockService returned no seeded stocks.");
        }

        var watchlist = await scope.ServiceProvider
            .GetRequiredService<WatchlistService>()
            .GetWatchlistAsync(login.UserId);
        var userCount = await dbContext.Users.CountAsync();
        var simulationCount = await dbContext.StockSimulations.CountAsync();

        Console.WriteLine($"[OK] AuthService: seeded admin login works.");
        Console.WriteLine($"[OK] StockService: {stockCount} stocks available.");
        Console.WriteLine($"[OK] WatchlistService: {watchlist.Count} stocks in admin watchlist.");
        Console.WriteLine($"[OK] Seeds: {userCount} users, {stockCount} stocks, {simulationCount} simulations.");
    }
}
