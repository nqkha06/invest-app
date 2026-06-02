using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockExchange.Shared.Models;

namespace StockExchange.Data.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_tokens");
        builder.HasKey(token => token.Id);

        builder.Property(token => token.Id).HasColumnName("id");
        builder.Property(token => token.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(token => token.Token).HasColumnName("token").IsRequired();
        builder.Property(token => token.ExpiredAt).HasColumnName("expired_at").IsRequired();
        builder.Property(token => token.RevokedAt).HasColumnName("revoked_at");
        builder.Property(token => token.IsUsed).HasColumnName("is_used").HasDefaultValue(false);
        builder.Property(token => token.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(token => token.Token).IsUnique();
        builder.HasIndex(token => token.UserId);

        builder.HasOne(token => token.User)
            .WithMany(user => user.RefreshTokens)
            .HasForeignKey(token => token.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
