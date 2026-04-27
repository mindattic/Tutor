namespace Tutor.Cli.Gutenberg;

// Curated top-10 Project Gutenberg works queued for batch import.
// First milestone is Moby Dick (id 2701) end-to-end. Once that flow is proven,
// `tutor gutenberg-top10` (TODO) should iterate this list with throttling
// between books to keep API spend predictable.
public static class GutenbergCatalog
{
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

public sealed record GutenbergBook(int Id, string Title, string Author);
