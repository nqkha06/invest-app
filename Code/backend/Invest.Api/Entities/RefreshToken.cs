namespace Invest.Api.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("refresh_tokens")]
public class RefreshToken
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [Column("user_id")]
    public long UserId { get; set; }

    [Required]
    [Column("token")]
    [StringLength(500)] 
    public string Token { get; set; } = string.Empty; 

    [Column("expired_at")]
    public DateTime ExpiredAt { get; set; } 

    [Column("revoked_at")]
    public DateTime? RevokedAt { get; set; } 

    [Column("is_used")]
    public bool IsUsed { get; set; } 

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
