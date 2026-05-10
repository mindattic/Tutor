using System.Text.Json;
using Tutor.Core.Models;

namespace Tutor.Tests.Models;

public class LlmCredentialTests
{
    [Test]
    public void Defaults_AreEmpty()
    {
        var cred = new LlmCredential();
        Assert.That(cred.ApiKey, Is.EqualTo(""));
        Assert.That(cred.Model, Is.EqualTo(""));
        Assert.That(cred.Extra, Is.Not.Null);
        Assert.That(cred.Extra, Is.Empty);
    }

    [Test]
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

        Assert.That(restored, Is.Not.Null);
        Assert.That(restored!.ApiKey, Is.EqualTo("sk-test"));
        Assert.That(restored.Model, Is.EqualTo("test-model"));
        Assert.That(restored.Extra["region"], Is.EqualTo("us-east-1"));
        Assert.That(restored.Extra["voice"], Is.EqualTo("alloy"));
    }
}
