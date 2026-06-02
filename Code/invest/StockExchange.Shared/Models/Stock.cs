using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockExchange.Shared.Models;

[Table("stocks")]
public class Stock
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [MaxLength(20)]
    [Column("symbol")]
    public string Symbol { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    [Column("company_name")]
    public string CompanyName { get; set; } = string.Empty;

    [MaxLength(100)]
    [Column("sector")]
    public string? Sector { get; set; }

    [Column("current_price")]
    public decimal CurrentPrice { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    public virtual ICollection<UserWatchlist> Watchlists { get; set; } = new List<UserWatchlist>();
    public virtual ICollection<StockSimulation> Simulations { get; set; } = new List<StockSimulation>();
    public virtual ICollection<StockPriceMinute> MinutePrices { get; set; } = new List<StockPriceMinute>();
    public virtual ICollection<StockPriceDay> DayPrices { get; set; } = new List<StockPriceDay>();
}
