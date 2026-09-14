using Tutor.Core.Services.Abstractions;

namespace Tutor.Core.Services;

/// <summary>
/// Shared CRUD surface over each LLM provider's BYO API-key pool (<see cref="ISecurePreferences"/>'s
/// <c>GetApiKeysAsync</c>/<c>SetApiKeysAsync</c>), so the CLI (<c>tutor keys</c>) and the MCP tool
/// surface manage it identically instead of duplicating add/remove logic — either front door sees
/// the other's writes immediately, since both read/write the same Vault-backed store.
/// </summary>
public sealed class ApiKeyPoolService
{
    /// <summary>Providers Tutor's chat pipeline actually resolves keys for (Kimi exists as a
    /// service but isn't wired into either front door today, so it's out of scope here too).</summary>
    public static readonly string[] Providers = ["claude", "openai", "gemini", "deepseek"];

    private static readonly Dictionary<string, string> ApiKeyNameByProvider = new(StringComparer.OrdinalIgnoreCase)
    {
        ["claude"] = "CLAUDE_API_KEY",
        ["openai"] = "OPENAI_API_KEY",
        ["gemini"] = "GEMINI_API_KEY",
        ["deepseek"] = "DEEPSEEK_API_KEY",
    };

    private readonly ISecurePreferences prefs;

    public ApiKeyPoolService(ISecurePreferences prefs) => this.prefs = prefs;

    public static bool IsSupportedProvider(string provider) => ApiKeyNameByProvider.ContainsKey(provider);

    public Task<IReadOnlyList<string>> GetPoolAsync(string provider) =>
        prefs.GetApiKeysAsync(ApiKeyNameFor(provider));

    public Task SetPoolAsync(string provider, IReadOnlyList<string> keys) =>
        prefs.SetApiKeysAsync(ApiKeyNameFor(provider), keys);

    /// <summary>Appends one key to the end of the pool. Returns the new pool size.</summary>
    public async Task<int> AddKeyAsync(string provider, string key)
    {
        var current = (await GetPoolAsync(provider)).ToList();
        current.Add(key);
        await SetPoolAsync(provider, current);
        return current.Count;
    }

    /// <summary>Removes one key (by exact value). Returns whether it was found, plus the
    /// remaining pool size.</summary>
    public async Task<(bool Removed, int Remaining)> RemoveKeyAsync(string provider, string key)
    {
        var current = (await GetPoolAsync(provider)).ToList();
        var removed = current.RemoveAll(k => k == key) > 0;
        await SetPoolAsync(provider, current);
        return (removed, current.Count);
    }

    public Task ClearAsync(string provider) => SetPoolAsync(provider, Array.Empty<string>());

    private static string ApiKeyNameFor(string provider) =>
        ApiKeyNameByProvider.TryGetValue(provider, out var name)
            ? name
            : throw new ArgumentException(
                $"Unknown provider '{provider}'. Supported: {string.Join(", ", Providers)}", nameof(provider));
}
