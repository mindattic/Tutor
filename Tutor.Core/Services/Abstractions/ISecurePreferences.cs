namespace Tutor.Core.Services.Abstractions;

public interface ISecurePreferences
{
    Task<string?> GetAsync(string key);
    Task SetAsync(string key, string value);
    void Remove(string key);

    /// <summary>
    /// Every key currently configured for an LLM API-key preference (e.g. "CLAUDE_API_KEY"),
    /// in priority order — the key a caller should try first is index 0. Empty when none are
    /// configured; a single key set via <see cref="SetAsync"/> still yields a one-element list.
    /// </summary>
    /// <exception cref="NotSupportedException">Thrown when <paramref name="key"/> is not an LLM API-key preference.</exception>
    Task<IReadOnlyList<string>> GetApiKeysAsync(string key);

    /// <summary>
    /// Replaces the whole key pool for an LLM API-key preference. An empty list clears it
    /// (matching <see cref="Remove"/>'s behaviour).
    /// </summary>
    /// <exception cref="NotSupportedException">Thrown when <paramref name="key"/> is not an LLM API-key preference.</exception>
    Task SetApiKeysAsync(string key, IReadOnlyList<string> keys);
}
