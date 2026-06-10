using System.ComponentModel;

namespace StockExchange.Client.WinForms.Mock;

public sealed class StockRow
{
    public long Id { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string Sector { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal ChangePercent { get; set; }
    public long Volume { get; set; }
    public bool Active { get; set; }
}

public sealed class UserRow
{
    public long Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public sealed class SimulationRow
{
    public long StockId { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public string Algorithm { get; set; } = "RandomWalk";
    public decimal Volatility { get; set; }
    public decimal TrendFactor { get; set; }
    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }
    public decimal UpdateSpeed { get; set; }
    public decimal JumpProbability { get; set; }
}

public sealed class CandlePoint
{
    public DateTime Time { get; init; }
    public decimal Open { get; init; }
    public decimal High { get; init; }
    public decimal Low { get; init; }
    public decimal Close { get; init; }
    public long Volume { get; init; }
}

public static class MockData
{
    public static BindingList<StockRow> Stocks { get; } = new(
    [
        new() { Id = 1, Symbol = "FPT", Company = "CTCP FPT", Sector = "Technology", Price = 115.50m, ChangePercent = 2.34m, Volume = 2_840_000, Active = true },
        new() { Id = 2, Symbol = "VCB", Company = "Vietcombank", Sector = "Banking", Price = 90.20m, ChangePercent = -0.55m, Volume = 1_920_000, Active = true },
        new() { Id = 3, Symbol = "VNM", Company = "Vinamilk", Sector = "Consumer", Price = 65.80m, ChangePercent = 0.92m, Volume = 1_120_000, Active = true },
        new() { Id = 4, Symbol = "VIC", Company = "Vingroup", Sector = "Real Estate", Price = 45.00m, ChangePercent = -1.20m, Volume = 3_240_000, Active = true },
        new() { Id = 5, Symbol = "MWG", Company = "Thế Giới Di Động", Sector = "Retail", Price = 48.50m, ChangePercent = 1.76m, Volume = 2_080_000, Active = true }
    ]);

    public static BindingList<UserRow> Users { get; } = new(
    [
        new() { Id = 1, Username = "admin", Email = "admin@investapp.local", Role = "Admin", Status = "Active", CreatedAt = DateTime.Today.AddMonths(-8) },
        new() { Id = 2, Username = "demo_user", Email = "user@investapp.local", Role = "Member", Status = "Active", CreatedAt = DateTime.Today.AddMonths(-5) },
        new() { Id = 3, Username = "quoc_kha", Email = "kha@example.com", Role = "Member", Status = "Active", CreatedAt = DateTime.Today.AddDays(-35) },
        new() { Id = 4, Username = "investor01", Email = "investor01@example.com", Role = "Member", Status = "Locked", CreatedAt = DateTime.Today.AddDays(-12) }
    ]);

    public static BindingList<SimulationRow> Simulations { get; } = new(
        Stocks.Select(stock => new SimulationRow
        {
            StockId = stock.Id,
            Symbol = stock.Symbol,
            Algorithm = "RandomWalk",
            Volatility = 0.02m,
            TrendFactor = 0.001m,
            MinPrice = stock.Price * 0.5m,
            MaxPrice = stock.Price * 2m,
            UpdateSpeed = 1m,
            JumpProbability = 0.05m
        }).ToList());

    public static BindingList<StockRow> Watchlist { get; } = new(
        Stocks.Where(stock => stock.Symbol is "FPT" or "VCB" or "MWG").ToList());

    public static IReadOnlyList<CandlePoint> BuildCandles(StockRow stock, int count, TimeSpan interval)
    {
        var seed = StringComparer.Ordinal.GetHashCode(stock.Symbol) ^ count ^ interval.GetHashCode();
        var random = new Random(seed);
        var candles = new List<CandlePoint>(count);
        var close = stock.Price * 0.91m;
        var start = DateTime.Today.AddTicks(-interval.Ticks * (count - 1L));

        for (var index = 0; index < count; index++)
        {
            var open = close;
            var remaining = count - index;
            var correction = (stock.Price - open) / remaining;
            var noise = remaining == 1
                ? 0m
                : (decimal)(random.NextDouble() - 0.5) * stock.Price * 0.018m;
            var drift = correction + noise;
            close = Math.Max(1m, open + drift);
            var wick = (decimal)(0.004 + random.NextDouble() * 0.015) * stock.Price;
            var high = Math.Max(open, close) + wick;
            var low = Math.Max(0.01m, Math.Min(open, close) - wick * (decimal)(0.7 + random.NextDouble() * 0.6));
            var volumeFactor = 0.45 + random.NextDouble() * 1.1;

            candles.Add(new CandlePoint
            {
                Time = start.AddTicks(interval.Ticks * index),
                Open = decimal.Round(open, 2),
                High = decimal.Round(high, 2),
                Low = decimal.Round(low, 2),
                Close = decimal.Round(close, 2),
                Volume = Math.Max(1, (long)(stock.Volume / Math.Max(1, count / 5d) * volumeFactor))
            });
        }

        var last = candles[^1];
        candles[^1] = new CandlePoint
        {
            Time = last.Time,
            Open = last.Open,
            High = Math.Max(last.High, stock.Price),
            Low = Math.Min(last.Low, stock.Price),
            Close = stock.Price,
            Volume = last.Volume
        };
        return candles;
    }
}
