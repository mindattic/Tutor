namespace Tutor.Core.Services.Abstractions;

public interface ISecurePreferences
{
    Task<string?> GetAsync(string key);
    Task SetAsync(string key, string value);
    void Remove(string key);
}
