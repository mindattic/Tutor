using Tutor.Core.Models;

namespace Tutor.Tests.Models;

/// <summary>
/// TUT-T62: placeholder follow-up suggestions are not rendered, and a restored lesson never
/// shows the internal teaching prompt as the student's message.
/// </summary>
public class ChatRestoreAndSuggestionTests
{
    [TestCase("First specific question about All Concepts?")]
    [TestCase("Second specific question?")]
    [TestCase("third question about Alice?")]
    [TestCase("  ")]
    public void IsPlaceholder_True_ForTemplateText(string text) =>
        Assert.That(SuggestedQuestions.IsPlaceholder(text), Is.True);

    [TestCase("Why does Alice follow the White Rabbit?")]
    [TestCase("What was the first question the Caterpillar asked?")]
    public void IsPlaceholder_False_ForRealQuestions(string text) =>
        Assert.That(SuggestedQuestions.IsPlaceholder(text), Is.False);

    [Test]
    public void DeriveDisplayText_HidesSpiralTeachingPrompt()
    {
        var prompt = "Teach me about \"Down the Rabbit-Hole\" from the curriculum.\n\nIMPORTANT CONTEXT - SPIRAL LEARNING (depth level 2):\n...";
        Assert.That(ChatMessageState.DeriveDisplayText("user", prompt),
            Is.EqualTo("Teach me about \"Down the Rabbit-Hole\""));
    }

    [Test]
    public void DeriveDisplayText_LeavesTypedMessagesAndAssistantRepliesAlone()
    {
        Assert.That(ChatMessageState.DeriveDisplayText("user", "Who is the Cheshire Cat?"), Is.Null);
        Assert.That(ChatMessageState.DeriveDisplayText("assistant", "Teach me about \"X\"\nmore"), Is.Null);
    }
}
