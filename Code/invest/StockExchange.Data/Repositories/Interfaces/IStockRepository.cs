using StockExchange.Shared.Models;

namespace StockExchange.Data.Repositories.Interfaces;

public interface IStockRepository
{
    Task<Stock?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<Stock?> GetBySymbolAsync(string symbol, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Stock>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Stock>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Stock>> SearchAsync(string keyword, int limit = 50, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Stock>> GetBySectorAsync(string sector, CancellationToken cancellationToken = default);
    Task AddAsync(Stock stock, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<Stock> stocks, CancellationToken cancellationToken = default);
    void Update(Stock stock);
    void Remove(Stock stock);
}
