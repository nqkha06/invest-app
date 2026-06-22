using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using StockExchange.Shared.Network;

namespace StockExchange.Server.Network;

public sealed class ClientSession : IAsyncDisposable
{
    private int _disposed;
    private readonly SemaphoreSlim _writeLock = new(1, 1);

    public ClientSession(TcpClient client)
    {
        Client = client;
        var stream = client.GetStream();
        Reader = new StreamReader(stream, Encoding.UTF8, false, leaveOpen: true);
        Writer = new StreamWriter(stream, new UTF8Encoding(false), leaveOpen: true)
        {
            AutoFlush = true
        };
    }

    public TcpClient Client { get; }
    public StreamReader Reader { get; }
    public StreamWriter Writer { get; }
    public long? UserId { get; set; }

    public async Task SendAsync(AppMessage message, CancellationToken cancellationToken = default)
    {
        await _writeLock.WaitAsync(cancellationToken);
        try
        {
            await Writer.WriteLineAsync(JsonSerializer.Serialize(message));
        }
        finally
        {
            _writeLock.Release();
        }
    }

    public ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
        {
            return ValueTask.CompletedTask;
        }

        Reader.Dispose();
        Writer.Dispose();
        _writeLock.Dispose();
        Client.Dispose();
        return ValueTask.CompletedTask;
    }
}
