using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using StockExchange.Shared.Network;

namespace StockExchange.Server.Network;

public class TcpServer
{
    public const int DefaultPort = 5050;

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ClientManager _clientManager;
    private readonly TcpListener _listener;

    public TcpServer(IServiceScopeFactory scopeFactory, ClientManager clientManager)
    {
        _scopeFactory = scopeFactory;
        _clientManager = clientManager;
        _listener = new TcpListener(IPAddress.Loopback, DefaultPort);
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        _listener.Start();
        Console.WriteLine($"TCP server listening on 127.0.0.1:{DefaultPort}.");

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var client = await _listener.AcceptTcpClientAsync(cancellationToken);
                _ = HandleClientAsync(client, cancellationToken);
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        finally
        {
            _listener.Stop();
            await _clientManager.DisconnectAllAsync();
            Console.WriteLine("TCP server stopped accepting clients.");
        }
    }

    private async Task HandleClientAsync(TcpClient client, CancellationToken cancellationToken)
    {
        await using var session = new ClientSession(client);
        _clientManager.Add(session);
        Console.WriteLine($"Client connected. Active clients: {_clientManager.Count}.");

        await using var scope = _scopeFactory.CreateAsyncScope();
        

        try
        {
            var dispatcher = scope.ServiceProvider.GetRequiredService<MessageDispatcher>();
            while (!cancellationToken.IsCancellationRequested)
            {
                var line = await session.Reader.ReadLineAsync(cancellationToken);
                if (line is null)
                {
                    break;
                }

                AppMessage response;
                try
                {
                    var request = JsonSerializer.Deserialize<AppMessage>(line)
                        ?? throw new JsonException();
                    response = await dispatcher.DispatchAsync(session, request, cancellationToken);
                }
                catch (JsonException)
                {
                    response = AppMessage.Failure(0, string.Empty, "Message format is invalid.");
                }

                await session.SendAsync(response, cancellationToken);
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        catch (IOException)
        {
        }
        catch (SocketException)
        {
        }
        finally
        {
            _clientManager.Remove(session);
            Console.WriteLine($"Client disconnected. Active clients: {_clientManager.Count}.");
        }
    }
}
