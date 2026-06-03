namespace StockExchange.Data.Repositories.Interfaces;

public interface IUnitOfWork : IAsyncDisposable, IDisposable
{
    IUserRepository Users { get; }
    IStockRepository Stocks { get; }
    IWatchlistRepository Watchlists { get; }
    IStockPriceMinuteRepository StockPriceMinutes { get; }
    IStockPriceDayRepository StockPriceDays { get; }
    IStockSimulationRepository StockSimulations { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
