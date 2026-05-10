using Tutor.Core.Models;

namespace Tutor.Tests.Models;

public class ChatMessageTests
{
    [Test]
    public void VisibleText_ReturnsDisplayText_WhenSet()
    {
        var msg = new ChatMessage("user", "raw text", "{}", DisplayText: "pretty text");
        Assert.That(msg.VisibleText, Is.EqualTo("pretty text"));
    }

    [Test]
    public void VisibleText_FallsBackToText_WhenDisplayTextNull()
    {
        var msg = new ChatMessage("assistant", "response text", "{}");
        Assert.That(msg.VisibleText, Is.EqualTo("response text"));
    }

    [Test]
    public void Record_Equality_Works()
    {
        var a = new ChatMessage("user", "hello", "{}", null, false);
        var b = new ChatMessage("user", "hello", "{}", null, false);
        Assert.That(b, Is.EqualTo(a));
    }

    [Test]
    public void Record_Inequality_Works()
    {
        var a = new ChatMessage("user", "hello", "{}");
        var b = new ChatMessage("user", "goodbye", "{}");
        Assert.That(b, Is.Not.EqualTo(a));
    }

    [Test]
    public void IsExploration_DefaultFalse()
    {
        var msg = new ChatMessage("user", "text", "{}");
        Assert.That(msg.IsExploration, Is.False);
    }
}
