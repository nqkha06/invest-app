using Invest.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Invest.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(
        DbContextOptions<AppDbContext> options
    ) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<UserWatchlist> UserWatchlists => Set<UserWatchlist>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserWatchlist>(entity =>
        {
            entity.ToTable("user_watchlists");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .HasColumnName("id");

            entity.Property(x => x.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            entity.Property(x => x.StockId)
                .HasColumnName("stock_id")
                .IsRequired()
                .HasMaxLength(32);

            entity.Property(x => x.AddedAtUtc)
                .HasColumnName("added_at_utc")
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(x => x.AlertPrice)
                .HasColumnName("alert_price")
                .HasColumnType("decimal(18,2)");

            entity.Property(x => x.AlertPercent)
                .HasColumnName("alert_percent")
                .HasColumnType("decimal(5,2)");

            entity.Property(x => x.IsPinned)
                .HasColumnName("is_pinned")
                .HasDefaultValue(false);

            entity.Property(x => x.Note)
                .HasColumnName("note");

            entity.HasIndex(x => new { x.UserId, x.StockId })
                .IsUnique();

            entity.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}