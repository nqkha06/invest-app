using Microsoft.EntityFrameworkCore;
using StockExchange.Data.Context;
using StockExchange.Shared.Models;

namespace StockExchange.Data.Seed;

public static class StockSeeder
{
    public static async Task SeedAsync(StockExchangeDbContext context)
    {
        if (await context.Stocks.AnyAsync())
            return;

        var stocks = new List<Stock>
        {
            new Stock { Symbol = "FPT", CompanyName = "CTCP FPT", Sector = "Technology", CurrentPrice = 115.5m, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Stock { Symbol = "VCB", CompanyName = "Ngân hàng TMCP Ngoại thương VN", Sector = "Banking", CurrentPrice = 90.2m, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Stock { Symbol = "VNM", CompanyName = "CTCP Sữa Việt Nam", Sector = "Consumer", CurrentPrice = 65.8m, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Stock { Symbol = "VIC", CompanyName = "Tập đoàn Vingroup", Sector = "Real Estate", CurrentPrice = 45.0m, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Stock { Symbol = "MWG", CompanyName = "CTCP Đầu tư Thế giới Di động", Sector = "Retail", CurrentPrice = 48.5m, IsActive = true, CreatedAt = DateTime.UtcNow }
        };

        await context.Stocks.AddRangeAsync(stocks);
    }
}
