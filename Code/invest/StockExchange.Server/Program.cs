using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StockExchange.Data.Context;
using StockExchange.Data.Seed;

namespace StockExchange.Server;

internal static class Program
{
    private static async Task Main()
    {
        Console.Title = "Stock Exchange Server";
        Console.WriteLine("Starting Stock Exchange Server...");

        // 1. Thiết lập Dependency Injection và DbContext
        var services = new ServiceCollection();
        
        // Cấu hình dùng SQLite
        services.AddDbContext<StockExchangeDbContext>(options =>
            options.UseSqlite("Data Source=stock_exchange.db"));

        var serviceProvider = services.BuildServiceProvider();

        // 2. Chạy Seeder để tự động tạo Database và nạp dữ liệu mẫu
        using (var scope = serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<StockExchangeDbContext>();
            try
            {
                Console.WriteLine("Applying migrations and seeding data...");
                // Hàm này sẽ tạo file stock_exchange.db nếu chưa có
                await DatabaseSeeder.SeedAllAsync(dbContext);
                
                Console.WriteLine("Database setup completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while seeding the database: {ex.Message}");
            }
        }

        Console.WriteLine("Stock Exchange Server is running.");
        Console.WriteLine("Press Enter to stop...");
        Console.ReadLine();
    }
}