using Microsoft.EntityFrameworkCore;
using StockExchange.Data.Context;
using StockExchange.Shared.Models;

namespace StockExchange.Data.Seed;

public static class StockSimulationSeeder
{
    public static async Task SeedAsync(StockExchangeDbContext context)
    {
        if (await context.StockSimulations.AnyAsync())
            return;

        var stocks = await context.Stocks.ToListAsync();
        var simulations = new List<StockSimulation>();

        foreach (var stock in stocks)
        {
            simulations.Add(new StockSimulation
            {
                StockId = stock.Id,
                AlgorithmType = "RandomWalk",
                Volatility = 0.02m,
                TrendFactor = 0.001m,
                MinPrice = stock.CurrentPrice * 0.5m,
                MaxPrice = stock.CurrentPrice * 2.0m,
                UpdateSpeed = 1.0m,
                JumpProbability = 0.05m,
                UpdatedAt = DateTime.UtcNow
            });
        }

        await context.StockSimulations.AddRangeAsync(simulations);
    }
}
