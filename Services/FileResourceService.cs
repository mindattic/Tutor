using System.Text.Json;
using Tutor.Models;

namespace Tutor.Services;

/// <summary>
/// Service for saving course resources to files for GitHub check-in.
/// Works alongside SecureStorage for app data.
/// </summary>
public sealed class FileResourceService
{
    /// <summary>
    /// Save an original resource file to the Original folder.
    /// </summary>
    public async Task SaveOriginalFileAsync(string fileName, string content)
    {
        StorageSettings.Load();
        var dir = Path.Combine(StorageSettings.GetResolvedDirectory(), "Original");
        Directory.CreateDirectory(dir);

        var filePath = Path.Combine(dir, SanitizeFileName(fileName));
        await File.WriteAllTextAsync(filePath, content);
    }

    /// <summary>
    /// Save a formatted resource as a markdown file.
    /// </summary>
    public async Task SaveFormattedFileAsync(string title, string content, string? originalFileName = null)
    {
        StorageSettings.Load();
        var dir = Path.Combine(StorageSettings.GetResolvedDirectory(), "Formatted");
        Directory.CreateDirectory(dir);

        var baseName = !string.IsNullOrEmpty(originalFileName) 
            ? Path.GetFileNameWithoutExtension(originalFileName)
            : SanitizeFileName(title);
        
        var fileName = $"{baseName}_formatted.md";
        var filePath = Path.Combine(dir, fileName);

        // Add frontmatter with metadata
        var markdown = $"""
            ---
            title: {title}
            source: {originalFileName ?? "manual entry"}
            formatted: {DateTime.UtcNow:O}
            ---

            {content}
            """;

        await File.WriteAllTextAsync(filePath, markdown);
    }

    /// <summary>
    /// Save a course resource (both original and formatted versions).
    /// </summary>
    public async Task SaveResourceToFilesAsync(CourseResource resource, string? formattedContent = null)
    {
        StorageSettings.Load();

        // Save original content
        if (!string.IsNullOrEmpty(resource.Content) && StorageSettings.PreserveOriginalFiles)
        {
            var originalFileName = !string.IsNullOrEmpty(resource.FileName) 
                ? resource.FileName 
                : $"{SanitizeFileName(resource.Title)}.txt";
            
            await SaveOriginalFileAsync(originalFileName, resource.Content);
        }

        // Save formatted content if provided
        if (!string.IsNullOrEmpty(formattedContent) && StorageSettings.SaveFormattedAsMarkdown)
        {
            await SaveFormattedFileAsync(resource.Title, formattedContent, resource.FileName);
        }
    }

    /// <summary>
    /// Save course metadata to a JSON file.
    /// </summary>
    public async Task SaveCourseAsync(Course course, List<CourseResource> resources)
    {
        StorageSettings.Load();
        var dir = Path.Combine(StorageSettings.GetResolvedDirectory(), "Courses");
        Directory.CreateDirectory(dir);

        var fileName = $"{SanitizeFileName(course.Name)}_{course.Id[..8]}.json";
        var filePath = Path.Combine(dir, fileName);

        var data = new
        {
            course.Id,
            course.Name,
            course.Description,
            CreatedAt = DateTime.UtcNow,
            Resources = resources.Select(r => new
            {
                r.Id,
                r.Title,
                r.Author,
                r.Year,
                r.Description,
                r.FileName,
                ContentLength = r.Content?.Length ?? 0
            }).ToList()
        };

        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(filePath, json);
    }

    /// <summary>
    /// Get all formatted markdown files from the Formatted folder.
    /// </summary>
    public List<string> GetFormattedFiles()
    {
        StorageSettings.Load();
        var dir = Path.Combine(StorageSettings.GetResolvedDirectory(), "Formatted");
        
        if (!Directory.Exists(dir))
            return [];

        return [.. Directory.GetFiles(dir, "*.md")];
    }

    /// <summary>
    /// Get all original files from the Original folder.
    /// </summary>
    public List<string> GetOriginalFiles()
    {
        StorageSettings.Load();
        var dir = Path.Combine(StorageSettings.GetResolvedDirectory(), "Original");
        
        if (!Directory.Exists(dir))
            return [];

        return [.. Directory.GetFiles(dir)];
    }

    /// <summary>
    /// Read a file's content.
    /// </summary>
    public async Task<string> ReadFileAsync(string filePath)
    {
        if (!File.Exists(filePath))
            return "";

        return await File.ReadAllTextAsync(filePath);
    }

    /// <summary>
    /// Delete a file.
    /// </summary>
    public void DeleteFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    /// <summary>
    /// Get the current resources directory.
    /// </summary>
    public string GetResourcesDirectory()
    {
        StorageSettings.Load();
        return StorageSettings.GetResolvedDirectory();
    }

    /// <summary>
    /// Open the learning resources directory in file explorer.
    /// </summary>
    public void OpenResourcesDirectory()
    {
        StorageSettings.OpenInExplorer();
    }

    private static string SanitizeFileName(string name)
    {
        var invalid = Path.GetInvalidFileNameChars();
        var sanitized = new string(name.Select(c => invalid.Contains(c) ? '_' : c).ToArray());
        return sanitized.Trim().Replace(' ', '-').ToLowerInvariant();
    }
}
