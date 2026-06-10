using System.Collections.Concurrent;

namespace StockExchange.Server.Network;

public sealed class ClientManager
{
    private readonly ConcurrentDictionary<ClientSession, byte> _sessions = new();

    public int Count => _sessions.Count;

    public IReadOnlyCollection<ClientSession> GetActiveSessions()
    {
        return _sessions.Keys.ToArray();
    }

    public bool Add(ClientSession session)
    {
        ArgumentNullException.ThrowIfNull(session);
        return _sessions.TryAdd(session, 0);
    }

    public bool Remove(ClientSession session)
    {
        ArgumentNullException.ThrowIfNull(session);
        return _sessions.TryRemove(session, out _);
    }

    public async Task DisconnectAllAsync()
    {
        var sessions = _sessions.Keys.ToArray();
        _sessions.Clear();

        foreach (var session in sessions)
        {
            await session.DisposeAsync();
        }
    }
}
