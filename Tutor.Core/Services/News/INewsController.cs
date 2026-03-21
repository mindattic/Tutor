using Tutor.Core.Models;

namespace Tutor.Core.Services;

/// <summary>
/// Interface for news operations.
/// Designed for future API integration with various news providers.
/// </summary>
public interface INewsController
{
    /// <summary>Fetch latest headlines</summary>
    Task<NewsFeed> GetHeadlinesAsync(int count = 5);
    
    /// <summary>Fetch headlines by category</summary>
    Task<NewsFeed> GetHeadlinesByCategoryAsync(string category, int count = 5);
}
