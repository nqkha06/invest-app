using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockExchange.Shared.Models;

namespace StockExchange.Data.Configurations;

public class UserWatchlistConfiguration : IEntityTypeConfiguration<UserWatchlist>
{
    public void Configure(EntityTypeBuilder<UserWatchlist> builder)
    {
        builder.ToTable("user_watchlists");
        builder.HasKey(watchlist => watchlist.Id);

        builder.Property(watchlist => watchlist.Id).HasColumnName("id");
        builder.Property(watchlist => watchlist.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(watchlist => watchlist.StockId).HasColumnName("stock_id").IsRequired();
        builder.Property(watchlist => watchlist.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(watchlist => new { watchlist.UserId, watchlist.StockId }).IsUnique();
        builder.HasIndex(watchlist => watchlist.StockId);

        builder.HasOne(watchlist => watchlist.User)
            .WithMany(user => user.Watchlists)
            .HasForeignKey(watchlist => watchlist.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(watchlist => watchlist.Stock)
            .WithMany(stock => stock.Watchlists)
            .HasForeignKey(watchlist => watchlist.StockId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
