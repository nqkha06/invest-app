using System.Collections.Generic;
using System.Threading.Tasks;
using StockExchange.Shared.Models;
using StockExchange.Shared.DTOs;
using StockExchange.Server.Services;

namespace StockExchange.Server.Handlers;

public class StockMessageHandler
{
    private readonly StockService _stockService;
    private readonly StockSimulationService _simulationService;

    public StockMessageHandler(StockService stockService, StockSimulationService simulationService)
    {
        _stockService = stockService;
        _simulationService = simulationService;
    }

    public async Task<IEnumerable<Stock>> HandleGetAllStocksAsync()
    {
        // Gọi Service lấy toàn bộ cổ phiếu
        return await _stockService.GetAllStocksAsync();
    }

    public async Task<IEnumerable<Stock>> HandleSearchStocksAsync(SearchStockRequestDto request)
    {
        // Gọi Service tìm kiếm cổ phiếu theo keyword
        return await _stockService.SearchStocksAsync(request.Keyword);
    }

    public Task<Stock> HandleCreateStockAsync(
        long userId,
        StockUpdateDto request,
        CancellationToken cancellationToken = default)
    {
        return _stockService.CreateStockAsync(userId, request, cancellationToken);
    }

    public Task<Stock> HandleUpdateStockAsync(
        long userId,
        StockUpdateDto request,
        CancellationToken cancellationToken = default)
    {
        return _stockService.UpdateStockAsync(userId, request, cancellationToken);
    }

    public async Task<bool> HandleDeleteStockAsync(
        long userId,
        StockDeleteRequestDto request,
        CancellationToken cancellationToken = default)
    {
        await _stockService.DeleteStockAsync(userId, request.Id, cancellationToken);
        return true;
    }

    public Task<List<StockSimulationConfigDto>> HandleGetSimulationsAsync(
        long adminUserId,
        CancellationToken cancellationToken = default)
    {
        return _simulationService.GetAllAsync(adminUserId, cancellationToken);
    }

    public Task<StockSimulationConfigDto> HandleUpdateSimulationAsync(
        long adminUserId,
        StockSimulationUpdateDto request,
        CancellationToken cancellationToken = default)
    {
        return _simulationService.UpdateAsync(adminUserId, request, cancellationToken);
    }
}
