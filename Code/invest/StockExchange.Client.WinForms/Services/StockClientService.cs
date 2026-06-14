using StockExchange.Shared.DTOs;
using StockExchange.Shared.Models;
using StockExchange.Shared.Network;

namespace StockExchange.Client.WinForms.Services;

public class StockClientService
{
    private readonly ClientConnectionService _connection;

    public StockClientService(ClientConnectionService connection)
    {
        _connection = connection;
    }

    public Task<List<Stock>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _connection.SendAsync<object, List<Stock>>(
            MessageType.GetAllStocks,
            new { },
            cancellationToken);
    }

    public Task<Stock> CreateAsync(StockUpdateDto request, CancellationToken cancellationToken = default)
    {
        return _connection.SendAsync<StockUpdateDto, Stock>(
            MessageType.CreateStock,
            request,
            cancellationToken);
    }

    public Task<Stock> UpdateAsync(StockUpdateDto request, CancellationToken cancellationToken = default)
    {
        return _connection.SendAsync<StockUpdateDto, Stock>(
            MessageType.UpdateStock,
            request,
            cancellationToken);
    }

    public Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        return _connection.SendAsync<StockDeleteRequestDto, bool>(
            MessageType.DeleteStock,
            new StockDeleteRequestDto { Id = id },
            cancellationToken);
    }
}
