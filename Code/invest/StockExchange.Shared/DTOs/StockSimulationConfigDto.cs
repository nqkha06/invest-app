namespace StockExchange.Shared.DTOs;

public class StockSimulationConfigDto
{
    public long Id { get; set; }
    public long StockId { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string AlgorithmType { get; set; } = string.Empty;
    public decimal Volatility { get; set; }
    public decimal TrendFactor { get; set; }
    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }
    public decimal UpdateSpeed { get; set; }
    public decimal JumpProbability { get; set; }
    public DateTime UpdatedAt { get; set; }
}
