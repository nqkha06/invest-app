namespace Invest.Api.Entities;

public class UserWatchlist
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public long StockId { get; set; }

    public DateTime CreatedAt { get; set; }

    public User? User { get; set; }
}