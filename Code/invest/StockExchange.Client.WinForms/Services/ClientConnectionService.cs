using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using StockExchange.Shared.DTOs;
using StockExchange.Shared.Network;

namespace StockExchange.Client.WinForms.Services;

public sealed class ClientConnectionService : IAsyncDisposable
{
    private const int ServerPort = 5050;
    private readonly SemaphoreSlim _connectionLock = new(1, 1);
    private readonly SemaphoreSlim _writeLock = new(1, 1);
    private readonly ConcurrentDictionary<string, TaskCompletionSource<AppMessage>> _pendingRequests = new();
    private TcpClient? _client;
    private StreamReader? _reader;
    private StreamWriter? _writer;
    private CancellationTokenSource? _readLoopCancellation;

    public event EventHandler<StockPriceUpdateDto>? StockPriceUpdated;

    public async Task<TResponse> SendAsync<TRequest, TResponse>(
        MessageType type,
        TRequest request,
        CancellationToken cancellationToken = default)
    {
        await EnsureConnectedAsync(cancellationToken);
        var message = AppMessage.Create(type, request);
        var pending = new TaskCompletionSource<AppMessage>(TaskCreationOptions.RunContinuationsAsynchronously);
        _pendingRequests[message.RequestId] = pending;

        try
        {
            await _writeLock.WaitAsync(cancellationToken);
            try
            {
                await _writer!.WriteLineAsync(JsonSerializer.Serialize(message));
            }
            finally
            {
                _writeLock.Release();
            }

            await using var registration = cancellationToken.Register(() => pending.TrySetCanceled(cancellationToken));
            var response = await pending.Task;
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
            throw new IOException("Khong the ket noi toi server. Hay kiem tra server dang chay.", ex);
        }
        finally
        {
            _pendingRequests.TryRemove(message.RequestId, out _);
        }
    }

    private async Task EnsureConnectedAsync(CancellationToken cancellationToken)
    {
        if (_client?.Connected == true)
        {
            return;
        }

        await _connectionLock.WaitAsync(cancellationToken);
        try
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
            _readLoopCancellation = new CancellationTokenSource();
            _ = Task.Run(() => ReadLoopAsync(_readLoopCancellation.Token));
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    private async Task ReadLoopAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var line = await _reader!.ReadLineAsync(cancellationToken);
                if (line is null)
                {
                    break;
                }

                var message = JsonSerializer.Deserialize<AppMessage>(line);
                if (message is null)
                {
                    continue;
                }

                if (message.Type == MessageType.StockPriceUpdated)
                {
                    var update = message.Payload.Deserialize<StockPriceUpdateDto>();
                    if (update is not null)
                    {
                        StockPriceUpdated?.Invoke(this, update);
                    }
                    continue;
                }

                if (_pendingRequests.TryRemove(message.RequestId, out var pending))
                {
                    pending.TrySetResult(message);
                }
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        catch (Exception ex)
        {
            foreach (var pending in _pendingRequests.Values)
            {
                pending.TrySetException(ex);
            }
        }
        finally
        {
            ResetConnection();
        }
    }

    private void ResetConnection()
    {
        _readLoopCancellation?.Cancel();
        _reader?.Dispose();
        _writer?.Dispose();
        _client?.Dispose();
        _readLoopCancellation?.Dispose();
        _reader = null;
        _writer = null;
        _client = null;
        _readLoopCancellation = null;
    }

    public ValueTask DisposeAsync()
    {
        ResetConnection();
        _connectionLock.Dispose();
        _writeLock.Dispose();
        return ValueTask.CompletedTask;
    }
}
