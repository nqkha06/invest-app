using StockExchange.Data.Repositories.Interfaces;
using StockExchange.Shared.DTOs;

namespace StockExchange.Server.Services;

public class PriceHistoryService
{
    private readonly IUnitOfWork _unitOfWork;

    public PriceHistoryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<PriceHistoryDto>?> GetAsync(
        PriceHistoryRequestDto request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var stock = await _unitOfWork.Stocks
            .GetBySymbolAsync(request.Symbol.Trim().ToUpperInvariant(), cancellationToken);

        if (stock is null || !stock.IsActive)
        {
            return null;
        }

        var range = ResolveRange(request);
        if (range.UseMinute)
        {
            var minutes = await FetchMinuteCandlesAsync(stock.Id, range.From, range.To, cancellationToken);
            return GroupCandles(minutes, range.Bucket);
        }

        var days = await FetchDayCandlesAsync(
            stock.Id,
            DateOnly.FromDateTime(range.From),
            DateOnly.FromDateTime(range.To),
            cancellationToken);
        return GroupCandles(days, range.Bucket);
    }

    private static (bool UseMinute, DateTime From, DateTime To, Func<DateTime, DateTime> Bucket) ResolveRange(
        PriceHistoryRequestDto request)
    {
        var now = DateTime.UtcNow;

        if (request.From.HasValue && request.To.HasValue)
        {
            var from = NormalizeUtc(request.From.Value);
            var to = NormalizeUtc(request.To.Value);
            var span = to - from;
            return (span.TotalDays <= 1, from, to, AlignToMinute);
        }

        return request.Interval?.Trim().ToUpperInvariant() switch
        {
            "1MIN" or "1P" => (true, now.AddHours(-4), now, AlignToMinute),
            "5MIN" or "5P" => (true, now.AddDays(-1), now, time => AlignToSpan(time, TimeSpan.FromMinutes(5))),
            "15MIN" or "15P" => (true, now.AddDays(-3), now, time => AlignToSpan(time, TimeSpan.FromMinutes(15))),
            "30MIN" or "30P" => (true, now.AddDays(-7), now, time => AlignToSpan(time, TimeSpan.FromMinutes(30))),
            "1H" => (true, now.AddDays(-14), now, time => AlignToSpan(time, TimeSpan.FromHours(1))),
            "1D" => (false, now.AddDays(-180).Date, now, time => time.Date),
            "1W" => (false, now.AddYears(-2).Date, now, AlignToWeek),
            "1M" => (false, now.AddYears(-5).Date, now, AlignToMonth),
            _ => (true, now.AddHours(-4), now, AlignToMinute)
        };
    }

    private async Task<IReadOnlyList<PriceHistoryDto>> FetchMinuteCandlesAsync(
        long stockId,
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken)
    {
        var rows = await _unitOfWork.StockPriceMinutes
            .GetHistoryAsync(stockId, from, to, cancellationToken);

        return rows.Select(row => new PriceHistoryDto
        {
            Timestamp = row.RecordedAt,
            Open = row.OpenPrice,
            High = row.HighPrice,
            Low = row.LowPrice,
            Close = row.ClosePrice,
            Volume = row.Volume
        }).ToList();
    }

    private async Task<IReadOnlyList<PriceHistoryDto>> FetchDayCandlesAsync(
        long stockId,
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken)
    {
        var rows = await _unitOfWork.StockPriceDays
            .GetHistoryAsync(stockId, from, to, cancellationToken);

        return rows.Select(row => new PriceHistoryDto
        {
            Timestamp = row.TradingDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc),
            Open = row.OpenPrice,
            High = row.HighPrice,
            Low = row.LowPrice,
            Close = row.ClosePrice,
            Volume = row.Volume
        }).ToList();
    }

    private static IReadOnlyList<PriceHistoryDto> GroupCandles(
        IReadOnlyList<PriceHistoryDto> candles,
        Func<DateTime, DateTime> bucket)
    {
        return candles
            .GroupBy(candle => bucket(NormalizeUtc(candle.Timestamp)))
            .OrderBy(group => group.Key)
            .Select(group =>
            {
                var ordered = group.OrderBy(candle => candle.Timestamp).ToList();
                return new PriceHistoryDto
                {
                    Timestamp = group.Key,
                    Open = ordered[0].Open,
                    High = ordered.Max(candle => candle.High),
                    Low = ordered.Min(candle => candle.Low),
                    Close = ordered[^1].Close,
                    Volume = ordered.Sum(candle => candle.Volume)
                };
            })
            .ToList();
    }

    private static DateTime NormalizeUtc(DateTime time) =>
        time.Kind == DateTimeKind.Utc ? time : DateTime.SpecifyKind(time, DateTimeKind.Utc);

    private static DateTime AlignToMinute(DateTime time) =>
        AlignToSpan(time, TimeSpan.FromMinutes(1));

    private static DateTime AlignToSpan(DateTime time, TimeSpan span) =>
        new(time.Ticks - time.Ticks % span.Ticks, DateTimeKind.Utc);

    private static DateTime AlignToWeek(DateTime time)
    {
        var date = time.Date;
        var offset = ((int)date.DayOfWeek + 6) % 7;
        return DateTime.SpecifyKind(date.AddDays(-offset), DateTimeKind.Utc);
    }

    private static DateTime AlignToMonth(DateTime time) =>
        new(time.Year, time.Month, 1, 0, 0, 0, DateTimeKind.Utc);
}
