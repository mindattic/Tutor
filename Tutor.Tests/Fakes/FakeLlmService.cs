using Tutor.Core.Models;
using Tutor.Core.Services.Abstractions;

namespace Tutor.Tests.Fakes;

/// <summary>
/// Fake LLM service that returns configurable canned responses.
/// </summary>
public class FakeLlmService : ILlmService
{
    public string ProviderName => "FakeProvider";
    public bool Configured { get; set; } = true;
    public string NextReply { get; set; } = "fake response";
    public int CallCount { get; private set; }
    public List<IEnumerable<ChatMessage>> ReceivedMessages { get; } = [];

    public Task<bool> IsConfiguredAsync() => Task.FromResult(Configured);

    public Task<ChatReply> GetReplyAsync(IEnumerable<ChatMessage> messages, string? instructions = null, CancellationToken ct = default)
    {
        CallCount++;
        ReceivedMessages.Add(messages);
        return Task.FromResult(new ChatReply(NextReply, $"{{\"text\":\"{NextReply}\"}}"));
    }
}
