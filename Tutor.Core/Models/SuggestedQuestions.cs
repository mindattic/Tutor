using System.Text.RegularExpressions;

namespace Tutor.Core.Models;

/// <summary>
/// Follow-up questions the tutor suggests at the end of a reply (rendered as clickable links).
/// </summary>
public static class SuggestedQuestions
{
    // "First specific question about All Concepts?", "Second question?", "Third follow-up question" -
    // the template wording an LLM echoes back instead of writing a real question (TUT-T62).
    private static readonly Regex Placeholder = new(
        @"^\s*(first|second|third|fourth|fifth|1st|2nd|3rd)\s+((specific|natural|follow-up|follow up)\s+)*question\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    /// <summary>True when the suggestion is template text rather than a real question.</summary>
    public static bool IsPlaceholder(string? question) =>
        string.IsNullOrWhiteSpace(question) || Placeholder.IsMatch(question);
}
