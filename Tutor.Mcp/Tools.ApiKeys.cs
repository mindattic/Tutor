using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;
using Tutor.Core.Services;

namespace Tutor.Mcp;

// ── LLM BYO-key pool ─────────────────────────────────────────────────────
// Manages the personal API-key pool each Tutor LLM service (ClaudeService,
// OpenAIService, GeminiService, DeepSeekService) tries in order, failing over
// key-to-key on an auth/rate-limit/server error (KeyPoolFailover). Same
// underlying ApiKeyPoolService as `tutor keys` (KeysCommand) — either surface
// sees the other's writes immediately, since both read/write the same
// Vault-backed store.
// ─────────────────────────────────────────────────────────────────────────

[McpServerToolType]
public class ApiKeyTools
{
    private static readonly JsonSerializerOptions JsonOpts = new() { WriteIndented = false };
    private static readonly string[] Providers = ApiKeyPoolService.Providers;

    private readonly ApiKeyPoolService pool;

    public ApiKeyTools(ApiKeyPoolService pool)
    {
        this.pool = pool;
    }

    private static string? ValidateProvider(string provider) =>
        Providers.Contains(provider, StringComparer.OrdinalIgnoreCase)
            ? null
            : $"unknown_provider: expected one of {string.Join(", ", Providers)}";

    private static string Mask(string key) => key.Length >= 4 ? "…" + key[^4..] : "(short key)";

    [McpServerTool, Description(
        "List the BYO API key pool configured for a Tutor LLM provider (claude, openai, gemini, " +
        "or deepseek) — the keys tried in priority order before falling back to the shared " +
        "default, failing over to the next on an auth/rate-limit/server error. Keys are returned " +
        "masked (last 4 chars only). Pass no provider to list all four.")]
    public async Task<string> ListApiKeys(
        [Description("'claude', 'openai', 'gemini', or 'deepseek'. Omit to list every provider.")] string? provider = null)
    {
        var targets = provider is null ? Providers : new[] { provider };
        foreach (var p in targets)
        {
            var err = ValidateProvider(p);
            if (err != null) return JsonSerializer.Serialize(new { error = err }, JsonOpts);
        }

        var result = new Dictionary<string, object>();
        foreach (var p in targets)
            result[p] = (await pool.GetPoolAsync(p)).Select(Mask).ToList();
        return JsonSerializer.Serialize(result, JsonOpts);
    }

    [McpServerTool, Description(
        "Replace the WHOLE BYO API key pool for one Tutor LLM provider with the given keys, tried " +
        "in that order and failing over on error. Use add_api_key / remove_api_key instead to edit " +
        "the pool incrementally without retyping every key.")]
    public async Task<string> SetApiKeys(
        [Description("'claude', 'openai', 'gemini', or 'deepseek'.")] string provider,
        [Description("The full pool, in priority order. An empty list clears the provider (same as clear_api_keys).")] string[] keys)
    {
        var err = ValidateProvider(provider);
        if (err != null) return JsonSerializer.Serialize(new { error = err }, JsonOpts);

        await pool.SetPoolAsync(provider, keys);
        var current = await pool.GetPoolAsync(provider);
        return JsonSerializer.Serialize(new { provider, count = current.Count, keys = current.Select(Mask) }, JsonOpts);
    }

    [McpServerTool, Description(
        "Append one key to the end of a Tutor LLM provider's BYO key pool (tried last, after every " +
        "key already configured).")]
    public async Task<string> AddApiKey(
        [Description("'claude', 'openai', 'gemini', or 'deepseek'.")] string provider,
        [Description("The raw API key to add.")] string key)
    {
        var err = ValidateProvider(provider);
        if (err != null) return JsonSerializer.Serialize(new { error = err }, JsonOpts);
        if (string.IsNullOrWhiteSpace(key)) return JsonSerializer.Serialize(new { error = "key must not be blank" }, JsonOpts);

        var count = await pool.AddKeyAsync(provider, key);
        return JsonSerializer.Serialize(new { provider, added = Mask(key), count }, JsonOpts);
    }

    [McpServerTool, Description(
        "Remove one key (by exact value) from a Tutor LLM provider's BYO key pool. Returns " +
        "removed=false if that key wasn't configured.")]
    public async Task<string> RemoveApiKey(
        [Description("'claude', 'openai', 'gemini', or 'deepseek'.")] string provider,
        [Description("The exact raw API key to remove.")] string key)
    {
        var err = ValidateProvider(provider);
        if (err != null) return JsonSerializer.Serialize(new { error = err }, JsonOpts);

        var (removed, remaining) = await pool.RemoveKeyAsync(provider, key);
        return JsonSerializer.Serialize(new { provider, removed, key = Mask(key), count = remaining }, JsonOpts);
    }

    [McpServerTool, Description(
        "Clear the ENTIRE BYO key pool for one Tutor LLM provider — falls back to the shared " +
        "default credential store, if one is configured.")]
    public async Task<string> ClearApiKeys(
        [Description("'claude', 'openai', 'gemini', or 'deepseek'.")] string provider)
    {
        var err = ValidateProvider(provider);
        if (err != null) return JsonSerializer.Serialize(new { error = err }, JsonOpts);

        await pool.ClearAsync(provider);
        return JsonSerializer.Serialize(new { provider, cleared = true }, JsonOpts);
    }
}
