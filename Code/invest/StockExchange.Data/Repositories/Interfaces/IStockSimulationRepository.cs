using StockExchange.Shared.Models;

namespace StockExchange.Data.Repositories.Interfaces;

public interface IStockSimulationRepository
{
    Task<StockSimulation?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<StockSimulation?> GetByStockIdAsync(long stockId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StockSimulation>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(StockSimulation simulation, CancellationToken cancellationToken = default);
    void Update(StockSimulation simulation);
    void Remove(StockSimulation simulation);
}
