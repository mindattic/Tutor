using System.Text.RegularExpressions;

namespace Tutor.Cli.Gutenberg;

/// <summary>
/// Downloads the canonical UTF-8 plaintext for a Project Gutenberg book and strips
/// the boilerplate header/footer so the body is ready for the import pipeline.
/// </summary>
public sealed class GutenbergFetcher
{
    private readonly HttpClient http;

    /// <summary>
    /// Sets a polite identifying User-Agent if the supplied <see cref="HttpClient"/>
    /// doesn't already have one configured.
    /// </summary>
    public GutenbergFetcher(HttpClient http)
    {
        this.http = http;
        if (this.http.DefaultRequestHeaders.UserAgent.Count == 0)
            this.http.DefaultRequestHeaders.UserAgent.ParseAdd("MindAttic-Tutor-CLI/1.0 (+https://mindattic.com)");
    }

    /// <summary>
    /// Downloads book <paramref name="bookId"/>'s canonical UTF-8 plaintext from
    /// gutenberg.org and returns the body with PG header/footer markers removed.
    /// </summary>
    public async Task<string> FetchPlainTextAsync(int bookId, CancellationToken ct = default)
    {
        // Project Gutenberg's canonical UTF-8 plaintext URL.
        var url = $"https://www.gutenberg.org/cache/epub/{bookId}/pg{bookId}.txt";
        var raw = await http.GetStringAsync(url, ct);
        return StripHeaderFooter(raw);
    }

    private static readonly Regex StartMarker = new(
        @"^\*{3}\s*START OF (?:THE|THIS) PROJECT GUTENBERG.*$",
        RegexOptions.IgnoreCase | RegexOptions.Multiline);

    private static readonly Regex EndMarker = new(
        @"^\*{3}\s*END OF (?:THE|THIS) PROJECT GUTENBERG.*$",
        RegexOptions.IgnoreCase | RegexOptions.Multiline);

    /// <summary>
    /// Trims the Project Gutenberg "*** START OF... ***" / "*** END OF... ***"
    /// preamble and license boilerplate from a raw download. Falls back to a plain
    /// trim when the markers are missing.
    /// </summary>
    public static string StripHeaderFooter(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return raw;

        var startMatch = StartMarker.Match(raw);
        var endMatch   = EndMarker.Match(raw);

        var bodyStart = startMatch.Success
            ? startMatch.Index + startMatch.Length
            : 0;
        var bodyEnd = endMatch.Success
            ? endMatch.Index
            : raw.Length;

        if (bodyEnd <= bodyStart) return raw.Trim();

        return raw[bodyStart..bodyEnd].Trim();
    }
}
