using StockExchange.Shared.DTOs;
using StockExchange.Shared.Models;
using StockExchange.Shared.Network;

namespace StockExchange.Client.WinForms.Services;

public class WatchlistClientService
{
    private readonly ClientConnectionService _connection;

    public WatchlistClientService(ClientConnectionService connection)
    {
        _connection = connection;
    }

    /// <summary>Lấy danh sách cổ phiếu đang theo dõi của user hiện tại từ server.</summary>
    public Task<List<Stock>> GetAsync(CancellationToken cancellationToken = default)
    {
        return _connection.SendAsync<object, List<Stock>>(
            MessageType.GetWatchlist,
            new { },
            cancellationToken);
    }

    /// <summary>Thêm cổ phiếu vào watchlist. Trả về true nếu thêm thành công.</summary>
    public Task<bool> AddAsync(long stockId, CancellationToken cancellationToken = default)
    {
        return _connection.SendAsync<WatchlistModifyRequestDto, bool>(
            MessageType.AddToWatchlist,
            new WatchlistModifyRequestDto { StockId = stockId },
            cancellationToken);
    }

    /// <summary>Xóa cổ phiếu khỏi watchlist. Trả về true nếu xóa thành công.</summary>
    public Task<bool> RemoveAsync(long stockId, CancellationToken cancellationToken = default)
    {
        return _connection.SendAsync<WatchlistModifyRequestDto, bool>(
            MessageType.RemoveFromWatchlist,
            new WatchlistModifyRequestDto { StockId = stockId },
            cancellationToken);
    }
}