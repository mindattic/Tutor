using MindAttic.Vault.Credentials;

namespace Tutor.Core.Services;

/// <summary>
/// How every Tutor LLM call gets its key (LAMP, TUT-T31): Vault's shared named key lists through
/// <see cref="KeySelector"/>, in Tutor's assigned order (or the shared order when Tutor has none),
/// failing over to the next key when one is rejected (401/403/quota/429) and flagging the bad key in
/// MindAttic.Vault.Desktop. Tutor never holds or edits a key itself.
/// </summary>
public sealed class TutorKeys
{
    private readonly KeySelector selector;

    public TutorKeys(KeySelector selector) => this.selector = selector ?? throw new ArgumentNullException(nameof(selector));

    /// <summary>The local key files and health for the current user (Tutor.Cli, Tutor.Mcp, tests).</summary>
    public static TutorKeys ForCurrentUser() => new(KeySelector.ForCurrentUser(ClassifyLegionFailure));

    /// <summary>The selector in use.</summary>
    public KeySelector Selector => selector;

    /// <summary>True when Tutor has at least one usable key for <paramref name="providerId"/>.</summary>
    public bool HasKey(string providerId) => selector.GetActiveKey(providerId, TutorSettings.AppId) is not null;

    /// <summary>Names (never values) of the keys Tutor would try for <paramref name="providerId"/>, in order.</summary>
    public IReadOnlyList<string> KeyNames(string providerId) =>
        selector.Plan(providerId, TutorSettings.AppId).Usable.Select(k => k.Name).ToList();

    /// <summary>
    /// Runs <paramref name="call"/> with each of Tutor's keys until one works. When none works (or none is
    /// configured) throws <see cref="InvalidOperationException"/> carrying the
    /// <see cref="AllKeysFailedException"/> message, which names keys and failure kinds, never values.
    /// </summary>
    public async Task<T> ExecuteAsync<T>(string providerId, Func<string, CancellationToken, Task<T>> call, CancellationToken ct = default)
    {
        try
        {
            return await selector.ExecuteAsync(providerId, TutorSettings.AppId, call, ct).ConfigureAwait(false);
        }
        catch (AllKeysFailedException ex)
        {
            var message = ex.NoKeys
                ? $"No {providerId} API key is available to Tutor. Add one in MindAttic Vault (Keys tab, or Tutor's tab)."
                : ex.Message + " Fix or replace the key in MindAttic Vault.";
            throw new InvalidOperationException(message, ex);
        }
    }

    /// <summary>
    /// Legion wraps provider HTTP failures (retries, circuit breaker); a 401/403/402/429 anywhere in the
    /// chain still means the key was refused, so the next key is tried.
    /// </summary>
    public static KeyFailureException? ClassifyLegionFailure(Exception ex)
    {
        for (var e = ex; e is not null; e = e.InnerException)
        {
            if (e is HttpRequestException { StatusCode: { } status } && KeySelector.Classify(status) is { } kind)
                return new KeyFailureException(kind, $"HTTP {(int)status}");
        }
        return null;
    }
}
