using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using Tutor.Core.Services;

namespace Tutor.Cli.Gutenberg;

/// <summary>
/// Downloads the canonical UTF-8 plaintext for a Project Gutenberg book and strips
/// the boilerplate header/footer so the body is ready for the import pipeline. Also reads the
/// site's "Top 100 EBooks last 30 days" list (TUT-T50). Addresses, the pause between requests and
/// the User-Agent are Tutor settings (the "Gutenberg import" group).
/// </summary>
public sealed class GutenbergFetcher
{
    private readonly HttpClient http;
    private readonly TutorSettingsSource settings;

    public GutenbergFetcher(HttpClient http, TutorSettingsSource settings)
    {
        this.http = http;
        this.settings = settings;
        if (this.http.DefaultRequestHeaders.UserAgent.Count == 0)
            this.http.DefaultRequestHeaders.UserAgent.ParseAdd(settings.Current.GutenbergUserAgent);
    }

    /// <summary>
    /// Downloads book <paramref name="bookId"/>'s canonical UTF-8 plaintext from
    /// gutenberg.org and returns the body with PG header/footer markers removed.
    /// </summary>
    public async Task<string> FetchPlainTextAsync(int bookId, CancellationToken ct = default)
        => StripHeaderFooter((await FetchRawAsync(bookId, ct)).Raw);

    /// <summary>
    /// Downloads the book's plain text exactly as served (license header and footer included), from the
    /// book text address or, when that fails, the mirror. Returns the text and the address it came from.
    /// </summary>
    public async Task<(string Raw, string Url)> FetchRawAsync(int bookId, CancellationToken ct = default)
    {
        var s = settings.Current;
        var urls = new[] { s.GutenbergTextUrlTemplate, s.GutenbergMirrorTextUrlTemplate }
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Select(t => t.Replace("{id}", bookId.ToString(), StringComparison.OrdinalIgnoreCase))
            .ToList();

        Exception? last = null;
        foreach (var url in urls)
        {
            try
            {
                return (await GetStringPolitelyAsync(url, s.GutenbergRequestDelayMs, ct), url);
            }
            catch (HttpRequestException ex)
            {
                Console.Error.WriteLine($"  [GUTENBERG] {url} failed: {Describe(ex)}");
                last = ex;
            }
        }
        throw new InvalidOperationException($"Could not download Project Gutenberg #{bookId}: {(last is null ? "no address is configured" : Describe(last))}", last);
    }

    /// <summary>
    /// The first <paramref name="count"/> distinct books of the "Top 100 EBooks last 30 days" list, most
    /// downloaded first. When the page can't be read, the fallback list (Gutendex, ordered by the same
    /// 30-day download count) is used instead; <c>Source</c> says which one answered.
    /// </summary>
    public async Task<(IReadOnlyList<GutenbergBook> Books, string Source)> FetchTopAsync(int count, CancellationToken ct = default)
    {
        var s = settings.Current;
        try
        {
            var html = await GetStringPolitelyAsync(s.GutenbergTopListUrl, s.GutenbergRequestDelayMs, ct);
            var books = ParseTopPage(html).Take(count).ToList();
            if (books.Count > 0) return (books, s.GutenbergTopListUrl);
            Console.Error.WriteLine($"  [GUTENBERG] {s.GutenbergTopListUrl} had no \"last 30 days\" list.");
        }
        catch (HttpRequestException ex)
        {
            Console.Error.WriteLine($"  [GUTENBERG] {s.GutenbergTopListUrl} failed: {Describe(ex)}");
        }

        if (string.IsNullOrWhiteSpace(s.GutenbergPopularFallbackUrl))
            throw new InvalidOperationException("The Project Gutenberg top-books page could not be read and no fallback list is configured.");

        var json = await GetStringPolitelyAsync(s.GutenbergPopularFallbackUrl, s.GutenbergRequestDelayMs, ct);
        var fallback = ParsePopularJson(json).Take(count).ToList();
        if (fallback.Count == 0)
            throw new InvalidOperationException($"{s.GutenbergPopularFallbackUrl} returned no books.");
        return (fallback, s.GutenbergPopularFallbackUrl);
    }

    private async Task<string> GetStringPolitelyAsync(string url, int delayMs, CancellationToken ct)
    {
        if (delayMs > 0) await Task.Delay(delayMs, ct);
        return await http.GetStringAsync(url, ct);
    }

    private static string Describe(Exception ex) =>
        ex is HttpRequestException { StatusCode: { } code } ? $"HTTP {(int)code} {code}" : ex.Message;

    private static readonly Regex Last30Heading = new(@"id=""books-last30""", RegexOptions.IgnoreCase);
    private static readonly Regex TopEntry = new(
        @"<li>\s*<a href=""/ebooks/(?<id>\d+)"">(?<text>[^<]+)</a>", RegexOptions.IgnoreCase);
    private static readonly Regex TrailingCount = new(@"\s*\(\d+\)\s*$");

    /// <summary>
    /// Reads the "Top 100 EBooks last 30 days" list out of the top-books page. Each entry reads
    /// "Title by Author (downloads)"; a title with no " by " has no author.
    /// </summary>
    public static IEnumerable<GutenbergBook> ParseTopPage(string html)
    {
        var heading = Last30Heading.Match(html);
        if (!heading.Success) yield break;
        var end = html.IndexOf("</ol>", heading.Index, StringComparison.OrdinalIgnoreCase);
        var list = end > heading.Index ? html[heading.Index..end] : html[heading.Index..];

        var seen = new HashSet<int>();
        foreach (Match m in TopEntry.Matches(list))
        {
            var id = int.Parse(m.Groups["id"].Value);
            if (!seen.Add(id)) continue;
            var text = TrailingCount.Replace(WebUtility.HtmlDecode(m.Groups["text"].Value), "").Trim();
            var by = text.LastIndexOf(" by ", StringComparison.Ordinal);
            yield return by > 0
                ? new GutenbergBook(id, text[..by].Trim(), text[(by + 4)..].Trim())
                : new GutenbergBook(id, text, "");
        }
    }

    /// <summary>Reads a Gutendex <c>/books</c> page: id, title and authors ("Last, First" turned around).</summary>
    public static IEnumerable<GutenbergBook> ParsePopularJson(string json)
    {
        using var doc = JsonDocument.Parse(json);
        if (!doc.RootElement.TryGetProperty("results", out var results)) yield break;
        var seen = new HashSet<int>();
        foreach (var book in results.EnumerateArray())
        {
            var id = book.GetProperty("id").GetInt32();
            if (!seen.Add(id)) continue;
            var authors = book.TryGetProperty("authors", out var a)
                ? a.EnumerateArray().Select(x => FirstLast(x.GetProperty("name").GetString() ?? "")).Where(n => n.Length > 0)
                : [];
            yield return new GutenbergBook(id, (book.GetProperty("title").GetString() ?? $"Project Gutenberg #{id}").Trim(),
                string.Join(", ", authors));
        }
    }

    // "Austen, Jane" -> "Jane Austen"; "Montgomery, L. M. (Lucy Maud)" -> "L. M. Montgomery".
    private static string FirstLast(string name)
    {
        name = Regex.Replace(name, @"\s*\([^)]*\)", "").Trim();
        var comma = name.IndexOf(',');
        return comma < 0 ? name : $"{name[(comma + 1)..].Trim()} {name[..comma].Trim()}".Trim();
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
