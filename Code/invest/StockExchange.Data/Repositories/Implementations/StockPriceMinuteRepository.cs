using Microsoft.EntityFrameworkCore;
using StockExchange.Data.Context;
using StockExchange.Data.Repositories.Interfaces;
using StockExchange.Shared.Models;

namespace StockExchange.Data.Repositories.Implementations;

public class StockPriceMinuteRepository : IStockPriceMinuteRepository
{
    private readonly StockExchangeDbContext _context;

    public StockPriceMinuteRepository(StockExchangeDbContext context)
    {
        _context = context;
    }

    public Task<StockPriceMinute?> GetLatestAsync(long stockId, CancellationToken cancellationToken = default)
    {
        return _context.StockPricesMinute
            .AsNoTracking()
            .Where(price => price.StockId == stockId)
            .OrderByDescending(price => price.RecordedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<StockPriceMinute>> GetHistoryAsync(
        long stockId,
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default)
    {
        return await _context.StockPricesMinute
            .AsNoTracking()
            .Where(price => price.StockId == stockId && price.RecordedAt >= from && price.RecordedAt <= to)
            .OrderBy(price => price.RecordedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<StockPriceMinute>> GetRecentAsync(long stockId, int count, CancellationToken cancellationToken = default)
    {
        return await _context.StockPricesMinute
            .AsNoTracking()
            .Where(price => price.StockId == stockId)
            .OrderByDescending(price => price.RecordedAt)
            .Take(Math.Max(1, count))
            .OrderBy(price => price.RecordedAt)
            .ToListAsync(cancellationToken);
    }

    public Task AddAsync(StockPriceMinute price, CancellationToken cancellationToken = default)
    {
        return _context.StockPricesMinute.AddAsync(price, cancellationToken).AsTask();
    }

    public Task AddRangeAsync(IEnumerable<StockPriceMinute> prices, CancellationToken cancellationToken = default)
    {
        return _context.StockPricesMinute.AddRangeAsync(prices, cancellationToken);
    }

    public void Remove(StockPriceMinute price)
    {
        _context.StockPricesMinute.Remove(price);
    }
}
