using Tutor.Core.Models;

namespace Tutor.Tests.Models;

public class NewsTests
{
    [Fact]
    public void NewsHeadline_Defaults()
    {
        var h = new NewsHeadline();
        Assert.True(Guid.TryParse(h.Id, out _));
        Assert.Equal("", h.Title);
        Assert.Null(h.Description);
        Assert.Null(h.Url);
        Assert.Null(h.PublishedAt);
    }

    [Fact]
    public void NewsFeed_Defaults()
    {
        var f = new NewsFeed();
        Assert.NotNull(f.Headlines);
        Assert.Empty(f.Headlines);
        Assert.Null(f.Error);
        Assert.True(f.FetchedAt > DateTime.MinValue);
    }
}
