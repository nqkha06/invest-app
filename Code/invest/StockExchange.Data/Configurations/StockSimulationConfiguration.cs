using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockExchange.Shared.Models;

namespace StockExchange.Data.Configurations;

public class StockSimulationConfiguration : IEntityTypeConfiguration<StockSimulation>
{
    public void Configure(EntityTypeBuilder<StockSimulation> builder)
    {
        builder.ToTable("stock_simulations");
        builder.HasKey(simulation => simulation.Id);

        builder.Property(simulation => simulation.Id).HasColumnName("id");
        builder.Property(simulation => simulation.StockId).HasColumnName("stock_id").IsRequired();
        builder.Property(simulation => simulation.AlgorithmType).HasColumnName("algorithm_type").HasMaxLength(50).IsRequired();
        builder.Property(simulation => simulation.Volatility).HasColumnName("volatility").HasPrecision(18, 6).IsRequired();
        builder.Property(simulation => simulation.TrendFactor).HasColumnName("trend_factor").HasPrecision(18, 6).IsRequired();
        builder.Property(simulation => simulation.MinPrice).HasColumnName("min_price").HasPrecision(18, 4).IsRequired();
        builder.Property(simulation => simulation.MaxPrice).HasColumnName("max_price").HasPrecision(18, 4).IsRequired();
        builder.Property(simulation => simulation.UpdateSpeed).HasColumnName("update_speed").HasPrecision(18, 6).IsRequired();
        builder.Property(simulation => simulation.JumpProbability).HasColumnName("jump_probability").HasPrecision(18, 6).IsRequired();
        builder.Property(simulation => simulation.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(simulation => simulation.StockId);

        builder.HasOne(simulation => simulation.Stock)
            .WithMany(stock => stock.Simulations)
            .HasForeignKey(simulation => simulation.StockId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
