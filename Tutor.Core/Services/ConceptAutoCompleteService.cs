using Tutor.Core.Models;

namespace Tutor.Core.Services;

/// <summary>
/// Provides autocomplete functionality for concept searches.
/// Uses a "starts with" strategy first, falling back to "contains" if no results.
/// </summary>
public class ConceptAutoCompleteService
{
    /// <summary>
    /// Minimum characters required to trigger autocomplete.
    /// </summary>
    public const int MinQueryLength = 3;

    /// <summary>
    /// Default maximum number of suggestions to return.
    /// </summary>
    public const int DefaultMaxResults = 8;

    /// <summary>
    /// Gets autocomplete suggestions for the given query from a list of concepts.
    /// First attempts "starts with" matching, then falls back to "contains" if no results.
    /// </summary>
    /// <param name="concepts">The list of concepts to search.</param>
    /// <param name="query">The search query (minimum 3 characters).</param>
    /// <param name="maxResults">Maximum number of results to return.</param>
    /// <returns>A list of matching concepts, ordered by relevance.</returns>
    public List<ConceptSuggestion> GetSuggestions(
        IEnumerable<Concept> concepts, 
        string query, 
        int maxResults = DefaultMaxResults)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length < MinQueryLength)
            return [];

        var normalizedQuery = query.Trim().ToLowerInvariant();
        var conceptList = concepts.ToList();

        // First try "starts with" matching
        var startsWithResults = GetStartsWithMatches(conceptList, normalizedQuery, maxResults);
        
        if (startsWithResults.Count > 0)
            return startsWithResults;

        // Fall back to "contains" matching
        return GetContainsMatches(conceptList, normalizedQuery, maxResults);
    }

    /// <summary>
    /// Gets suggestions where the title or an alias starts with the query.
    /// </summary>
    private List<ConceptSuggestion> GetStartsWithMatches(
        List<Concept> concepts, 
        string normalizedQuery, 
        int maxResults)
    {
        return concepts
            .Select(c => new
            {
                Concept = c,
                Score = GetStartsWithScore(c, normalizedQuery)
            })
            .Where(x => x.Score > 0)
            .OrderByDescending(x => x.Score)
            .ThenBy(x => x.Concept.Title.Length) // Prefer shorter titles
            .Take(maxResults)
            .Select(x => new ConceptSuggestion
            {
                Id = x.Concept.Id,
                Title = x.Concept.Title,
                MatchType = x.Score >= 100 ? AutoCompleteMatchType.ExactMatch : AutoCompleteMatchType.StartsWith,
                Score = x.Score
            })
            .ToList();
    }

    /// <summary>
    /// Gets suggestions where the title or an alias contains the query.
    /// </summary>
    private List<ConceptSuggestion> GetContainsMatches(
        List<Concept> concepts, 
        string normalizedQuery, 
        int maxResults)
    {
        return concepts
            .Select(c => new
            {
                Concept = c,
                Score = GetContainsScore(c, normalizedQuery)
            })
            .Where(x => x.Score > 0)
            .OrderByDescending(x => x.Score)
            .ThenBy(x => x.Concept.Title.Length)
            .Take(maxResults)
            .Select(x => new ConceptSuggestion
            {
                Id = x.Concept.Id,
                Title = x.Concept.Title,
                MatchType = AutoCompleteMatchType.Contains,
                Score = x.Score
            })
            .ToList();
    }

    /// <summary>
    /// Scores a concept based on "starts with" matching.
    /// </summary>
    private static int GetStartsWithScore(Concept concept, string query)
    {
        var titleLower = concept.Title.ToLowerInvariant();

        // Exact title match (highest priority)
        if (titleLower == query)
            return 100;

        // Title starts with query
        if (titleLower.StartsWith(query))
            return 80;

        // Alias exact match
        if (concept.Aliases.Any(a => a.Equals(query, StringComparison.OrdinalIgnoreCase)))
            return 70;

        // Alias starts with query
        if (concept.Aliases.Any(a => a.StartsWith(query, StringComparison.OrdinalIgnoreCase)))
            return 60;

        return 0; // No "starts with" match
    }

    /// <summary>
    /// Scores a concept based on "contains" matching.
    /// </summary>
    private static int GetContainsScore(Concept concept, string query)
    {
        var titleLower = concept.Title.ToLowerInvariant();

        // Title contains query as a word boundary
        if (titleLower.Contains($" {query}") || titleLower.Contains($"{query} "))
            return 50;

        // Title contains query anywhere
        if (titleLower.Contains(query))
            return 40;

        // Alias contains query
        if (concept.Aliases.Any(a => a.Contains(query, StringComparison.OrdinalIgnoreCase)))
            return 30;

        return 0; // No match
    }

    /// <summary>
    /// Checks if the query meets the minimum length requirement.
    /// </summary>
    public static bool IsValidQuery(string? query)
    {
        return !string.IsNullOrWhiteSpace(query) && query.Trim().Length >= MinQueryLength;
    }
}

/// <summary>
/// Represents a concept suggestion for autocomplete.
/// </summary>
public class ConceptSuggestion
{
    /// <summary>
    /// The concept ID.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// The concept title to display.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// The type of match that was found.
    /// </summary>
    public AutoCompleteMatchType MatchType { get; set; }

    /// <summary>
    /// The match score (higher = better match).
    /// </summary>
    public int Score { get; set; }
}

/// <summary>
/// Indicates how the autocomplete match was found.
/// </summary>
public enum AutoCompleteMatchType
{
    /// <summary>
    /// Exact match on title or alias.
    /// </summary>
    ExactMatch,

    /// <summary>
    /// Title or alias starts with the query.
    /// </summary>
    StartsWith,

    /// <summary>
    /// Title or alias contains the query.
    /// </summary>
    Contains
}
