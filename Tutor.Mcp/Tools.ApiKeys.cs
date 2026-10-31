using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;
using Tutor.Core.Services;

namespace Tutor.Mcp;

// ── LLM keys (read-only) ─────────────────────────────────────────────────
// LAMP (TUT-T31): Tutor's keys live in MindAttic Vault's shared named lists
// and are added, changed and assigned only in MindAttic.Vault.Desktop. This
// tool reports which keys Tutor would use, by NAME (never the value), in the
// order it tries them; the old set/add/remove/clear tools are gone.
// ─────────────────────────────────────────────────────────────────────────

[McpServerToolType]
public class ApiKeyTools
{
    private static readonly JsonSerializerOptions JsonOpts = new() { WriteIndented = false };
    private static readonly string[] Providers = ["claude", "openai", "gemini", "deepseek", "kimi"];

    private readonly TutorKeys keys;

    public ApiKeyTools(TutorKeys keys)
    {
        this.keys = keys;
    }

    [McpServerTool, Description(
        "List the API keys Tutor uses for an LLM provider (claude, openai, gemini, deepseek or kimi), by " +
        "name only and in the order Tutor tries them, failing over to the next when one is refused. Keys " +
        "are managed in MindAttic Vault (Keys tab and Tutor's tab), not here. Pass no provider to list all.")]
    public string ListApiKeys(
        [Description("'claude', 'openai', 'gemini', 'deepseek' or 'kimi'. Omit to list every provider.")] string? provider = null)
    {
        if (provider is not null && !Providers.Contains(provider, StringComparer.OrdinalIgnoreCase))
            return JsonSerializer.Serialize(new { error = $"unknown_provider: expected one of {string.Join(", ", Providers)}" }, JsonOpts);

        var result = new Dictionary<string, object>();
        foreach (var p in provider is null ? Providers : new[] { provider.ToLowerInvariant() })
            result[p] = keys.KeyNames(p);
        return JsonSerializer.Serialize(new { keys = result, managedIn = "MindAttic Vault" }, JsonOpts);
    }
}
