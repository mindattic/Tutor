namespace Tutor.Core.Services.Abstractions;

/// <summary>
/// Tutor's per-user / content preferences (the signed-in user, course lists, lesson state). App-level
/// settings are <see cref="TutorSettings"/> and API keys come from Vault through <see cref="TutorKeys"/>
/// (LAMP, TUT-T31); neither goes through this interface.
/// </summary>
public interface ISecurePreferences
{
    Task<string?> GetAsync(string key);
    Task SetAsync(string key, string value);
    void Remove(string key);
}
