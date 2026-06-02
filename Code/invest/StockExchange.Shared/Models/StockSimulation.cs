using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockExchange.Shared.Models;

[Table("stock_simulations")]
public class StockSimulation
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("stock_id")]
    public long StockId { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("algorithm_type")]
    public string AlgorithmType { get; set; } = string.Empty;

    [Column("volatility")]
    public decimal Volatility { get; set; }

    [Column("trend_factor")]
    public decimal TrendFactor { get; set; }

    [Column("min_price")]
    public decimal MinPrice { get; set; }

    [Column("max_price")]
    public decimal MaxPrice { get; set; }

    [Column("update_speed")]
    public decimal UpdateSpeed { get; set; }

    [Column("jump_probability")]
    public decimal JumpProbability { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [ForeignKey(nameof(StockId))]
    public virtual Stock? Stock { get; set; }
}
