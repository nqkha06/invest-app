using System.ComponentModel.DataAnnotations;

namespace StockExchange.Shared.DTOs;

public class PriceHistoryRequestDto
{
    [Required]
    [MaxLength(20)]
    public string Symbol { get; set; } = string.Empty;

    [MaxLength(10)]
    public string? Interval { get; set; } = "1D";

    public DateTime? From { get; set; }

    public DateTime? To { get; set; }
}
