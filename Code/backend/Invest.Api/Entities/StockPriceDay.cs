using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Invest.Api.Entities;

[Table("stock_prices_day")]
public class StockPriceDay
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [Column("stock_id")]
    public long StockId { get; set; }

    [Required]
    [Column("open_price", TypeName = "decimal(18, 2)")]
    public decimal OpenPrice { get; set; }

    [Required]
    [Column("high_price", TypeName = "decimal(18, 2)")]
    public decimal HighPrice { get; set; }

    [Required]
    [Column("low_price", TypeName = "decimal(18, 2)")]
    public decimal LowPrice { get; set; }

    [Required]
    [Column("close_price", TypeName = "decimal(18, 2)")]
    public decimal ClosePrice { get; set; }

    [Required]
    [Column("volume")]
    public long Volume { get; set; }

    [Required]
    [Column("trading_date", TypeName = "date")]
    public DateTime TradingDate { get; set; }
}