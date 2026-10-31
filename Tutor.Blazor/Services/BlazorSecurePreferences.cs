using System.Text.Json;
using MindAttic.Vault.Credentials;
using Tutor.Core.Services.Abstractions;

namespace Tutor.Blazor.Services;

/// <summary>
/// Tutor's per-user / content preferences file (<c>%LocalAppData%\Tutor\Settings\secure-preferences.json</c>:
/// the signed-in user, course lists, lesson state), shared by Tutor.Blazor and Tutor.Cli. App-level settings
/// live in <see cref="Tutor.Core.Services.TutorSettings"/> and API keys in Vault's shared named lists (LAMP,
/// TUT-T31), so neither is read or written here; the old settings entries in this file are left in place
/// as the rollback. The <c>*_MODEL</c> names still read the shared provider record's optional <c>model</c> field.
/// </summary>
public class BlazorSecurePreferences : ISecurePreferences, IDisposable
{
    private readonly string filePath;
    private readonly LlmCredentialResolver vault;
    private Dictionary<string, string> store = new();
    private readonly SemaphoreSlim @lock = new(1, 1);

    // Model-name preferences read (never written) from the shared provider record's "model" field.
    private static readonly Dictionary<string, string> ModelKeyMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["CHATGPT_MODEL"]  = "openai",
        ["CLAUDE_MODEL"]   = "claude",
        ["GEMINI_MODEL"]   = "gemini",
        ["DEEPSEEK_MODEL"] = "deepseek",
    };

    /// <summary>Loads local (non-secret) preferences; LLM keys are read from Vault on demand.</summary>
    public BlazorSecurePreferences(LlmCredentialResolver vault)
    {
        this.vault = vault;
        var dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Tutor", "Settings");
        Directory.CreateDirectory(dir);
        filePath = Path.Combine(dir, "secure-preferences.json");
        Load();
    }

    /// <summary>Reads a preference. Model names resolve from the shared provider record; everything else from local prefs.</summary>
    public Task<string?> GetAsync(string key)
    {
        if (ModelKeyMap.TryGetValue(key, out var provider))
            return Task.FromResult(ReadModel(provider));

        store.TryGetValue(key, out var value);
        return Task.FromResult<string?>(value);
    }

    /// <summary>Writes a preference. A model-name field is Vault-managed/read-only and ignored.</summary>
    public async Task SetAsync(string key, string value)
    {
        if (ModelKeyMap.ContainsKey(key)) return;

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

    /// <summary>Removes a preference. A model-name field is Vault-managed/read-only and ignored.</summary>
    public void Remove(string key)
    {
        if (ModelKeyMap.ContainsKey(key)) return;

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

    // The shared provider's optional "model" field from the raw record (unchanged by LAMP).
    private string? ReadModel(string provider)
    {
        try
        {
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
