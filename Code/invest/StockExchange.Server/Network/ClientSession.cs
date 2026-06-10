using System.Net.Sockets;
using System.Text;

namespace StockExchange.Server.Network;

public sealed class ClientSession : IAsyncDisposable
{
    private int _disposed;

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

    public ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
        {
            return ValueTask.CompletedTask;
        }

        Reader.Dispose();
        Writer.Dispose();
        Client.Dispose();
        return ValueTask.CompletedTask;
    }
}
