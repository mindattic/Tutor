namespace Tutor.Core.Models;

public record ChatMessage(string Role, string Text, string FullJson, string? DisplayText = null, bool IsExploration = false)
{
    /// <summary>
    /// Gets the text to display in the chat UI.
    /// Returns DisplayText if set, otherwise returns Text.
    /// </summary>
    public string VisibleText => DisplayText ?? Text;
}
