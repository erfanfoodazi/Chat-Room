using System.Collections.Concurrent;

namespace ChatRoomApp.Services;

public class ConnectionTracker
{
    private readonly ConcurrentDictionary<int, ConcurrentHashSet<string>> _connections = new();

    public int AddConnection(int userId, string connectionId)
    {
        var connections = _connections.GetOrAdd(userId, _ => new ConcurrentHashSet<string>());
        connections.Add(connectionId);
        return connections.Count;
    }

    public int RemoveConnection(int userId, string connectionId)
    {
        if (_connections.TryGetValue(userId, out var connections))
        {
            connections.TryRemove(connectionId);
            return connections.Count;
        }
        return 0;
    }
}

public class ConcurrentHashSet<T> where T : notnull
{
    private readonly ConcurrentDictionary<T, byte> _items = new();

    public void Add(T item) => _items.TryAdd(item, 0);

    public bool TryRemove(T item) => _items.TryRemove(item, out _);

    public int Count => _items.Count;
}
