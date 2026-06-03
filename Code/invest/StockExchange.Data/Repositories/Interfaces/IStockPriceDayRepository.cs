using StockExchange.Shared.Models;

namespace StockExchange.Data.Repositories.Interfaces;

public interface IStockPriceDayRepository
{
    Task<StockPriceDay?> GetByDateAsync(long stockId, DateOnly tradingDate, CancellationToken cancellationToken = default);
    Task<StockPriceDay?> GetLatestAsync(long stockId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StockPriceDay>> GetHistoryAsync(long stockId, DateOnly from, DateOnly to, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StockPriceDay>> GetRecentAsync(long stockId, int count, CancellationToken cancellationToken = default);
    Task AddAsync(StockPriceDay price, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<StockPriceDay> prices, CancellationToken cancellationToken = default);
    void Update(StockPriceDay price);
    void Remove(StockPriceDay price);
}
