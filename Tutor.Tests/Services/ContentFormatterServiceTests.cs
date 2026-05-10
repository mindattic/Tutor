using Microsoft.Extensions.DependencyInjection;
using MindAttic.Legion;
using Tutor.Core.Services;
using Tutor.Core.Services.Abstractions;
using Tutor.Tests.Fakes;

namespace Tutor.Tests.Services;

public class ContentFormatterServiceTests
{
    private static ContentFormatterService CreateService()
    {
        // Build a router with no real LLM credentials. The tests below only
        // exercise paths that don't make a network call (QuickFormat is static,
        // SplitIntoChunks is pure, FormatContentAsync short-circuits on
        // null/empty input).
        var prefs = new FakeSecurePreferences();
        var services = new ServiceCollection();
        services.AddSingleton<ISecurePreferences>(prefs);
        services.AddSingleton(sp => new OpenAIOptions(sp.GetRequiredService<ISecurePreferences>()) { Model = "gpt-4.1-mini" });
        services.AddLegionClient();
        services.AddSingleton<OpenAIService>();
        services.AddSingleton<ClaudeService>();
        services.AddSingleton<DeepSeekService>();
        services.AddSingleton<GeminiService>();
        var sp = services.BuildServiceProvider();
        var router = new LlmServiceRouter(prefs, sp);
        return new ContentFormatterService(router);
    }

    [Test]
    public void QuickFormat_NullOrEmpty_ReturnsInput()
    {
        Assert.That(ContentFormatterService.QuickFormat(null!), Is.Null);
        Assert.That(ContentFormatterService.QuickFormat(""), Is.EqualTo(""));
        Assert.That(ContentFormatterService.QuickFormat("   "), Is.EqualTo("   "));
    }

    [Test]
    public void QuickFormat_DetectsAllCapsHeader()
    {
        var output = ContentFormatterService.QuickFormat("INTRODUCTION\n\nFirst paragraph.");
        Assert.That(output, Does.Contain("## Introduction"));
        Assert.That(output, Does.Contain("First paragraph."));
    }

    [Test]
    public void QuickFormat_DetectsColonHeader()
    {
        var output = ContentFormatterService.QuickFormat("Overview:\nSome body text.");
        Assert.That(output, Does.Contain("## Overview"));
    }

    [Test]
    public void QuickFormat_DetectsChapterPrefix()
    {
        var output = ContentFormatterService.QuickFormat("Chapter 1\nFirst sentence.");
        Assert.That(output, Does.Contain("## Chapter 1"));
    }

    [Test]
    public void QuickFormat_NumberedListBecomesBullets()
    {
        var output = ContentFormatterService.QuickFormat("1. First\n2. Second");
        Assert.That(output, Does.Contain("- First"));
        Assert.That(output, Does.Contain("- Second"));
    }

    [Test]
    public void QuickFormat_DashListPassthrough()
    {
        var output = ContentFormatterService.QuickFormat("- already bulleted");
        Assert.That(output, Does.Contain("- already bulleted"));
    }

    [Test]
    public void SplitIntoChunks_EmptyContent_ReturnsEmpty()
    {
        var sut = CreateService();
        Assert.That(sut.SplitIntoChunks(""), Is.Empty);
        Assert.That(sut.SplitIntoChunks("   "), Is.Empty);
    }

    [Test]
    public void SplitIntoChunks_SmallContent_ReturnsSingleChunk()
    {
        var sut = CreateService();
        var content = "short content under the threshold.";

        var chunks = sut.SplitIntoChunks(content, maxChunkSize: 100);

        Assert.That(chunks, Has.Count.EqualTo(1));
        Assert.That(chunks[0], Is.EqualTo(content));
    }

    [Test]
    public void SplitIntoChunks_LargeContent_SplitsByParagraphs()
    {
        var sut = CreateService();
        var paragraphs = Enumerable.Range(1, 10)
            .Select(i => $"Paragraph {i} " + new string('x', 200))
            .ToList();
        var content = string.Join("\n\n", paragraphs);

        var chunks = sut.SplitIntoChunks(content, maxChunkSize: 600);

        Assert.That(chunks.Count > 1, Is.True);
    }

    [Test]
    public async Task FormatContentAsync_BlankInput_ReturnedUnchanged()
    {
        var sut = CreateService();
        Assert.That(await sut.FormatContentAsync(""), Is.EqualTo(""));
        Assert.That(await sut.FormatContentAsync("   "), Is.EqualTo("   "));
    }
}
