using Tutor.Core.Models;

namespace Tutor.Tests.Models;

public class ChatMessageTests
{
    [Fact]
    public void VisibleText_ReturnsDisplayText_WhenSet()
    {
        var msg = new ChatMessage("user", "raw text", "{}", DisplayText: "pretty text");
        Assert.Equal("pretty text", msg.VisibleText);
    }

    [Fact]
    public void VisibleText_FallsBackToText_WhenDisplayTextNull()
    {
        var msg = new ChatMessage("assistant", "response text", "{}");
        Assert.Equal("response text", msg.VisibleText);
    }

    [Fact]
    public void Record_Equality_Works()
    {
        var a = new ChatMessage("user", "hello", "{}", null, false);
        var b = new ChatMessage("user", "hello", "{}", null, false);
        Assert.Equal(a, b);
    }

    [Fact]
    public void Record_Inequality_Works()
    {
        var a = new ChatMessage("user", "hello", "{}");
        var b = new ChatMessage("user", "goodbye", "{}");
        Assert.NotEqual(a, b);
    }

    [Fact]
    public void IsExploration_DefaultFalse()
    {
        var msg = new ChatMessage("user", "text", "{}");
        Assert.False(msg.IsExploration);
    }
}
