using System.Net.Sockets;
using StockExchange.Server.Network;
using StockExchange.Shared.DTOs;
using StockExchange.Shared.Network;

namespace StockExchange.Server.Services;

public sealed class StockBroadcastService
{
    private readonly ClientManager _clientManager;

    public StockBroadcastService(ClientManager clientManager)
    {
        _clientManager = clientManager;
    }

    public async Task BroadcastPriceUpdateAsync(
        StockPriceUpdateDto update,
        CancellationToken cancellationToken = default)
    {
        var message = AppMessage.Create(MessageType.StockPriceUpdated, update);
        var sessions = _clientManager.GetActiveSessions();

        foreach (var session in sessions)
        {
            try
            {
                await session.SendAsync(message, cancellationToken);
            }
            catch (Exception ex) when (ex is IOException or SocketException or ObjectDisposedException)
            {
                _clientManager.Remove(session);
            }
        }
    }
}
