namespace StockExchange.Shared.DTOs;

public class StockPriceUpdateDto
{
    public long StockId { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal PreviousPrice { get; set; }
    public decimal ChangePercent { get; set; }
    public DateTime UpdatedAt { get; set; }

    // OHLCV của phiên hôm nay — được broadcast cùng giá để client không cần tính mock
    public decimal OpenPrice { get; set; }
    public decimal HighPrice { get; set; }
    public decimal LowPrice { get; set; }
    public long Volume { get; set; }
}