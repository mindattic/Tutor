using Tutor.Core.Models;

namespace Tutor.Tests.Models;

public class NewsTests
{
    [Test]
    public void NewsHeadline_Defaults()
    {
        var h = new NewsHeadline();
        Assert.That(Guid.TryParse(h.Id, out _), Is.True);
        Assert.That(h.Title, Is.EqualTo(""));
        Assert.That(h.Description, Is.Null);
        Assert.That(h.Url, Is.Null);
        Assert.That(h.PublishedAt, Is.Null);
    }

    [Test]
    public void NewsFeed_Defaults()
    {
        var f = new NewsFeed();
        Assert.That(f.Headlines, Is.Not.Null);
        Assert.That(f.Headlines, Is.Empty);
        Assert.That(f.Error, Is.Null);
        Assert.That(f.FetchedAt > DateTime.MinValue, Is.True);
    }
}
