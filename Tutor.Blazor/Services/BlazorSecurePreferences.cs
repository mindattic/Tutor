using System.Text.Json;
using Tutor.Core.Services.Abstractions;

namespace Tutor.Blazor.Services;

public class BlazorSecurePreferences : ISecurePreferences, IDisposable
{
    private readonly string _filePath;
    private Dictionary<string, string> _store = new();
    private readonly SemaphoreSlim _lock = new(1, 1);

    public BlazorSecurePreferences()
    {
        var dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Tutor", "Settings");
        Directory.CreateDirectory(dir);
        _filePath = Path.Combine(dir, "secure-preferences.json");
        Load();
    }

    public Task<string?> GetAsync(string key)
    {
        _store.TryGetValue(key, out var value);
        return Task.FromResult(value);
    }

    public async Task SetAsync(string key, string value)
    {
        await _lock.WaitAsync();
        try
        {
            _store[key] = value;
            await SaveAsync();
        }
        finally
        {
            _lock.Release();
        }
    }

    public void Remove(string key)
    {
        _lock.Wait();
        try
        {
            _store.Remove(key);
            SaveSync();
        }
        finally
        {
            _lock.Release();
        }
    }

    private void Load()
    {
        try
        {
            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);
                _store = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[BlazorSecurePreferences] Failed to load preferences: {ex.Message}");
            _store = new();
        }
    }

    private async Task SaveAsync()
    {
        try
        {
            var json = JsonSerializer.Serialize(_store, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_filePath, json);
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
            var json = JsonSerializer.Serialize(_store, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[BlazorSecurePreferences] Failed to save preferences: {ex.Message}");
        }
    }

    public void Dispose()
    {
        _lock.Dispose();
    }
}
