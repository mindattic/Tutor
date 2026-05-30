using System.Text.Json;
using Tutor.Core.Services.Abstractions;
using LlmCredentialResolver = MindAttic.Vault.Credentials.LlmCredentialResolver;

namespace Tutor.Cli.Services;

/// <summary>
/// Mirrors <c>Tutor.Blazor.Services.BlazorSecurePreferences</c> — keeps the same on-disk
/// path (%LocalAppData%/Tutor/Settings/secure-preferences.json) so courses created from
/// the CLI are visible to the Blazor UI and vice versa. LLM API keys and models are NOT
/// stored in-app: they resolve read-only through MindAttic.Vault's
/// <see cref="LlmCredentialResolver"/> (User Secrets / env / App Service / Key Vault →
/// the shared %APPDATA%/MindAttic/LLM/providers.json). Ordinary preferences
/// (SELECTED_MODEL, ENTER_TO_SEND, …) live in the local JSON.
/// </summary>
public sealed class CliSecurePreferences : ISecurePreferences, IDisposable
{
    private readonly string filePath;
    private readonly LlmCredentialResolver vault;
    private Dictionary<string, string> store = new();
    private readonly SemaphoreSlim @lock = new(1, 1);

    private static readonly Dictionary<string, (string Provider, bool IsApiKey)> LlmKeyMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["OPENAI_API_KEY"]    = ("openai",   true),
        ["CHATGPT_MODEL"]     = ("openai",   false),
        ["CLAUDE_API_KEY"]    = ("claude",   true),
        ["CLAUDE_MODEL"]      = ("claude",   false),
        ["GEMINI_API_KEY"]    = ("gemini",   true),
        ["GEMINI_MODEL"]      = ("gemini",   false),
        ["DEEPSEEK_API_KEY"]  = ("deepseek", true),
        ["DEEPSEEK_MODEL"]    = ("deepseek", false),
    };

    public CliSecurePreferences(LlmCredentialResolver vault)
    {
        this.vault = vault;
        var dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Tutor", "Settings");
        Directory.CreateDirectory(dir);
        filePath = Path.Combine(dir, "secure-preferences.json");
        Load();
    }

    /// <summary>Reads a preference. LLM-mapped keys resolve from Vault; everything else from local prefs.</summary>
    public Task<string?> GetAsync(string key)
    {
        if (LlmKeyMap.TryGetValue(key, out var map))
            return Task.FromResult(ReadFromVault(map.Provider, map.IsApiKey));

        store.TryGetValue(key, out var value);
        return Task.FromResult<string?>(value);
    }

    /// <summary>Writes a preference. LLM-mapped keys are Vault-managed (read-only) and ignored.</summary>
    public async Task SetAsync(string key, string value)
    {
        if (LlmKeyMap.ContainsKey(key)) return;

        await @lock.WaitAsync();
        try
        {
            store[key] = value;
            await SaveAsync();
        }
        finally
        {
            @lock.Release();
        }
    }

    /// <summary>Removes a preference. LLM-mapped keys are Vault-managed and ignored.</summary>
    public void Remove(string key)
    {
        if (LlmKeyMap.ContainsKey(key)) return;

        @lock.Wait();
        try
        {
            store.Remove(key);
            SaveSync();
        }
        finally
        {
            @lock.Release();
        }
    }

    // apiKey → resolver.GetKey; model → the provider's "model" field from the raw record.
    private string? ReadFromVault(string provider, bool isApiKey)
    {
        try
        {
            if (isApiKey)
            {
                var key = vault.GetKey(provider);
                return string.IsNullOrWhiteSpace(key) ? null : key;
            }
            if (vault.LoadAllRaw().TryGetValue(provider, out var raw) && !string.IsNullOrWhiteSpace(raw))
            {
                using var doc = JsonDocument.Parse(raw);
                if (doc.RootElement.TryGetProperty("model", out var m) && m.ValueKind == JsonValueKind.String)
                {
                    var model = m.GetString();
                    return string.IsNullOrWhiteSpace(model) ? null : model;
                }
            }
            return null;
        }
        catch
        {
            return null;
        }
    }

    private void Load()
    {
        try
        {
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                store = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new();
            }
        }
        catch
        {
            store = new();
        }
    }

    private async Task SaveAsync()
    {
        try
        {
            var json = JsonSerializer.Serialize(store, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(filePath, json);
        }
        catch
        {
            // Best-effort persistence
        }
    }

    private void SaveSync()
    {
        try
        {
            var json = JsonSerializer.Serialize(store, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }
        catch
        {
            // Best-effort persistence
        }
    }

    /// <inheritdoc />
    public void Dispose() => @lock.Dispose();
}
