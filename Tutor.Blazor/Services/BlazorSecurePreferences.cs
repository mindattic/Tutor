using System.Text.Json;
using Tutor.Core.Services.Abstractions;

namespace Tutor.Blazor.Services;

public class BlazorSecurePreferences : ISecurePreferences, IDisposable
{
    private readonly string filePath;
    private Dictionary<string, string> store = new();
    private readonly SemaphoreSlim @lock = new(1, 1);

    public BlazorSecurePreferences()
    {
        var dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Tutor", "Settings");
        Directory.CreateDirectory(dir);
        filePath = Path.Combine(dir, "secure-preferences.json");
        Load();
    }

    public Task<string?> GetAsync(string key)
    {
        store.TryGetValue(key, out var value);
        return Task.FromResult(value);
    }

    public async Task SetAsync(string key, string value)
    {
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

    public void Remove(string key)
    {
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

    public void Dispose()
    {
        @lock.Dispose();
    }
}
