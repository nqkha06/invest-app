using Microsoft.EntityFrameworkCore;
using Invest.Api.Entities;

namespace Invest.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    
    public DbSet<User> Users => Set<User>();
    
    // 2. ĐĂNG KÝ ĐỒNG BỘ: Bảng StockPriceDay chính thức 
    public DbSet<StockPriceDay> StockPricesDays => Set<StockPriceDay>();

    // 3. Cấu hình Fluent API để tạo Index tối ưu dữ liệu (Đoạn code dài)
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<StockPriceDay>(entity =>
        {
            // Tạo Composite Index tăng tốc độ tìm kiếm theo cặp (StockId, TradingDate)
            entity.HasIndex(e => new { e.StockId, e.TradingDate })
                  .HasDatabaseName("IX_stock_prices_day_stock_id_trading_date");
        });
    }
}