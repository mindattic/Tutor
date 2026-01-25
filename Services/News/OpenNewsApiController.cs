using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tutor.Models;

namespace Tutor.Services;

/// <summary>
/// News controller using a free news API.
/// Currently uses MediaStack API (free tier) - can be swapped for any news API.
/// 
/// Note: For production, use proper API key management.
/// </summary>
public class OpenNewsApiController : INewsController
{
    private readonly HttpClient httpClient;
    
    // Using a sample free news API - can be replaced with any provider
    // For demo purposes, we'll use static placeholder headlines when API is unavailable
    private static readonly List<NewsHeadline> PlaceholderHeadlines =
    [
        new NewsHeadline
        {
            Id = "1",
            Title = "Welcome to Tutor - Your AI Learning Companion",
            Description = "Start your learning journey with personalized courses and AI-guided lessons.",
            Source = "Tutor App",
            PublishedAt = DateTime.UtcNow
        },
        new NewsHeadline
        {
            Id = "2",
            Title = "New Feature: Interactive Quizzes",
            Description = "Test your knowledge with quizzes generated from your course material.",
            Source = "Tutor App",
            PublishedAt = DateTime.UtcNow.AddHours(-2)
        },
        new NewsHeadline
        {
            Id = "3",
            Title = "Track Your Progress",
            Description = "Monitor your learning progress with detailed analytics and completion tracking.",
            Source = "Tutor App",
            PublishedAt = DateTime.UtcNow.AddHours(-5)
        },
        new NewsHeadline
        {
            Id = "4",
            Title = "Explore Concept Maps",
            Description = "Visualize relationships between concepts with interactive concept maps.",
            Source = "Tutor App",
            PublishedAt = DateTime.UtcNow.AddDays(-1)
        },
        new NewsHeadline
        {
            Id = "5",
            Title = "Learning Tip: Consistent Practice",
            Description = "Studies show that regular, shorter study sessions are more effective than cramming.",
            Source = "Tutor Tips",
            PublishedAt = DateTime.UtcNow.AddDays(-2)
        }
    ];

    public OpenNewsApiController(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<NewsFeed> GetHeadlinesAsync(int count = 5)
    {
        try
        {
            // Try to fetch from a free news API
            // Using NewsAPI.org as example - requires API key
            // For demo, we'll return placeholder headlines
            
            // Uncomment and add API key for real news:
            // var apiKey = await SecureStorage.GetAsync("NEWS_API_KEY");
            // if (!string.IsNullOrEmpty(apiKey))
            // {
            //     var response = await httpClient.GetAsync(
            //         $"https://newsapi.org/v2/top-headlines?country=us&pageSize={count}&apiKey={apiKey}");
            //     if (response.IsSuccessStatusCode)
            //     {
            //         var result = await response.Content.ReadFromJsonAsync<NewsApiResponse>();
            //         return MapToNewsFeed(result);
            //     }
            // }

            // Return placeholder headlines for demo
            await Task.Delay(100); // Simulate network delay
            return new NewsFeed
            {
                Headlines = PlaceholderHeadlines.Take(count).ToList(),
                FetchedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            return new NewsFeed
            {
                Headlines = PlaceholderHeadlines.Take(count).ToList(),
                FetchedAt = DateTime.UtcNow,
                Error = ex.Message
            };
        }
    }

    public async Task<NewsFeed> GetHeadlinesByCategoryAsync(string category, int count = 5)
    {
        // For now, just return general headlines
        // Can be extended to filter by category
        return await GetHeadlinesAsync(count);
    }

    // Helper classes for API response mapping (when using real API)
    private class NewsApiResponse
    {
        [JsonPropertyName("status")]
        public string? Status { get; set; }
        
        [JsonPropertyName("totalResults")]
        public int TotalResults { get; set; }
        
        [JsonPropertyName("articles")]
        public List<NewsApiArticle>? Articles { get; set; }
    }

    private class NewsApiArticle
    {
        [JsonPropertyName("title")]
        public string? Title { get; set; }
        
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        
        [JsonPropertyName("url")]
        public string? Url { get; set; }
        
        [JsonPropertyName("urlToImage")]
        public string? UrlToImage { get; set; }
        
        [JsonPropertyName("publishedAt")]
        public DateTime? PublishedAt { get; set; }
        
        [JsonPropertyName("source")]
        public NewsApiSource? Source { get; set; }
    }

    private class NewsApiSource
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }

    private static NewsFeed MapToNewsFeed(NewsApiResponse? response)
    {
        if (response?.Articles == null)
            return new NewsFeed { FetchedAt = DateTime.UtcNow };

        return new NewsFeed
        {
            Headlines = response.Articles.Select(a => new NewsHeadline
            {
                Id = Guid.NewGuid().ToString(),
                Title = a.Title ?? "",
                Description = a.Description,
                Url = a.Url,
                ImageUrl = a.UrlToImage,
                PublishedAt = a.PublishedAt,
                Source = a.Source?.Name
            }).ToList(),
            FetchedAt = DateTime.UtcNow
        };
    }
}
