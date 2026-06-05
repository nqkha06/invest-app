using Microsoft.EntityFrameworkCore;
using StockExchange.Data.Context;

namespace StockExchange.Data.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAllAsync(StockExchangeDbContext context)
    {
        // 1. Áp dụng tất cả các migrations còn thiếu (Tạo DB tự động)
        await context.Database.MigrateAsync();

        // 2. Chạy các seeder
        await UserSeeder.SeedAsync(context);
        await StockSeeder.SeedAsync(context);
        
        // Lưu Stock trước để lấy ID cho phần Simulation
        await context.SaveChangesAsync(); 

        await StockSimulationSeeder.SeedAsync(context);

        // Lưu lần cuối
        await context.SaveChangesAsync();
    }
}