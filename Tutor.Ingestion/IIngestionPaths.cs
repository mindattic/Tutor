namespace Tutor.Ingestion;

/// <summary>
/// Where the pipeline keeps its files. The host supplies the folders; the package never decides them. Every
/// member is read on each use, so a host whose storage location can change at runtime stays correct.
/// </summary>
public interface IIngestionPaths
{
    /// <summary>Folder holding the vector store's <c>content_chunks.json</c> (created when missing).</summary>
    string GetVectorStoreDirectory();

    /// <summary>Folder holding the LSH hyperplane file (created when missing).</summary>
    string GetLshDirectory();

    /// <summary>Folders searched, in order, for <c>eng.traineddata</c> (OCR language data).</summary>
    IReadOnlyList<string> GetTessdataDirectories();
}

/// <summary>
/// Default paths: everything under one root folder (<c>%LocalAppData%\Tutor.Ingestion</c> unless given), with
/// OCR data looked up in <c>tessdata/</c> next to the application, then in the root's own <c>tessdata</c>.
/// </summary>
public sealed class DefaultIngestionPaths : IIngestionPaths
{
    private readonly string root;

    public DefaultIngestionPaths(string? rootDirectory = null)
    {
        root = string.IsNullOrWhiteSpace(rootDirectory)
            ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Tutor.Ingestion")
            : rootDirectory;
    }

    public string GetVectorStoreDirectory() => Ensure(Path.Combine(root, "vectorstore"));
    public string GetLshDirectory() => Ensure(Path.Combine(root, "lsh"));

    public IReadOnlyList<string> GetTessdataDirectories() =>
        [Path.Combine(AppContext.BaseDirectory, "tessdata"), Path.Combine(root, "tessdata")];

    private static string Ensure(string path)
    {
        Directory.CreateDirectory(path);
        return path;
    }
}
