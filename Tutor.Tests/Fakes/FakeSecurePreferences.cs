using Tutor.Core.Services.Abstractions;

namespace Tutor.Tests.Fakes;

/// <summary>
/// In-memory implementation of ISecurePreferences for testing.
/// </summary>
public class FakeSecurePreferences : ISecurePreferences
{
    private readonly Dictionary<string, string> store = new();

    public Task<string?> GetAsync(string key)
    {
        store.TryGetValue(key, out var value);
        return Task.FromResult(value);
    }

    public Task SetAsync(string key, string value)
    {
        store[key] = value;
        return Task.CompletedTask;
    }

    public void Remove(string key)
    {
        store.Remove(key);
    }

    public void Clear() => store.Clear();

    public int Count => store.Count;

    public bool ContainsKey(string key) => store.ContainsKey(key);
}
