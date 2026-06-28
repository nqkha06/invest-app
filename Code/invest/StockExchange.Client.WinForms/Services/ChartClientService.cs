using StockExchange.Shared.DTOs;
using StockExchange.Shared.Network;

namespace StockExchange.Client.WinForms.Services;

public class ChartClientService
{
    private readonly ClientConnectionService _connection;

    public ChartClientService(ClientConnectionService connection)
    {
        _connection = connection;
    }

    public Task<List<PriceHistoryDto>> GetHistoryAsync(
        string symbol,
        string interval,
        CancellationToken cancellationToken = default)
    {
        return _connection.SendAsync<PriceHistoryRequestDto, List<PriceHistoryDto>>(
            MessageType.GetPriceHistory,
            new PriceHistoryRequestDto
            {
                Symbol = symbol,
                Interval = interval
            },
            cancellationToken);
    }
}
