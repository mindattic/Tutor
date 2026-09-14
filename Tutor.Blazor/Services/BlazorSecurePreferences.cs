using System.Text.Json;
using MindAttic.Vault.Credentials;
using Tutor.Core.Services.Abstractions;

namespace Tutor.Blazor.Services;

/// <summary>
/// File-backed preferences store for Tutor. LLM API keys are NOT stored in the local
/// preferences file — a key entered in Settings writes into MindAttic.Vault under Tutor's OWN
/// provider id (an <see cref="AppScopedCredentialStore"/> scoped to <c>"tutor"</c>), so it never
/// changes what another MindAttic app resolves. Reading falls back to the shared cross-app id
/// (User Secrets / env / App Service / Key Vault / <c>%APPDATA%\MindAttic\LLM\providers.json</c>,
/// via <see cref="LlmCredentialResolver"/>) only when Tutor has none of its own. Model-name
/// fields stay read-only from the shared id, unchanged.
///
/// Ordinary preferences (theme, SELECTED_MODEL, ENTER_TO_SEND, …) continue to live in
/// the local <c>secure-preferences.json</c>.
/// </summary>
public class BlazorSecurePreferences : ISecurePreferences, IDisposable
{
    private readonly string filePath;
    private readonly LlmCredentialResolver vault;
    private readonly CompositeCredentialStore keys;
    private Dictionary<string, string> store = new();
    private readonly SemaphoreSlim @lock = new(1, 1);

    // Tutor's ISecurePreferences keys → (provider, isApiKey) resolved from Vault.
    // Provider IDs follow the MindAttic cross-app convention (claude/openai/gemini/deepseek).
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

    /// <summary>Loads local (non-secret) preferences; LLM keys are read from Vault on demand.</summary>
    public BlazorSecurePreferences(LlmCredentialResolver vault)
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

    /// <summary>
    /// Writes a preference. An LLM API key writes into Vault under Tutor's own provider id
    /// (never the shared one); a model-name field is still Vault-managed/read-only and ignored;
    /// everything else is an ordinary local preference.
    /// </summary>
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

    /// <summary>Removes a preference. An LLM API key clears Tutor's own Vault override (the
    /// shared default, if any, takes back over); a model-name field stays Vault-managed/ignored.</summary>
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

    /// <summary>Every key configured for an LLM API-key preference, in priority order (Tutor's
    /// own Vault override first, then the shared cross-app pool).</summary>
    public Task<IReadOnlyList<string>> GetApiKeysAsync(string key)
    {
        if (!LlmKeyMap.TryGetValue(key, out var map) || !map.IsApiKey)
            throw new NotSupportedException($"'{key}' is not an LLM API-key preference.");
        IReadOnlyList<string> result = keys.GetKeys(map.Provider).Select(k => k.Key).ToList();
        return Task.FromResult(result);
    }

    /// <summary>Replaces the whole key pool for an LLM API-key preference, writing into Vault
    /// under Tutor's own provider id (never the shared one).</summary>
    public Task SetApiKeysAsync(string key, IReadOnlyList<string> apiKeys)
    {
        if (!LlmKeyMap.TryGetValue(key, out var map) || !map.IsApiKey)
            throw new NotSupportedException($"'{key}' is not an LLM API-key preference.");
        keys.SetKeys(map.Provider, apiKeys.Select(k => new CredentialPoolEntry(k)).ToList());
        return Task.CompletedTask;
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
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[BlazorSecurePreferences] Failed to load preferences: {ex.Message}");
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
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[BlazorSecurePreferences] Failed to save preferences: {ex.Message}");
        }
    }

    private void SaveSync()
    {
        try
        {
            var json = JsonSerializer.Serialize(store, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[BlazorSecurePreferences] Failed to save preferences: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        @lock.Dispose();
    }
}
