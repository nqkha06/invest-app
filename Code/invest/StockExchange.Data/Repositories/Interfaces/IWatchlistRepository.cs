using StockExchange.Shared.Models;

namespace StockExchange.Data.Repositories.Interfaces;

public interface IWatchlistRepository
{
    Task<UserWatchlist?> GetAsync(long userId, long stockId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserWatchlist>> GetByUserIdAsync(long userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Stock>> GetStocksByUserIdAsync(long userId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(long userId, long stockId, CancellationToken cancellationToken = default);
    Task AddAsync(UserWatchlist watchlist, CancellationToken cancellationToken = default);
    Task<bool> RemoveAsync(long userId, long stockId, CancellationToken cancellationToken = default);
    void Remove(UserWatchlist watchlist);
}
