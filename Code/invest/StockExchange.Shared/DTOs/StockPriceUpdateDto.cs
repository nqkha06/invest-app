namespace StockExchange.Shared.DTOs;

public class StockPriceUpdateDto
{
    public long StockId { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal PreviousPrice { get; set; }
    public decimal ChangePercent { get; set; }
    public DateTime UpdatedAt { get; set; }
}
