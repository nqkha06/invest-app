using Microsoft.EntityFrameworkCore;
using StockExchange.Data.Context;

namespace StockExchange.Data.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAllAsync(StockExchangeDbContext context)
    {
        await context.Database.MigrateAsync();

        await UserSeeder.SeedAsync(context);
        await StockSeeder.SeedAsync(context);
        
        await context.SaveChangesAsync(); 

        await StockSimulationSeeder.SeedAsync(context);

        await context.SaveChangesAsync();
    }
}
