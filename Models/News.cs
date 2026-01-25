namespace Tutor.Models;

/// <summary>
/// A news headline for display on the home page.
/// </summary>
public class NewsHeadline
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public string? Url { get; set; }
    public string? Source { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime? PublishedAt { get; set; }
}

/// <summary>
/// Collection of news headlines.
/// </summary>
public class NewsFeed
{
    public List<NewsHeadline> Headlines { get; set; } = [];
    public DateTime FetchedAt { get; set; } = DateTime.UtcNow;
    public string? Error { get; set; }
}
