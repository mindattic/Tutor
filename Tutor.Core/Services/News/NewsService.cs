using Tutor.Core.Models;

namespace Tutor.Core.Services;

/// <summary>
/// Service for fetching and caching news headlines.
/// </summary>
public class NewsService
{
    private readonly INewsController newsController;
    private NewsFeed? cachedFeed;
    private DateTime lastFetch = DateTime.MinValue;
    private readonly TimeSpan cacheExpiry = TimeSpan.FromMinutes(15);

    public NewsService(INewsController newsController)
    {
        this.newsController = newsController;
    }

    /// <summary>
    /// Get headlines, using cache if available and not expired.
    /// </summary>
    public async Task<NewsFeed> GetHeadlinesAsync(int count = 5, bool forceRefresh = false)
    {
        if (!forceRefresh && cachedFeed != null && DateTime.UtcNow - lastFetch < cacheExpiry)
        {
            return cachedFeed;
        }

        cachedFeed = await newsController.GetHeadlinesAsync(count);
        lastFetch = DateTime.UtcNow;
        return cachedFeed;
    }

    /// <summary>
    /// Get headlines by category.
    /// </summary>
    public async Task<NewsFeed> GetHeadlinesByCategoryAsync(string category, int count = 5)
    {
        return await newsController.GetHeadlinesByCategoryAsync(category, count);
    }

    /// <summary>
    /// Clear cached headlines.
    /// </summary>
    public void ClearCache()
    {
        cachedFeed = null;
        lastFetch = DateTime.MinValue;
    }
}
