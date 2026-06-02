using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockExchange.Shared.Models;

namespace StockExchange.Data.Configurations;

public class StockConfiguration : IEntityTypeConfiguration<Stock>
{
    public void Configure(EntityTypeBuilder<Stock> builder)
    {
        builder.ToTable("stocks");
        builder.HasKey(stock => stock.Id);

        builder.Property(stock => stock.Id).HasColumnName("id");
        builder.Property(stock => stock.Symbol).HasColumnName("symbol").HasMaxLength(20).IsRequired();
        builder.Property(stock => stock.CompanyName).HasColumnName("company_name").HasMaxLength(255).IsRequired();
        builder.Property(stock => stock.Sector).HasColumnName("sector").HasMaxLength(100);
        builder.Property(stock => stock.CurrentPrice).HasColumnName("current_price").HasPrecision(18, 4).IsRequired();
        builder.Property(stock => stock.IsActive).HasColumnName("is_active").HasDefaultValue(true);
        builder.Property(stock => stock.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(stock => stock.Symbol).IsUnique();
        builder.HasIndex(stock => stock.Sector);
    }
}
