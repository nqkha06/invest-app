using Microsoft.EntityFrameworkCore;
using StockExchange.Shared.Models;

namespace StockExchange.Data.Context;

public class StockExchangeDbContext : DbContext
{
    public StockExchangeDbContext(DbContextOptions<StockExchangeDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Stock> Stocks => Set<Stock>();
    public DbSet<StockSimulation> StockSimulations => Set<StockSimulation>();
    public DbSet<StockPriceMinute> StockPricesMinute => Set<StockPriceMinute>();
    public DbSet<StockPriceDay> StockPricesDay => Set<StockPriceDay>();
    public DbSet<UserWatchlist> UserWatchlists => Set<UserWatchlist>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(StockExchangeDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
