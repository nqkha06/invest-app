using StockExchange.Server.Services;
using StockExchange.Shared.Models;

namespace StockExchange.Server.Handlers;

public class WatchlistMessageHandler
{
    private readonly WatchlistService _watchlistService;

    public WatchlistMessageHandler(WatchlistService watchlistService)
    {
        _watchlistService = watchlistService;
    }

    public Task<IReadOnlyList<Stock>> HandleGetWatchlistAsync(
        long userId,
        CancellationToken cancellationToken = default)
    {
        return _watchlistService.GetWatchlistAsync(userId, cancellationToken);
    }

    public Task<bool> HandleAddToWatchlistAsync(
        long userId,
        long stockId,
        CancellationToken cancellationToken = default)
    {
        return _watchlistService.AddAsync(userId, stockId, cancellationToken);
    }

    public Task<bool> HandleRemoveFromWatchlistAsync(
        long userId,
        long stockId,
        CancellationToken cancellationToken = default)
    {
        return _watchlistService.RemoveAsync(userId, stockId, cancellationToken);
    }
}