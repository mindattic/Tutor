namespace Tutor.Cli.Gutenberg;

/// <summary>
/// Curated top-10 Project Gutenberg works queued for batch import. First milestone
/// is Moby Dick (id 2701) end-to-end. Once that flow is proven, the
/// <c>tutor gutenberg-top10</c> command iterates this list with throttling between
/// books to keep API spend predictable.
/// </summary>
public static class GutenbergCatalog
{
    /// <summary>The fixed list of books driven by <c>tutor gutenberg-top10</c>.</summary>
    public static readonly IReadOnlyList<GutenbergBook> Top10 = new[]
    {
        new GutenbergBook(2701, "Moby Dick; Or, The Whale", "Herman Melville"),
        new GutenbergBook(1342, "Pride and Prejudice",       "Jane Austen"),
        new GutenbergBook( 829, "Gulliver's Travels",         "Jonathan Swift"),
        new GutenbergBook(  84, "Frankenstein",               "Mary Wollstonecraft Shelley"),
        new GutenbergBook(1661, "The Adventures of Sherlock Holmes", "Arthur Conan Doyle"),
        new GutenbergBook(  11, "Alice's Adventures in Wonderland", "Lewis Carroll"),
        new GutenbergBook( 174, "The Picture of Dorian Gray", "Oscar Wilde"),
        new GutenbergBook(  74, "The Adventures of Tom Sawyer","Mark Twain"),
        new GutenbergBook( 120, "Treasure Island",            "Robert Louis Stevenson"),
        new GutenbergBook( 345, "Dracula",                    "Bram Stoker"),
    };
}

/// <summary>
/// One entry in <see cref="GutenbergCatalog.Top10"/>. <paramref name="Id"/> is the
/// Project Gutenberg numeric identifier used to fetch the canonical UTF-8 plaintext.
/// </summary>
public sealed record GutenbergBook(int Id, string Title, string Author);
