using Microsoft.Maui.Storage;
using Tutor.Core.Services.Abstractions;

namespace Tutor.MAUI.Services;

/// <summary>
/// MAUI platform implementation of <see cref="ISecurePreferences"/> using
/// <see cref="SecureStorage"/>. Provides encrypted key-value storage backed by
/// the platform's secure enclave (Keychain on iOS/macOS, KeyStore on Android,
/// DPAPI on Windows) for storing sensitive data like API keys.
/// </summary>
public class MauiSecurePreferences : ISecurePreferences
{
    /// <inheritdoc/>
    public Task<string?> GetAsync(string key) => SecureStorage.GetAsync(key);

    /// <inheritdoc/>
    public Task SetAsync(string key, string value) => SecureStorage.SetAsync(key, value);

    /// <inheritdoc/>
    public void Remove(string key) => SecureStorage.Remove(key);
}
