using System.Text.Json;
using Tutor.Core.Services;
using Tutor.Core.Services.Abstractions;

namespace Tutor.Cli.Services;

// Mirrors Tutor.Blazor.Services.BlazorSecurePreferences — keeps the same on-disk
// path (%LocalAppData%/Tutor/Settings/secure-preferences.json) so courses created
// from the CLI are visible to the Blazor UI and vice versa. LLM credentials are
// routed through the shared LlmCredentialStore at %APPDATA%/MindAttic/LLM/.
public sealed class CliSecurePreferences : ISecurePreferences, IDisposable
{
    private readonly string filePath;
    private readonly LlmCredentialStore credentialStore;
    private Dictionary<string, string> store = new();
    private readonly SemaphoreSlim @lock = new(1, 1);

    private static readonly Dictionary<string, (string Provider, Field Field)> LlmKeyMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["OPENAI_API_KEY"]   = ("openai",   Field.ApiKey),
        ["CHATGPT_MODEL"]    = ("openai",   Field.Model),
        ["CLAUDE_API_KEY"]   = ("claude",   Field.ApiKey),
        ["CLAUDE_MODEL"]     = ("claude",   Field.Model),
        ["GEMINI_API_KEY"]   = ("gemini",   Field.ApiKey),
        ["GEMINI_MODEL"]     = ("gemini",   Field.Model),
        ["DEEPSEEK_API_KEY"] = ("deepseek", Field.ApiKey),
        ["DEEPSEEK_MODEL"]   = ("deepseek", Field.Model),
    };

    private enum Field { ApiKey, Model }

    public CliSecurePreferences(LlmCredentialStore credentialStore)
    {
        this.credentialStore = credentialStore;
        var dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Tutor", "Settings");
        Directory.CreateDirectory(dir);
        filePath = Path.Combine(dir, "secure-preferences.json");
        Load();
    }

    public async Task<string?> GetAsync(string key)
    {
        if (LlmKeyMap.TryGetValue(key, out var map))
        {
            var fromStore = ReadFromStore(map.Provider, map.Field);
            if (!string.IsNullOrEmpty(fromStore)) return fromStore;

            if (store.TryGetValue(key, out var legacy) && !string.IsNullOrEmpty(legacy))
            {
                await @lock.WaitAsync();
                try
                {
                    if (!credentialStore.Exists(map.Provider) || string.IsNullOrEmpty(ReadFromStore(map.Provider, map.Field)))
                        WriteToStore(map.Provider, map.Field, legacy);
                }
                finally { @lock.Release(); }
                return legacy;
            }
            return null;
        }

        store.TryGetValue(key, out var value);
        return value;
    }

    public async Task SetAsync(string key, string value)
    {
        await @lock.WaitAsync();
        try
        {
            if (LlmKeyMap.TryGetValue(key, out var map))
            {
                WriteToStore(map.Provider, map.Field, value);
            }

            store[key] = value;
            await SaveAsync();
        }
        finally
        {
            @lock.Release();
        }
    }

    public void Remove(string key)
    {
        @lock.Wait();
        try
        {
            if (LlmKeyMap.TryGetValue(key, out var map))
            {
                WriteToStore(map.Provider, map.Field, "");
            }

            store.Remove(key);
            SaveSync();
        }
        finally
        {
            @lock.Release();
        }
    }

    private string? ReadFromStore(string provider, Field field)
    {
        try
        {
            var value = field == Field.ApiKey
                ? credentialStore.GetApiKey(provider)
                : credentialStore.GetModel(provider);
            return string.IsNullOrEmpty(value) ? null : value;
        }
        catch
        {
            return null;
        }
    }

    private void WriteToStore(string provider, Field field, string value)
    {
        try
        {
            if (field == Field.ApiKey)
                credentialStore.SetApiKey(provider, value);
            else
                credentialStore.SetModel(provider, value);
        }
        catch
        {
            // Best-effort write to shared store
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

    public void Dispose() => @lock.Dispose();
}
