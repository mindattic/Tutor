using System.Text.Json;
using Tutor.Core.Models;

namespace Tutor.Tests.Models;

public class LlmCredentialTests
{
    [Fact]
    public void Defaults_AreEmpty()
    {
        var cred = new LlmCredential();
        Assert.Equal("", cred.ApiKey);
        Assert.Equal("", cred.Model);
        Assert.NotNull(cred.Extra);
        Assert.Empty(cred.Extra);
    }

    [Fact]
    public void RoundTripsThroughJson()
    {
        var original = new LlmCredential
        {
            ApiKey = "sk-test",
            Model = "test-model",
            Extra = { ["region"] = "us-east-1", ["voice"] = "alloy" }
        };

        var json = JsonSerializer.Serialize(original);
        var restored = JsonSerializer.Deserialize<LlmCredential>(json);

        Assert.NotNull(restored);
        Assert.Equal("sk-test", restored!.ApiKey);
        Assert.Equal("test-model", restored.Model);
        Assert.Equal("us-east-1", restored.Extra["region"]);
        Assert.Equal("alloy", restored.Extra["voice"]);
    }
}
