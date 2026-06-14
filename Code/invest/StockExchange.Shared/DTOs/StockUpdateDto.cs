namespace StockExchange.Shared.DTOs;

public class StockUpdateDto
{
    public long Id { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string? Sector { get; set; }
    public decimal CurrentPrice { get; set; }
    public bool IsActive { get; set; }
}
