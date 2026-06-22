using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StockExchange.Data.Context;
using StockExchange.Shared.DTOs;

namespace StockExchange.Server.BackgroundJobs;

public sealed class StockPriceUpdateJob
{
    private static readonly TimeSpan TickInterval = TimeSpan.FromSeconds(1);
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly Random _random = new();
    private readonly Dictionary<long, DateTime> _lastUpdatedByStock = [];

    public StockPriceUpdateJob(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Stock price update job started.");

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await UpdatePricesAsync(cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Stock price update job failed: {ex.Message}");
            }

            await Task.Delay(TickInterval, cancellationToken);
        }
    }

    private async Task UpdatePricesAsync(CancellationToken cancellationToken)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StockExchangeDbContext>();
        var broadcaster = scope.ServiceProvider.GetRequiredService<Services.StockBroadcastService>();
        var now = DateTime.UtcNow;

        var simulations = await dbContext.StockSimulations
            .Include(simulation => simulation.Stock)
            .Where(simulation => simulation.Stock != null && simulation.Stock.IsActive)
            .ToListAsync(cancellationToken);

        var updates = new List<StockPriceUpdateDto>();
        foreach (var simulation in simulations)
        {
            var stock = simulation.Stock!;
            var updateSpeed = Math.Max(1, (double)simulation.UpdateSpeed);
            if (_lastUpdatedByStock.TryGetValue(stock.Id, out var lastUpdated)
                && now - lastUpdated < TimeSpan.FromSeconds(updateSpeed))
            {
                continue;
            }

            var previousPrice = stock.CurrentPrice;
            var nextPrice = CalculateNextPrice(previousPrice, simulation.AlgorithmType, simulation.Volatility,
                simulation.TrendFactor, simulation.JumpProbability);
            nextPrice = Math.Clamp(nextPrice, simulation.MinPrice, simulation.MaxPrice);
            nextPrice = Math.Round(nextPrice, 2, MidpointRounding.AwayFromZero);

            if (nextPrice == previousPrice)
            {
                nextPrice = Math.Min(simulation.MaxPrice, previousPrice + 0.01m);
            }

            stock.CurrentPrice = nextPrice;
            simulation.UpdatedAt = now;
            _lastUpdatedByStock[stock.Id] = now;

            updates.Add(new StockPriceUpdateDto
            {
                StockId = stock.Id,
                Symbol = stock.Symbol,
                Price = nextPrice,
                PreviousPrice = previousPrice,
                ChangePercent = previousPrice == 0 ? 0 : (nextPrice - previousPrice) / previousPrice * 100m,
                UpdatedAt = now
            });
        }

        if (updates.Count == 0)
        {
            return;
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        foreach (var update in updates)
        {
            await broadcaster.BroadcastPriceUpdateAsync(update, cancellationToken);
        }
    }

    private decimal CalculateNextPrice(
        decimal currentPrice,
        string algorithmType,
        decimal volatility,
        decimal trendFactor,
        decimal jumpProbability)
    {
        var randomFactor = (decimal)(_random.NextDouble() * 2 - 1);
        var movement = currentPrice * volatility * randomFactor;

        if (algorithmType.Equals("MeanReversion", StringComparison.OrdinalIgnoreCase))
        {
            movement -= currentPrice * trendFactor * Math.Sign(randomFactor);
        }
        else if (algorithmType.Equals("TrendFollowing", StringComparison.OrdinalIgnoreCase))
        {
            movement += currentPrice * trendFactor;
        }

        if ((decimal)_random.NextDouble() < jumpProbability)
        {
            movement += currentPrice * volatility * 2m * Math.Sign(randomFactor == 0 ? 1 : randomFactor);
        }

        return Math.Max(0.01m, currentPrice + movement);
    }
}
