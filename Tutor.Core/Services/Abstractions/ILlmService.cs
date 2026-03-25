using Tutor.Core.Models;

namespace Tutor.Core.Services.Abstractions;

/// <summary>
/// Common interface for LLM chat services.
/// </summary>
public interface ILlmService
{
    /// <summary>
    /// Checks if the API key is configured for this provider.
    /// </summary>
    Task<bool> IsConfiguredAsync();

    /// <summary>
    /// Get a chat reply from the LLM.
    /// </summary>
    Task<ChatReply> GetReplyAsync(IEnumerable<ChatMessage> messages, string? instructions = null, CancellationToken ct = default);

    /// <summary>
    /// The display name of this provider (e.g., "ChatGPT", "Claude").
    /// </summary>
    string ProviderName { get; }
}
