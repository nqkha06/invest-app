namespace StockExchange.Shared.DTOs;

public class AdminDashboardDto
{
    public int TotalUsers { get; set; }
    public int ActiveStocks { get; set; }
    public int SimulationConfigs { get; set; }
    public int ConnectedClients { get; set; }
    public List<UserProfileDto> RecentUsers { get; set; } = [];
}
