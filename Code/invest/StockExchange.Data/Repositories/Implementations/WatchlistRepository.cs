using Microsoft.EntityFrameworkCore;
using StockExchange.Data.Context;
using StockExchange.Data.Repositories.Interfaces;
using StockExchange.Shared.Models;

namespace StockExchange.Data.Repositories.Implementations;

public class WatchlistRepository : IWatchlistRepository
{
    private readonly StockExchangeDbContext _context;

    public WatchlistRepository(StockExchangeDbContext context)
    {
        _context = context;
    }

    public Task<UserWatchlist?> GetAsync(long userId, long stockId, CancellationToken cancellationToken = default)
    {
        return _context.UserWatchlists
            .Include(watchlist => watchlist.Stock)
            .FirstOrDefaultAsync(
                watchlist => watchlist.UserId == userId && watchlist.StockId == stockId,
                cancellationToken);
    }

    public async Task<IReadOnlyList<UserWatchlist>> GetByUserIdAsync(long userId, CancellationToken cancellationToken = default)
    {
        return await _context.UserWatchlists
            .AsNoTracking()
            .Include(watchlist => watchlist.Stock)
            .Where(watchlist => watchlist.UserId == userId)
            .OrderBy(watchlist => watchlist.Stock!.Symbol)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Stock>> GetStocksByUserIdAsync(long userId, CancellationToken cancellationToken = default)
    {
        return await _context.UserWatchlists
            .AsNoTracking()
            .Include(watchlist => watchlist.Stock)
            .Where(watchlist => watchlist.UserId == userId && watchlist.Stock != null)
            .Select(watchlist => watchlist.Stock!)
            .OrderBy(stock => stock.Symbol)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> ExistsAsync(long userId, long stockId, CancellationToken cancellationToken = default)
    {
        return _context.UserWatchlists.AnyAsync(
            watchlist => watchlist.UserId == userId && watchlist.StockId == stockId,
            cancellationToken);
    }

    public Task AddAsync(UserWatchlist watchlist, CancellationToken cancellationToken = default)
    {
        return _context.UserWatchlists.AddAsync(watchlist, cancellationToken).AsTask();
    }

    public async Task<bool> RemoveAsync(long userId, long stockId, CancellationToken cancellationToken = default)
    {
        var watchlist = await _context.UserWatchlists.FirstOrDefaultAsync(
            item => item.UserId == userId && item.StockId == stockId,
            cancellationToken);

        if (watchlist is null)
        {
            return false;
        }

        _context.UserWatchlists.Remove(watchlist);
        return true;
    }

    public void Remove(UserWatchlist watchlist)
    {
        _context.UserWatchlists.Remove(watchlist);
    }
}
