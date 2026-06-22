using Microsoft.EntityFrameworkCore;
using StockExchange.Data.Context;
using StockExchange.Data.Repositories.Interfaces;
using StockExchange.Shared.Models;

namespace StockExchange.Data.Repositories.Implementations;

public class StockSimulationRepository : IStockSimulationRepository
{
    private readonly StockExchangeDbContext _context;

    public StockSimulationRepository(StockExchangeDbContext context)
    {
        _context = context;
    }

    public Task<StockSimulation?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return _context.StockSimulations
            .FirstOrDefaultAsync(simulation => simulation.Id == id, cancellationToken);
    }

    public Task<StockSimulation?> GetByStockIdAsync(long stockId, CancellationToken cancellationToken = default)
    {
        return _context.StockSimulations
            .AsNoTracking()
            .Include(simulation => simulation.Stock)
            .FirstOrDefaultAsync(simulation => simulation.StockId == stockId, cancellationToken);
    }

    public async Task<IReadOnlyList<StockSimulation>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.StockSimulations
            .AsNoTracking()
            .Include(simulation => simulation.Stock)
            .OrderBy(simulation => simulation.Stock!.Symbol)
            .ToListAsync(cancellationToken);
    }

    public Task AddAsync(StockSimulation simulation, CancellationToken cancellationToken = default)
    {
        return _context.StockSimulations.AddAsync(simulation, cancellationToken).AsTask();
    }

    public void Update(StockSimulation simulation)
    {
        simulation.UpdatedAt = DateTime.UtcNow;
        _context.StockSimulations.Update(simulation);
    }

    public void Remove(StockSimulation simulation)
    {
        _context.StockSimulations.Remove(simulation);
    }
}
