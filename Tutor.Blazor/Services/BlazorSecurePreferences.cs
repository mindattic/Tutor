using System.Text.Json;
using Tutor.Core.Services.Abstractions;

namespace Tutor.Blazor.Services;

public class BlazorSecurePreferences : ISecurePreferences
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
            _ = SaveAsync();
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
        catch
        {
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
        catch
        {
            // Silently fail on save errors
        }
    }
}
