using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Invest.Api.Entities;

[Table("stock_prices_minute")]
public class StockPriceMinute
{
    [Key]
    [Column("id")]
    public int Id { get; set; } 

    // Nếu ERD của bạn nối với bảng Stock (Mã chứng khoán), thường sẽ có StockId hoặc Symbol
    [Required]
    [Column("symbol")]
    [MaxLength(10)]
    public string Symbol { get; set; } = string.Empty; 

    [Required]
    [Column("time")]
    public DateTime Time { get; set; } // Thời gian của cây nến phút đó

    [Column("open_price", TypeName = "decimal(18, 4)")]
    public decimal OpenPrice { get; set; } 

    [Column("high_price", TypeName = "decimal(18, 4)")]
    public decimal HighPrice { get; set; } 

    [Column("low_price", TypeName = "decimal(18, 4)")]
    public decimal LowPrice { get; set; } 

    [Column("close_price", TypeName = "decimal(18, 4)")]
    public decimal ClosePrice { get; set; } 

    [Column("volume")]
    public long Volume { get; set; } 
}