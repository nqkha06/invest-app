using StockExchange.Server.Services;
using StockExchange.Shared.DTOs;

namespace StockExchange.Server.Handlers;

public class ChartMessageHandler
{
    private readonly PriceHistoryService _priceHistoryService;

    public ChartMessageHandler(PriceHistoryService priceHistoryService)
    {
        _priceHistoryService = priceHistoryService;
    }

    public async Task<IReadOnlyList<PriceHistoryDto>> HandleGetPriceHistoryAsync(
        PriceHistoryRequestDto request,
        CancellationToken cancellationToken = default)
    {
        return await _priceHistoryService.GetAsync(request, cancellationToken)
            ?? Array.Empty<PriceHistoryDto>();
    }
}
