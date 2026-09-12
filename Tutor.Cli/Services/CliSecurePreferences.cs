using System.Text.Json;
using Tutor.Core.Services.Abstractions;
using AppScopedCredentialStore = MindAttic.Vault.Credentials.AppScopedCredentialStore;
using CompositeCredentialStore = MindAttic.Vault.Credentials.CompositeCredentialStore;
using LlmCredentialResolver = MindAttic.Vault.Credentials.LlmCredentialResolver;

namespace Tutor.Cli.Services;

/// <summary>
/// Mirrors <c>Tutor.Blazor.Services.BlazorSecurePreferences</c> — keeps the same on-disk
/// path (%LocalAppData%/Tutor/Settings/secure-preferences.json) so courses created from
/// the CLI are visible to the Blazor UI and vice versa. An LLM API key writes into Vault under
/// Tutor's own provider id (an <see cref="AppScopedCredentialStore"/> scoped to <c>"tutor"</c>),
/// never the shared one; reading falls back to the shared cross-app id (User Secrets / env /
/// App Service / Key Vault / %APPDATA%/MindAttic/LLM/providers.json) only when Tutor has none of
/// its own. Model fields stay Vault-managed/read-only. Ordinary preferences (SELECTED_MODEL,
/// ENTER_TO_SEND, …) live in the local JSON.
/// </summary>
public sealed class CliSecurePreferences : ISecurePreferences, IDisposable
{
    private readonly string filePath;
    private readonly LlmCredentialResolver vault;
    private readonly CompositeCredentialStore keys;
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
        keys = new CompositeCredentialStore(new AppScopedCredentialStore("tutor", vault), vault);
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

    /// <summary>Writes a preference. An LLM API key writes into Vault under Tutor's own
    /// provider id; a model-name field stays Vault-managed/read-only and is ignored.</summary>
    public async Task SetAsync(string key, string value)
    {
        if (LlmKeyMap.TryGetValue(key, out var map))
        {
            if (map.IsApiKey) keys.SetKey(map.Provider, value);
            return;
        }

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

    /// <summary>Removes a preference. An LLM API key clears Tutor's own Vault override; a
    /// model-name field stays Vault-managed and is ignored.</summary>
    public void Remove(string key)
    {
        if (LlmKeyMap.TryGetValue(key, out var map))
        {
            if (map.IsApiKey) keys.SetKey(map.Provider, "");
            return;
        }

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

    // apiKey → this app's own Vault override, falling back to the shared cross-app key;
    // model → the shared provider's "model" field from the raw record (unchanged).
    private string? ReadFromVault(string provider, bool isApiKey)
    {
        try
        {
            if (isApiKey)
            {
                var key = keys.GetKey(provider);
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
