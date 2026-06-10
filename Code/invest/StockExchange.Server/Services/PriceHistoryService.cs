using StockExchange.Data.Repositories.Interfaces;
using StockExchange.Shared.DTOs;

namespace StockExchange.Server.Services;

/// <summary>
/// Fetches OHLCV candle data for chart rendering.
///
/// Interval logic:
///   "1D"  → minute candles from session open (09:00 UTC) to now
///   "1W"  → daily candles for the past 7 days
///   "1M"  → daily candles for the past 30 days
///   "3M"  → daily candles for the past 90 days
///   "1Y"  → daily candles for the past 365 days
///   Custom From/To → minute candles if span ≤ 1 day, else daily
/// </summary>
public class PriceHistoryService
{
    private readonly IUnitOfWork _unitOfWork;

    public PriceHistoryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    // -------------------------------------------------------------------------
    //  Public API
    // -------------------------------------------------------------------------

    /// <summary>
    /// Returns candles for the requested symbol and interval.
    /// Returns <c>null</c> when the symbol does not exist or is inactive.
    /// </summary>
    public async Task<IReadOnlyList<PriceHistoryDto>?> GetAsync(
        PriceHistoryRequestDto request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        // 1. Resolve the stock by symbol
        var stock = await _unitOfWork.Stocks
            .GetBySymbolAsync(request.Symbol.Trim().ToUpperInvariant(), cancellationToken);

        if (stock is null || !stock.IsActive)
            return null;

        // 2. Determine time range and granularity
        var (useMinute, from, to) = ResolveRange(request);

        // 3. Query the correct repository and project to DTOs
        if (useMinute)
            return await FetchMinuteCandlesAsync(stock.Id, from, to, cancellationToken);

        return await FetchDayCandlesAsync(
            stock.Id,
            DateOnly.FromDateTime(from),
            DateOnly.FromDateTime(to),
            cancellationToken);
    }

    // -------------------------------------------------------------------------
    //  Range resolution
    // -------------------------------------------------------------------------

    private static (bool useMinute, DateTime from, DateTime to) ResolveRange(
        PriceHistoryRequestDto request)
    {
        var now = DateTime.UtcNow;

        // Explicit range supplied by the caller
        if (request.From.HasValue && request.To.HasValue)
        {
            var span = request.To.Value - request.From.Value;
            return (span.TotalDays <= 1, request.From.Value, request.To.Value);
        }

        // Standard intervals
        return request.Interval?.ToUpperInvariant() switch
        {
            "1D" => (true,  now.Date.AddHours(9),   now),
            "1W" => (false, now.AddDays(-7).Date,   now),
            "1M" => (false, now.AddDays(-30).Date,  now),
            "3M" => (false, now.AddDays(-90).Date,  now),
            "1Y" => (false, now.AddDays(-365).Date, now),
            _    => (true,  now.Date.AddHours(9),   now),   // default → 1D
        };
    }

    // -------------------------------------------------------------------------
    //  Data fetching & mapping
    // -------------------------------------------------------------------------

    private async Task<IReadOnlyList<PriceHistoryDto>> FetchMinuteCandlesAsync(
        long stockId, DateTime from, DateTime to, CancellationToken ct)
    {
        var rows = await _unitOfWork.StockPriceMinutes
            .GetHistoryAsync(stockId, from, to, ct);

        return rows.Select(r => new PriceHistoryDto
        {
            Timestamp = r.RecordedAt,
            Open      = r.OpenPrice,
            High      = r.HighPrice,
            Low       = r.LowPrice,
            Close     = r.ClosePrice,
            Volume    = r.Volume,
        }).ToList();
    }

    private async Task<IReadOnlyList<PriceHistoryDto>> FetchDayCandlesAsync(
        long stockId, DateOnly from, DateOnly to, CancellationToken ct)
    {
        var rows = await _unitOfWork.StockPriceDays
            .GetHistoryAsync(stockId, from, to, ct);

        return rows.Select(r => new PriceHistoryDto
        {
            Timestamp = r.TradingDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc),
            Open      = r.OpenPrice,
            High      = r.HighPrice,
            Low       = r.LowPrice,
            Close     = r.ClosePrice,
            Volume    = r.Volume,
        }).ToList();
    }
}