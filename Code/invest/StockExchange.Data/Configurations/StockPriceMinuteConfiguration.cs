using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockExchange.Shared.Models;

namespace StockExchange.Data.Configurations;

public class StockPriceMinuteConfiguration : IEntityTypeConfiguration<StockPriceMinute>
{
    public void Configure(EntityTypeBuilder<StockPriceMinute> builder)
    {
        builder.ToTable("stock_prices_minute");
        builder.HasKey(price => price.Id);

        builder.Property(price => price.Id).HasColumnName("id");
        builder.Property(price => price.StockId).HasColumnName("stock_id").IsRequired();
        builder.Property(price => price.OpenPrice).HasColumnName("open_price").HasPrecision(18, 4).IsRequired();
        builder.Property(price => price.HighPrice).HasColumnName("high_price").HasPrecision(18, 4).IsRequired();
        builder.Property(price => price.LowPrice).HasColumnName("low_price").HasPrecision(18, 4).IsRequired();
        builder.Property(price => price.ClosePrice).HasColumnName("close_price").HasPrecision(18, 4).IsRequired();
        builder.Property(price => price.Volume).HasColumnName("volume").IsRequired();
        builder.Property(price => price.RecordedAt).HasColumnName("recorded_at").IsRequired();

        builder.HasIndex(price => new { price.StockId, price.RecordedAt }).IsUnique();

        builder.HasOne(price => price.Stock)
            .WithMany(stock => stock.MinutePrices)
            .HasForeignKey(price => price.StockId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
