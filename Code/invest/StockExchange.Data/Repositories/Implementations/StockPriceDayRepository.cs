using Microsoft.EntityFrameworkCore;
using StockExchange.Data.Context;
using StockExchange.Data.Repositories.Interfaces;
using StockExchange.Shared.Models;

namespace StockExchange.Data.Repositories.Implementations;

public class StockPriceDayRepository : IStockPriceDayRepository
{
    private readonly StockExchangeDbContext _context;

    public StockPriceDayRepository(StockExchangeDbContext context)
    {
        _context = context;
    }

    public Task<StockPriceDay?> GetByDateAsync(long stockId, DateOnly tradingDate, CancellationToken cancellationToken = default)
    {
        return _context.StockPricesDay
            .AsNoTracking()
            .FirstOrDefaultAsync(
                price => price.StockId == stockId && price.TradingDate == tradingDate,
                cancellationToken);
    }

    public Task<StockPriceDay?> GetLatestAsync(long stockId, CancellationToken cancellationToken = default)
    {
        return _context.StockPricesDay
            .AsNoTracking()
            .Where(price => price.StockId == stockId)
            .OrderByDescending(price => price.TradingDate)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<StockPriceDay>> GetHistoryAsync(
        long stockId,
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken = default)
    {
        return await _context.StockPricesDay
            .AsNoTracking()
            .Where(price => price.StockId == stockId && price.TradingDate >= from && price.TradingDate <= to)
            .OrderBy(price => price.TradingDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<StockPriceDay>> GetRecentAsync(long stockId, int count, CancellationToken cancellationToken = default)
    {
        return await _context.StockPricesDay
            .AsNoTracking()
            .Where(price => price.StockId == stockId)
            .OrderByDescending(price => price.TradingDate)
            .Take(Math.Max(1, count))
            .OrderBy(price => price.TradingDate)
            .ToListAsync(cancellationToken);
    }

    public Task AddAsync(StockPriceDay price, CancellationToken cancellationToken = default)
    {
        return _context.StockPricesDay.AddAsync(price, cancellationToken).AsTask();
    }

    public Task AddRangeAsync(IEnumerable<StockPriceDay> prices, CancellationToken cancellationToken = default)
    {
        return _context.StockPricesDay.AddRangeAsync(prices, cancellationToken);
    }

    public void Update(StockPriceDay price)
    {
        _context.StockPricesDay.Update(price);
    }

    public void Remove(StockPriceDay price)
    {
        _context.StockPricesDay.Remove(price);
    }
}
