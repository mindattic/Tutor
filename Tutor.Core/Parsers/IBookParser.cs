namespace Tutor.Core.Parsers;

// Implementations are stateless and registered as singletons in DI. Extensions
// are matched case-insensitively, with or without the leading dot.
public interface IBookParser
{
    IReadOnlyCollection<string> SupportedExtensions { get; }
    Task<ExtractedBook> ParseAsync(Stream input, string fileName, CancellationToken ct = default);
}
