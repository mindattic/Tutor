# Tutor.Ingestion

Turns documents into text and searchable chunks. Extracted from Tutor (TUT-T64) so Tutor and Gadfly share one parse / chunk / embed pipeline. The package has no dependency on any app, UI, authentication or storage path: the host injects the folders (`IIngestionPaths`), the embedding provider (`IEmbeddingProvider`) and the log sink (`IngestionLog.Sink`).

- Target: `net10.0`. Version: whole numbers only (`1.0.0`).
- Pure-managed parsers (PdfPig, VersOne.Epub, DocumentFormat.OpenXml, AngleSharp, SmartReader) plus Tesseract OCR for scanned PDFs. `eng.traineddata` ships in the package and is copied to the application's `tessdata/` folder by `buildTransitive/Tutor.Ingestion.targets` (set `CopyTutorIngestionTessdata` to `false` to opt out).

## Public API

Namespace `Tutor.Ingestion.Parsers`

| Type | Purpose |
|---|---|
| `IBookParser` | `SupportedExtensions`, `ParseAsync(Stream, fileName, ct)` returning an `ExtractedBook`. |
| `ParserRegistry` | Routes a path or stream to the parser for its extension (`CanHandle`, `Resolve`, `ParseAsync(path)`). |
| `ExtractedBook` | `PlainText`, `Title`, `Author`, `Description`, `SourceFormat`, `Warnings`, and `Sections`. |
| `ExtractedSection` | `Label` ("Page 3", "Chapter 2: Setup", a heading), `Text`, `Page` (PDF only). |
| `ParserLimits` | Ceilings for untrusted input: `MaxPages`, `MaxChars`, `MaxUncompressedBytes` (zip-bomb guard). Unlimited by default. |
| `DocumentLimitException` | Thrown when a document is over a `ParserLimits` ceiling; the message is safe to show a user. |
| `PdfBookParser` | `.pdf` via PdfPig with OCR fallback for text-light pages; sections are pages. |
| `DocxBookParser` | `.docx`; sections start at Title / Heading paragraphs. |
| `EpubBookParser` | `.epub`; sections are reading-order chapters. |
| `HtmlBookParser` | `.html .htm .xhtml`; SmartReader with AngleSharp fallback; sections start at h1-h6. |
| `TxtBookParser` | `.txt .md .markdown .text .log`. |
| `MobiBookParser`, `LibreOfficeBookParser`, `ExternalToolBookParser` | Shell out to Calibre `ebook-convert` / LibreOffice `soffice` when installed (not used by Gadfly). |

Malformed input throws from `ParseAsync` (`InvalidDataException`, `FileFormatException` and similar); callers catch and report.

Namespace `Tutor.Ingestion.Ocr`: `IPdfOcrService` (`IsAvailable`, `OcrImageAsync`), `TesseractPdfOcrService(IIngestionPaths?)`, `NoOpPdfOcrService`.

Namespace `Tutor.Ingestion`: `ChunkingService.ChunkContent(string)` (about 500-token chunks on paragraph and sentence boundaries, with overlap); `IIngestionPaths` and `DefaultIngestionPaths(rootDirectory?)`; `IngestionLog.Sink`.

Namespace `Tutor.Ingestion.Embedding`: `IEmbeddingProvider` (`GetEmbeddingAsync`, `GetEmbeddingsAsync`), `EmbeddingMath.CosineSimilarity`.

Namespace `Tutor.Ingestion.Storage`: `ContentChunk`, `IVectorStore`, `VectorStoreService` (JSON-file store with LSH candidate generation and combined cosine / semantic / lexical scoring), `LSHService`, `SimHashService`.

## Use

```csharp
IPdfOcrService ocr = new NoOpPdfOcrService();            // or new TesseractPdfOcrService()
var limits = new ParserLimits(MaxPages: 500, MaxChars: 5_000_000, MaxUncompressedBytes: 200L << 20);
var registry = new ParserRegistry([new PdfBookParser(ocr, limits), new DocxBookParser(limits),
                                   new EpubBookParser(limits), new HtmlBookParser(limits), new TxtBookParser()]);
var book = await registry.ParseAsync(@"C:\docs\lecture.pdf");
foreach (var s in book.Sections) Console.WriteLine($"{s.Label}: {s.Text.Length} chars");
var chunks = new ChunkingService().ChunkContent(book.PlainText);
```
