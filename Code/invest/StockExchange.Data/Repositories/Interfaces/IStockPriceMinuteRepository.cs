using StockExchange.Shared.Models;

namespace StockExchange.Data.Repositories.Interfaces;

public interface IStockPriceMinuteRepository
{
    Task<StockPriceMinute?> GetLatestAsync(long stockId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StockPriceMinute>> GetHistoryAsync(long stockId, DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StockPriceMinute>> GetRecentAsync(long stockId, int count, CancellationToken cancellationToken = default);
    Task AddAsync(StockPriceMinute price, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<StockPriceMinute> prices, CancellationToken cancellationToken = default);
    void Remove(StockPriceMinute price);
}
