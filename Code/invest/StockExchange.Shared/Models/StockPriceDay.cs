using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockExchange.Shared.Models;

[Table("stock_prices_day")]
public class StockPriceDay
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("stock_id")]
    public long StockId { get; set; }

    [Column("open_price")]
    public decimal OpenPrice { get; set; }

    [Column("high_price")]
    public decimal HighPrice { get; set; }

    [Column("low_price")]
    public decimal LowPrice { get; set; }

    [Column("close_price")]
    public decimal ClosePrice { get; set; }

    [Column("volume")]
    public long Volume { get; set; }

    [Column("trading_date")]
    public DateOnly TradingDate { get; set; }

    [ForeignKey(nameof(StockId))]
    public virtual Stock? Stock { get; set; }
}
