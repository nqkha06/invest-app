using StockExchange.Data.Context;
using StockExchange.Data.Repositories.Interfaces;

namespace StockExchange.Data.Repositories.Implementations;

public class UnitOfWork : IUnitOfWork
{
    private readonly StockExchangeDbContext _context;

    public UnitOfWork(StockExchangeDbContext context)
    {
        _context = context;
        Users = new UserRepository(context);
        Stocks = new StockRepository(context);
        Watchlists = new WatchlistRepository(context);
        StockPriceMinutes = new StockPriceMinuteRepository(context);
        StockPriceDays = new StockPriceDayRepository(context);
        StockSimulations = new StockSimulationRepository(context);
    }

    public IUserRepository Users { get; }
    public IStockRepository Stocks { get; }
    public IWatchlistRepository Watchlists { get; }
    public IStockPriceMinuteRepository StockPriceMinutes { get; }
    public IStockPriceDayRepository StockPriceDays { get; }
    public IStockSimulationRepository StockSimulations { get; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    public ValueTask DisposeAsync()
    {
        return _context.DisposeAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
