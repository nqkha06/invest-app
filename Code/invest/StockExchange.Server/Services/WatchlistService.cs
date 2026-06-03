using StockExchange.Data.Repositories.Interfaces;
using StockExchange.Shared.Models;

namespace StockExchange.Server.Services;

public class WatchlistService
{
    private readonly IUnitOfWork _unitOfWork;

    public WatchlistService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<bool> AddAsync(long userId, long stockId, CancellationToken cancellationToken = default)
    {
        return AddToWatchlistAsync(userId, stockId, cancellationToken);
    }

    public async Task<bool> AddToWatchlistAsync(long userId, long stockId, CancellationToken cancellationToken = default)
    {
        if (userId <= 0 || stockId <= 0)
        {
            return false;
        }

        var stock = await _unitOfWork.Stocks.GetByIdAsync(stockId, cancellationToken);
        if (stock is null || !stock.IsActive)
        {
            return false;
        }

        var exists = await _unitOfWork.Watchlists.ExistsAsync(userId, stockId, cancellationToken);
        if (exists)
        {
            return false;
        }

        var watchlist = new UserWatchlist
        {
            UserId = userId,
            StockId = stockId,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Watchlists.AddAsync(watchlist, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public Task<bool> RemoveAsync(long userId, long stockId, CancellationToken cancellationToken = default)
    {
        return RemoveFromWatchlistAsync(userId, stockId, cancellationToken);
    }

    public async Task<bool> RemoveFromWatchlistAsync(long userId, long stockId, CancellationToken cancellationToken = default)
    {
        if (userId <= 0 || stockId <= 0)
        {
            return false;
        }

        var removed = await _unitOfWork.Watchlists.RemoveAsync(userId, stockId, cancellationToken);
        if (!removed)
        {
            return false;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public Task<IReadOnlyList<Stock>> GetWatchlistAsync(long userId, CancellationToken cancellationToken = default)
    {
        return GetWatchlistStocksAsync(userId, cancellationToken);
    }

    public Task<IReadOnlyList<Stock>> GetWatchlistStocksAsync(long userId, CancellationToken cancellationToken = default)
    {
        if (userId <= 0)
        {
            return Task.FromResult<IReadOnlyList<Stock>>(Array.Empty<Stock>());
        }

        return _unitOfWork.Watchlists.GetStocksByUserIdAsync(userId, cancellationToken);
    }
}
