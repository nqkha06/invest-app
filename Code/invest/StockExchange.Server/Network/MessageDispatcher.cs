using System.Text.Json;
using StockExchange.Server.Handlers;
using StockExchange.Shared.DTOs;
using StockExchange.Shared.Network;

namespace StockExchange.Server.Network;

public class MessageDispatcher
{
    private readonly AuthMessageHandler _authHandler;
    private readonly StockMessageHandler _stockHandler; // THÊM DÒNG NÀY
    public MessageDispatcher(AuthMessageHandler authHandler,StockMessageHandler stockHandler)
    {
        _authHandler = authHandler;
        _stockHandler = stockHandler;
    }

    public async Task<AppMessage> DispatchAsync(
        ClientSession session,
        AppMessage message,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return message.Type switch
            {
                MessageType.Login => await LoginAsync(session, message, cancellationToken),
                MessageType.Register => await RegisterAsync(session, message, cancellationToken),
                MessageType.GetProfile => await GetProfileAsync(session, message, cancellationToken),
                MessageType.UpdateProfile => await UpdateProfileAsync(session, message, cancellationToken),
                MessageType.GetAllStocks => await GetAllStocksAsync(message),
                MessageType.SearchStocks => await SearchStocksAsync(message),
                MessageType.CreateStock => await CreateStockAsync(session, message, cancellationToken),
                MessageType.UpdateStock => await UpdateStockAsync(session, message, cancellationToken),
                MessageType.DeleteStock => await DeleteStockAsync(session, message, cancellationToken),
                _ => AppMessage.Failure(message.Type, message.RequestId, "Unsupported message type.")
            };
        }   
        catch (JsonException)
        {
            return AppMessage.Failure(message.Type, message.RequestId, "Request payload is invalid.");
        }
        catch (InvalidOperationException ex)
        {
            return AppMessage.Failure(message.Type, message.RequestId, ex.Message);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Request {message.RequestId} failed: {ex}");
            return AppMessage.Failure(message.Type, message.RequestId, "The server could not process the request.");
        }
    }

    private async Task<AppMessage> LoginAsync(
        ClientSession session,
        AppMessage message,
        CancellationToken cancellationToken)
    {
        var request = Deserialize<LoginRequestDto>(message);
        var response = await _authHandler.HandleLoginAsync(request, cancellationToken);
        session.UserId = response.Success ? response.UserId : null;
        return AppMessage.Create(message.Type, response, message.RequestId);
    }
   private async Task<AppMessage> GetAllStocksAsync(AppMessage message)
    {
        var response = await _stockHandler.HandleGetAllStocksAsync();
        return AppMessage.Create(message.Type, response, message.RequestId);
    }

    private async Task<AppMessage> SearchStocksAsync(AppMessage message)
    {
        var request = Deserialize<SearchStockRequestDto>(message);
        var response = await _stockHandler.HandleSearchStocksAsync(request);
        return AppMessage.Create(message.Type, response, message.RequestId);
    }

    private async Task<AppMessage> CreateStockAsync(
        ClientSession session,
        AppMessage message,
        CancellationToken cancellationToken)
    {
        var request = Deserialize<StockUpdateDto>(message);
        var response = await _stockHandler.HandleCreateStockAsync(
            RequireUser(session), request, cancellationToken);
        return AppMessage.Create(message.Type, response, message.RequestId);
    }

    private async Task<AppMessage> UpdateStockAsync(
        ClientSession session,
        AppMessage message,
        CancellationToken cancellationToken)
    {
        var request = Deserialize<StockUpdateDto>(message);
        var response = await _stockHandler.HandleUpdateStockAsync(
            RequireUser(session), request, cancellationToken);
        return AppMessage.Create(message.Type, response, message.RequestId);
    }

    private async Task<AppMessage> DeleteStockAsync(
        ClientSession session,
        AppMessage message,
        CancellationToken cancellationToken)
    {
        var request = Deserialize<StockDeleteRequestDto>(message);
        var response = await _stockHandler.HandleDeleteStockAsync(
            RequireUser(session), request, cancellationToken);
        return AppMessage.Create(message.Type, response, message.RequestId);
    }

    private async Task<AppMessage> RegisterAsync(
        ClientSession session,
        AppMessage message,
        CancellationToken cancellationToken)
    {
        var request = Deserialize<RegisterRequestDto>(message);
        var response = await _authHandler.HandleRegisterAsync(request, cancellationToken);
        if (response.Success)
        {
            session.UserId = response.UserId;
        }
        return AppMessage.Create(message.Type, response, message.RequestId);
    }

    private async Task<AppMessage> GetProfileAsync(
        ClientSession session,
        AppMessage message,
        CancellationToken cancellationToken)
    {
        var userId = RequireUser(session);
        var response = await _authHandler.HandleGetProfileAsync(userId, cancellationToken);
        return AppMessage.Create(message.Type, response, message.RequestId);
    }

    private async Task<AppMessage> UpdateProfileAsync(
        ClientSession session,
        AppMessage message,
        CancellationToken cancellationToken)
    {
        var userId = RequireUser(session);
        var request = Deserialize<UpdateProfileRequestDto>(message);
        var response = await _authHandler.HandleUpdateProfileAsync(userId, request, cancellationToken);
        return AppMessage.Create(message.Type, response, message.RequestId);
    }

    private static T Deserialize<T>(AppMessage message)
    {
        return message.Payload.Deserialize<T>()
            ?? throw new JsonException("Payload cannot be empty.");
    }

    private static long RequireUser(ClientSession session)
    {
        return session.UserId
            ?? throw new InvalidOperationException("Please log in before performing this action.");
    }
}
