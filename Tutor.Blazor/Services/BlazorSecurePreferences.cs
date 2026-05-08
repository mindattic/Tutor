using System.Text.Json;
using Tutor.Core.Services;
using Tutor.Core.Services.Abstractions;

namespace Tutor.Blazor.Services;

/// <summary>
/// File-backed preferences store for Tutor. LLM credentials are routed through
/// the shared <see cref="LlmCredentialStore"/> at %APPDATA%/MindAttic/LLM/ so a
/// key rotation in any MindAttic app propagates to every other app.
///
/// Resolution order for LLM keys:
///   1. Shared %APPDATA%/MindAttic/LLM/{provider}.json  (canonical)
///   2. Local secure-preferences.json                    (legacy fallback)
///
/// First-time reads that resolve via legacy are auto-migrated up into the shared
/// store so subsequent reads short-circuit on step 1.
/// </summary>
public class BlazorSecurePreferences : ISecurePreferences, IDisposable
{
    private readonly string filePath;
    private readonly LlmCredentialStore credentialStore;
    private Dictionary<string, string> store = new();
    private readonly SemaphoreSlim @lock = new(1, 1);

    // Tutor's ISecurePreferences keys → (provider, isApiKey) in the shared store.
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

    /// <summary>
    /// Loads any existing local preferences from disk; the LLM credential store is
    /// consulted lazily when keys are read or written.
    /// </summary>
    public BlazorSecurePreferences(LlmCredentialStore credentialStore)
    {
        this.credentialStore = credentialStore;
        var dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Tutor", "Settings");
        Directory.CreateDirectory(dir);
        filePath = Path.Combine(dir, "secure-preferences.json");
        Load();
    }

    /// <summary>
    /// Reads a preference. LLM-mapped keys try the shared MindAttic credential store
    /// first and fall back to the legacy local JSON; legacy hits are auto-migrated up
    /// into the shared store on first access.
    /// </summary>
    public async Task<string?> GetAsync(string key)
    {
        if (LlmKeyMap.TryGetValue(key, out var map))
        {
            var fromStore = ReadFromStore(map.Provider, map.IsApiKey);
            if (!string.IsNullOrEmpty(fromStore)) return fromStore;

            // Fallback to legacy local store; migrate up if found.
            if (store.TryGetValue(key, out var legacy) && !string.IsNullOrEmpty(legacy))
            {
                await @lock.WaitAsync();
                try
                {
                    if (!credentialStore.Exists(map.Provider) || string.IsNullOrEmpty(ReadFromStore(map.Provider, map.IsApiKey)))
                        WriteToStore(map.Provider, map.IsApiKey, legacy);
                }
                finally { @lock.Release(); }
                return legacy;
            }
            return null;
        }

        store.TryGetValue(key, out var value);
        return value;
    }

    /// <summary>
    /// Writes a preference to both the shared MindAttic credential store (for mapped
    /// LLM keys) and the local JSON mirror so legacy readers stay consistent.
    /// </summary>
    public async Task SetAsync(string key, string value)
    {
        await @lock.WaitAsync();
        try
        {
            if (LlmKeyMap.TryGetValue(key, out var map))
            {
                WriteToStore(map.Provider, map.IsApiKey, value);
            }

            // Always mirror to local JSON so legacy readers still see consistent state.
            store[key] = value;
            await SaveAsync();
        }
        finally
        {
            @lock.Release();
        }
    }

    /// <summary>
    /// Removes a preference. For mapped LLM keys this also clears the corresponding
    /// field in the shared credential store.
    /// </summary>
    public void Remove(string key)
    {
        @lock.Wait();
        try
        {
            if (LlmKeyMap.TryGetValue(key, out var map))
            {
                WriteToStore(map.Provider, map.IsApiKey, "");
            }

            store.Remove(key);
            SaveSync();
        }
        finally
        {
            @lock.Release();
        }
    }

    private string? ReadFromStore(string provider, bool isApiKey)
    {
        try
        {
            var value = isApiKey ? credentialStore.GetApiKey(provider) : credentialStore.GetModel(provider);
            return string.IsNullOrEmpty(value) ? null : value;
        }
        catch
        {
            return null;
        }
    }

    private void WriteToStore(string provider, bool isApiKey, string value)
    {
        try
        {
            if (isApiKey) credentialStore.SetApiKey(provider, value);
            else          credentialStore.SetModel(provider, value);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[BlazorSecurePreferences] Failed to write shared LLM credential for '{provider}': {ex.Message}");
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
