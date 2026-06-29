namespace StockExchange.Client.WinForms.Models;

public sealed class CandlePoint
{
    public DateTime Time { get; init; }
    public decimal Open { get; init; }
    public decimal High { get; init; }
    public decimal Low { get; init; }
    public decimal Close { get; init; }
    public long Volume { get; init; }
}
