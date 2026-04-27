using System.Text.RegularExpressions;

namespace Tutor.Cli.Gutenberg;

public sealed class GutenbergFetcher
{
    private readonly HttpClient http;

    public GutenbergFetcher(HttpClient http)
    {
        this.http = http;
        if (this.http.DefaultRequestHeaders.UserAgent.Count == 0)
            this.http.DefaultRequestHeaders.UserAgent.ParseAdd("MindAttic-Tutor-CLI/1.0 (+https://mindattic.com)");
    }

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
