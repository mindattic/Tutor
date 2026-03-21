using Microsoft.Maui.Storage;
using Tutor.Core.Services.Abstractions;

namespace Tutor.MAUI.Services;

public class MauiSecurePreferences : ISecurePreferences
{
    public Task<string?> GetAsync(string key) => SecureStorage.GetAsync(key);

    public Task SetAsync(string key, string value) => SecureStorage.SetAsync(key, value);

    public void Remove(string key) => SecureStorage.Remove(key);
}
