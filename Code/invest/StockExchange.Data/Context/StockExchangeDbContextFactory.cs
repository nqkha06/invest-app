using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace StockExchange.Data.Context;

public class StockExchangeDbContextFactory : IDesignTimeDbContextFactory<StockExchangeDbContext>
{
    public StockExchangeDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<StockExchangeDbContext>();
        optionsBuilder.UseSqlite("Data Source=stock_exchange.db");

        return new StockExchangeDbContext(optionsBuilder.Options);
    }
}
