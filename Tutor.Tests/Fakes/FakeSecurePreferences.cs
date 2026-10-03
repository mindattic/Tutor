using Tutor.Core.Services.Abstractions;

namespace Tutor.Tests.Fakes;

/// <summary>
/// In-memory implementation of ISecurePreferences for testing. API-key pools mirror the real
/// stores: a single key set via <see cref="SetAsync"/> reads back as a one-element pool, and
/// <see cref="SetApiKeysAsync"/> replaces the whole pool (index 0 is also what <see cref="GetAsync"/> returns).
/// </summary>
public class FakeSecurePreferences : ISecurePreferences
{
    private readonly Dictionary<string, string> store = new();
    private readonly Dictionary<string, List<string>> pools = new();

    public Task<string?> GetAsync(string key)
    {
        store.TryGetValue(key, out var value);
        return Task.FromResult(value);
    }

    public Task SetAsync(string key, string value)
    {
        store[key] = value;
        pools.Remove(key);
        return Task.CompletedTask;
    }

    public void Remove(string key)
    {
        store.Remove(key);
        pools.Remove(key);
    }

    public Task<IReadOnlyList<string>> GetApiKeysAsync(string key)
    {
        IReadOnlyList<string> result = pools.TryGetValue(key, out var pool)
            ? pool.ToList()
            : store.TryGetValue(key, out var single) && !string.IsNullOrWhiteSpace(single)
                ? new[] { single }
                : Array.Empty<string>();
        return Task.FromResult(result);
    }

    public Task SetApiKeysAsync(string key, IReadOnlyList<string> keys)
    {
        var cleaned = keys.Where(k => !string.IsNullOrWhiteSpace(k)).ToList();
        if (cleaned.Count == 0)
        {
            Remove(key);
            return Task.CompletedTask;
        }
        pools[key] = cleaned;
        store[key] = cleaned[0];
        return Task.CompletedTask;
    }

    public void Clear()
    {
        store.Clear();
        pools.Clear();
    }

    public int Count => store.Count;

    public bool ContainsKey(string key) => store.ContainsKey(key);
}
