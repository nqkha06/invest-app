namespace Invest.Api.Entities;

public class UserWatchlist
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string StockId { get; set; } = string.Empty;

    public DateTime AddedAtUtc { get; set; }

    public decimal? AlertPrice { get; set; }

    public decimal? AlertPercent { get; set; }

    public bool IsPinned { get; set; }

    public string? Note { get; set; }

    public User? User { get; set; }
}