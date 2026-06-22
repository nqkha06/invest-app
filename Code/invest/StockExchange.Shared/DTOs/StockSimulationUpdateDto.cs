namespace StockExchange.Shared.DTOs;

public class StockSimulationUpdateDto
{
    public long Id { get; set; }
    public string AlgorithmType { get; set; } = string.Empty;
    public decimal Volatility { get; set; }
    public decimal TrendFactor { get; set; }
    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }
    public decimal UpdateSpeed { get; set; }
    public decimal JumpProbability { get; set; }
}
