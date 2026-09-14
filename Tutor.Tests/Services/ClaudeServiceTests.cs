using System.Net;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using MindAttic.Legion;
using Tutor.Core.Models;
using Tutor.Core.Services;
using Tutor.Tests.Fakes;

namespace Tutor.Tests.Services;

/// <summary>
/// Above-the-wire tests for ClaudeService — wire transport is owned by Legion
/// and not exercised here. The shared Legion credential store is sandboxed so
/// IsConfiguredAsync can't accidentally pick up a real machine-local key.
/// </summary>
public class ClaudeServiceTests
{
    private string sandbox = string.Empty;
    private string? prevCredsEnv;
    private ServiceProvider sp = null!;
    private LegionClient legion = null!;

    [SetUp]
    public void SetUp()
    {
        sandbox = Path.Combine(Path.GetTempPath(), "tutor-claude-test-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(sandbox);
        prevCredsEnv = Environment.GetEnvironmentVariable("MINDATTIC_LLM_CREDENTIALS");
        Environment.SetEnvironmentVariable("MINDATTIC_LLM_CREDENTIALS", sandbox);

        var services = new ServiceCollection();
        services.AddLegionClient();
        sp = services.BuildServiceProvider();
        legion = sp.GetRequiredService<LegionClient>();
    }

    [TearDown]
    public void TearDown()
    {
        sp.Dispose();
        Environment.SetEnvironmentVariable("MINDATTIC_LLM_CREDENTIALS", prevCredsEnv);
        try { Directory.Delete(sandbox, recursive: true); } catch { }
    }

    [Test]
    public void ProviderName_IsClaude()
    {
        var prefs = new FakeSecurePreferences();
        var svc = new ClaudeService(legion, prefs);
        Assert.That(svc.ProviderName, Is.EqualTo("Claude"));
    }

    [Test]
    public async Task IsConfiguredAsync_NoKey_False()
    {
        var prefs = new FakeSecurePreferences();
        var svc = new ClaudeService(legion, prefs);
        Assert.That(await svc.IsConfiguredAsync(), Is.False);
    }

    [Test]
    public async Task IsConfiguredAsync_PrefsKey_True()
    {
        var prefs = new FakeSecurePreferences();
        await prefs.SetAsync("CLAUDE_API_KEY", "sk-ant-test");
        var svc = new ClaudeService(legion, prefs);

        Assert.That(await svc.IsConfiguredAsync(), Is.True);
    }

    /// <summary>
    /// Regression test for the "claude-api" -> "claude" provider-id rename.
    /// The stale id fell through Legion's dispatcher into the OpenAI-compatible
    /// branch, which has no endpoint registered for "claude-api" and throws
    /// ArgumentException("Unknown provider: claude-api") before any HTTP
    /// request is ever sent. Routing through a fake handler and asserting the
    /// outbound request actually reached Claude's endpoint with Claude's
    /// x-api-key auth header proves the provider id Legion received is really
    /// "claude" — this would have failed (no request captured, exception
    /// thrown instead) against the old "claude-api" code.
    /// </summary>
    [Test]
    public async Task GetReplyAsync_DispatchesToClaudeEndpoint_WithPlainClaudeProviderId()
    {
        var handler = new CapturingHandler(HttpStatusCode.OK, """{"content":[{"type":"text","text":"hello from claude"}]}""");
        var directLegion = new LegionClient(new HttpClient(handler), null);

        var prefs = new FakeSecurePreferences();
        await prefs.SetAsync("CLAUDE_API_KEY", "sk-ant-test-key");
        var svc = new ClaudeService(directLegion, prefs);

        var messages = new[] { new ChatMessage("user", "hi", "hi") };
        var reply = await svc.GetReplyAsync(messages);

        Assert.That(reply.Text, Is.EqualTo("hello from claude"));
        Assert.That(handler.CallCount, Is.EqualTo(1));
        Assert.That(handler.LastRequestUri, Is.EqualTo(new Uri("https://api.anthropic.com/v1/messages")));
        Assert.That(handler.LastAuthHeaderName, Is.EqualTo("x-api-key"));
        Assert.That(handler.LastAuthHeaderValue, Is.EqualTo("sk-ant-test-key"));
    }

    /// <summary>
    /// Captures the single outbound HTTP request (URI + auth header) and
    /// always returns the same status/body — enough to pin down which
    /// provider-specific wire shape LegionClient's dispatcher chose without
    /// hitting the network.
    /// </summary>
    private sealed class CapturingHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode statusCode;
        private readonly string body;

        public int CallCount { get; private set; }
        public Uri? LastRequestUri { get; private set; }
        public string? LastAuthHeaderName { get; private set; }
        public string? LastAuthHeaderValue { get; private set; }

        public CapturingHandler(HttpStatusCode statusCode, string body)
        {
            this.statusCode = statusCode;
            this.body = body;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            CallCount++;
            LastRequestUri = request.RequestUri;

            if (request.Headers.TryGetValues("x-api-key", out var values))
            {
                LastAuthHeaderName = "x-api-key";
                LastAuthHeaderValue = values.FirstOrDefault();
            }
            else if (request.Headers.Authorization is not null)
            {
                LastAuthHeaderName = "Authorization";
                LastAuthHeaderValue = request.Headers.Authorization.Parameter;
            }

            return Task.FromResult(new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            });
        }
    }
}
