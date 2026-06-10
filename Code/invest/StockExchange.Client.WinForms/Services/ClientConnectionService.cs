using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using StockExchange.Shared.Network;

namespace StockExchange.Client.WinForms.Services;

public sealed class ClientConnectionService : IAsyncDisposable
{
    private const int ServerPort = 5050;
    private readonly SemaphoreSlim _requestLock = new(1, 1);
    private TcpClient? _client;
    private StreamReader? _reader;
    private StreamWriter? _writer;

    public async Task<TResponse> SendAsync<TRequest, TResponse>(
        MessageType type,
        TRequest request,
        CancellationToken cancellationToken = default)
    {
        await _requestLock.WaitAsync(cancellationToken);
        try
        {
            await EnsureConnectedAsync(cancellationToken);
            var message = AppMessage.Create(type, request);
            await _writer!.WriteLineAsync(JsonSerializer.Serialize(message));

            var line = await _reader!.ReadLineAsync(cancellationToken);
            if (line is null)
            {
                ResetConnection();
                throw new IOException("Server closed the connection.");
            }

            var response = JsonSerializer.Deserialize<AppMessage>(line)
                ?? throw new IOException("Server returned an invalid response.");
            if (response.RequestId != message.RequestId)
            {
                throw new IOException("Server response did not match the request.");
            }
            if (!response.Success)
            {
                throw new InvalidOperationException(response.Error);
            }

            return response.Payload.Deserialize<TResponse>()
                ?? throw new IOException("Server response payload was empty.");
        }
        catch (SocketException ex)
        {
            ResetConnection();
            throw new IOException("Không thể kết nối tới server. Hãy kiểm tra server đang chạy.", ex);
        }
        finally
        {
            _requestLock.Release();
        }
    }

    private async Task EnsureConnectedAsync(CancellationToken cancellationToken)
    {
        if (_client?.Connected == true)
        {
            return;
        }

        ResetConnection();
        _client = new TcpClient();
        await _client.ConnectAsync("127.0.0.1", ServerPort, cancellationToken);
        var stream = _client.GetStream();
        _reader = new StreamReader(stream, Encoding.UTF8, false, leaveOpen: true);
        _writer = new StreamWriter(stream, new UTF8Encoding(false), leaveOpen: true)
        {
            AutoFlush = true
        };
    }

    private void ResetConnection()
    {
        _reader?.Dispose();
        _writer?.Dispose();
        _client?.Dispose();
        _reader = null;
        _writer = null;
        _client = null;
    }

    public ValueTask DisposeAsync()
    {
        ResetConnection();
        _requestLock.Dispose();
        return ValueTask.CompletedTask;
    }
}
