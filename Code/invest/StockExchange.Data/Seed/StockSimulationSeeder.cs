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

        // Lấy danh sách cổ phiếu vừa tạo
        var stocks = await context.Stocks.ToListAsync();
        var simulations = new List<StockSimulation>();

        foreach (var stock in stocks)
        {
            simulations.Add(new StockSimulation
            {
                StockId = stock.Id,
                AlgorithmType = "RandomWalk",
                Volatility = 0.02m, // Độ biến động 2%
                TrendFactor = 0.001m, // Xu hướng tăng nhẹ
                MinPrice = stock.CurrentPrice * 0.5m, // Rớt tối đa 50%
                MaxPrice = stock.CurrentPrice * 2.0m, // Tăng tối đa 200%
                UpdateSpeed = 1.0m, // Cập nhật mỗi giây
                JumpProbability = 0.05m, // 5% cơ hội có bước giá đột biến
                UpdatedAt = DateTime.UtcNow
            });
        }

        await context.StockSimulations.AddRangeAsync(simulations);
    }
}