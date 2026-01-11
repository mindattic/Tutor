using System.Text.Json;
using Tutor.Models;

namespace Tutor.Services;

public sealed class CourseService
{
    private const string ResourcesKey = "COURSE_RESOURCES_DATA";
    private const string CoursesKey = "COURSES_DATA";
    private const string ActiveCourseKey = "ACTIVE_COURSE";

    private readonly ChunkingService chunkingService;
    private readonly EmbeddingService embeddingService;
    private readonly VectorStoreService vectorStoreService;
    private readonly ContentFormatterService contentFormatterService;
    private readonly FileResourceService fileResourceService;
    private readonly LSHService lshService;
    private readonly SimHashService simHashService;

    private List<CourseResource>? cachedResources;
    private List<Course>? cachedCourses;

    public CourseService(
        ChunkingService chunkingService,
        EmbeddingService embeddingService,
        VectorStoreService vectorStoreService,
        ContentFormatterService contentFormatterService,
        FileResourceService fileResourceService,
        LSHService lshService,
        SimHashService simHashService)
    {
        this.chunkingService = chunkingService;
        this.embeddingService = embeddingService;
        this.vectorStoreService = vectorStoreService;
        this.contentFormatterService = contentFormatterService;
        this.fileResourceService = fileResourceService;
        this.lshService = lshService;
        this.simHashService = simHashService;
    }

    // Resource management
    public async Task<List<CourseResource>> GetAllResourcesAsync()
    {
        if (cachedResources != null)
            return cachedResources;

        try
        {
            var json = await SecureStorage.GetAsync(ResourcesKey);
            if (string.IsNullOrEmpty(json))
            {
                cachedResources = new List<CourseResource>();
                return cachedResources;
            }

            cachedResources = JsonSerializer.Deserialize<List<CourseResource>>(json) ?? new List<CourseResource>();
            return cachedResources;
        }
        catch
        {
            cachedResources = new List<CourseResource>();
            return cachedResources;
        }
    }

    public async Task<CourseResource?> GetResourceAsync(string id)
    {
        var resources = await GetAllResourcesAsync();
        return resources.FirstOrDefault(r => r.Id == id);
    }

    public async Task SaveResourceAsync(CourseResource resource)
    {
        var resources = await GetAllResourcesAsync();
        var existing = resources.FirstOrDefault(r => r.Id == resource.Id);

        if (existing != null)
        {
            resources.Remove(existing);
            resource.UpdatedAt = DateTime.UtcNow;
        }

        resources.Add(resource);
        await SaveResourcesAsync(resources);
    }

    /// <summary>
    /// Save a resource with optional auto-formatting of content.
    /// When autoFormat is true, creates a NEW formatted resource and keeps the original intact.
    /// Returns the ID of the saved resource (original if not formatted, new if formatted).
    /// </summary>
    public async Task<string> SaveResourceAsync(CourseResource resource, bool autoFormat, CancellationToken ct = default)
    {
        return await SaveResourceAsync(resource, autoFormat, null, ct);
    }

    /// <summary>
    /// Save a resource with optional auto-formatting of content and progress reporting.
    /// When autoFormat is true, creates a NEW formatted resource and keeps the original intact.
    /// Returns the ID of the saved resource (original if not formatted, new if formatted).
    /// </summary>
    public async Task<string> SaveResourceAsync(CourseResource resource, bool autoFormat, Action<int, int>? onFormatProgress, CancellationToken ct = default)
    {
        if (autoFormat && !string.IsNullOrWhiteSpace(resource.Content))
        {
            // Save the original resource first (to SecureStorage)
            await SaveResourceAsync(resource);

            // Also save original to file system for GitHub
            await fileResourceService.SaveOriginalFileAsync(
                resource.FileName ?? $"{resource.Title}.txt",
                resource.Content);

            // Create a new resource with formatted content
            var formattedContent = await contentFormatterService.FormatContentAsync(resource.Content, onFormatProgress, ct);
            var newId = Guid.NewGuid().ToString();
            
            var formattedResource = new CourseResource
            {
                Id = newId,
                Title = $"{resource.Title} (Formatted)",
                Author = resource.Author,
                Year = resource.Year,
                Description = $"AI-formatted version of: {resource.Title}",
                Content = formattedContent,
                Type = resource.Type,
                FileName = AppendGuidToFileName(resource.FileName, newId),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await SaveResourceAsync(formattedResource);

            // Save formatted content to file system for GitHub
            await fileResourceService.SaveFormattedFileAsync(
                resource.Title,
                formattedContent,
                resource.FileName);

            return formattedResource.Id;
        }

        await SaveResourceAsync(resource);
        return resource.Id;
    }

    /// <summary>
    /// Appends a GUID to a filename to indicate it's been processed.
    /// Example: "science-textbook.txt" -> "science-textbook_abc123.txt"
    /// </summary>
    private static string AppendGuidToFileName(string fileName, string guid)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return $"formatted_{guid[..8]}";

        var extension = Path.GetExtension(fileName);
        var nameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
        var shortGuid = guid.Length >= 8 ? guid[..8] : guid;
        
        return $"{nameWithoutExt}_{shortGuid}{extension}";
    }

    /// <summary>
    /// Format an existing resource's content using AI.
    /// </summary>
    public async Task FormatResourceContentAsync(string resourceId, CancellationToken ct = default)
    {
        var resource = await GetResourceAsync(resourceId);
        if (resource == null || string.IsNullOrWhiteSpace(resource.Content))
            return;

        resource.Content = await contentFormatterService.FormatContentAsync(resource.Content, ct);
        resource.UpdatedAt = DateTime.UtcNow;

        await SaveResourceAsync(resource);
    }

    /// <summary>
    /// Quick format (no AI) - applies basic markdown rules.
    /// </summary>
    public async Task QuickFormatResourceContentAsync(string resourceId)
    {
        var resource = await GetResourceAsync(resourceId);
        if (resource == null || string.IsNullOrWhiteSpace(resource.Content))
            return;

        resource.Content = ContentFormatterService.QuickFormat(resource.Content);
        resource.UpdatedAt = DateTime.UtcNow;

        await SaveResourceAsync(resource);
    }

    public async Task DeleteResourceAsync(string id)
    {
        var resources = await GetAllResourcesAsync();
        var resource = resources.FirstOrDefault(r => r.Id == id);

        if (resource != null)
        {
            resources.Remove(resource);
            await SaveResourcesAsync(resources);

            // Remove from all courses
            var courses = await GetAllCoursesAsync();
            foreach (var course in courses)
            {
                course.ResourceIds.Remove(id);
            }
            await SaveCoursesAsync(courses);
        }
    }

    private async Task SaveResourcesAsync(List<CourseResource> resources)
    {
        cachedResources = resources;
        var json = JsonSerializer.Serialize(resources);
        await SecureStorage.SetAsync(ResourcesKey, json);
    }

    // Course management
    public async Task<List<Course>> GetAllCoursesAsync()
    {
        if (cachedCourses != null)
            return cachedCourses;

        try
        {
            var json = await SecureStorage.GetAsync(CoursesKey);
            if (string.IsNullOrEmpty(json))
            {
                cachedCourses = new List<Course>();
                return cachedCourses;
            }

            cachedCourses = JsonSerializer.Deserialize<List<Course>>(json) ?? new List<Course>();
            return cachedCourses;
        }
        catch
        {
            cachedCourses = new List<Course>();
            return cachedCourses;
        }
    }

    public async Task<Course?> GetCourseAsync(string id)
    {
        var courses = await GetAllCoursesAsync();
        return courses.FirstOrDefault(c => c.Id == id);
    }

    public async Task<Course> CreateCourseAsync(string name, string description = "")
    {
        var course = new Course
        {
            Name = name,
            Description = description,
            ResourceIds = new List<string>()
        };

        var courses = await GetAllCoursesAsync();
        courses.Add(course);
        await SaveCoursesAsync(courses);

        return course;
    }

    public async Task SaveCourseAsync(Course course)
    {
        var courses = await GetAllCoursesAsync();
        var existing = courses.FirstOrDefault(c => c.Id == course.Id);

        if (existing != null)
        {
            courses.Remove(existing);
        }

        courses.Add(course);
        await SaveCoursesAsync(courses);
    }

    public async Task RenameCourseAsync(string courseId, string newName)
    {
        var courses = await GetAllCoursesAsync();
        var course = courses.FirstOrDefault(c => c.Id == courseId);

        if (course != null)
        {
            course.Name = newName;
            await SaveCoursesAsync(courses);
        }
    }

    public async Task DeleteCourseAsync(string id)
    {
        var courses = await GetAllCoursesAsync();
        var course = courses.FirstOrDefault(c => c.Id == id);

        if (course != null)
        {
            courses.Remove(course);
            await SaveCoursesAsync(courses);

            // Clear active course if it was deleted
            var activeId = await GetActiveCourseIdAsync();
            if (activeId == id)
            {
                await ClearActiveCourseAsync();
            }
        }
    }

    private async Task SaveCoursesAsync(List<Course> courses)
    {
        cachedCourses = courses;
        var json = JsonSerializer.Serialize(courses);
        await SecureStorage.SetAsync(CoursesKey, json);
    }

    // Course resource management
    public async Task AddResourceToCourseAsync(string courseId, string resourceId)
    {
        var courses = await GetAllCoursesAsync();
        var course = courses.FirstOrDefault(c => c.Id == courseId);

        if (course != null && !course.ResourceIds.Contains(resourceId))
        {
            course.ResourceIds.Add(resourceId);
            await SaveCoursesAsync(courses);
            
            // Chunk and embed the resource for RAG
            await ChunkAndEmbedResourceAsync(resourceId, courseId);
        }
    }

    /// <summary>
    /// Chunk and embed a resource's content for RAG retrieval.
    /// </summary>
    public async Task ChunkAndEmbedResourceAsync(string resourceId, string courseId, CancellationToken ct = default)
    {
        var resource = await GetResourceAsync(resourceId);
        if (resource == null || string.IsNullOrWhiteSpace(resource.Content))
            return;

        // Check if already embedded
        if (await vectorStoreService.HasChunksForResourceAsync(resourceId))
            return;

        // Split content into chunks
        var textChunks = chunkingService.ChunkContent(resource.Content);
        if (textChunks.Count == 0)
            return;

        // Generate embeddings for all chunks (batch API call)
        var embeddings = await embeddingService.GetEmbeddingsAsync(textChunks, ct);
        if (embeddings.Count != textChunks.Count)
            return;

        // Create ContentChunk objects with embeddings and signatures
        var chunks = new List<ContentChunk>();
        for (int i = 0; i < textChunks.Count; i++)
        {
            var embedding = embeddings[i];
            var content = textChunks[i];

            // Compute semantic signature from embedding using LSH
            var semanticSignature = lshService.GetSignature(embedding);

            // Compute lexical signature from content using SimHash
            var lexicalSignature = simHashService.GetSignature64(content);

            chunks.Add(new ContentChunk
            {
                ResourceId = resourceId,
                CurriculumId = courseId, // Note: ContentChunk still uses CurriculumId internally for storage
                ChunkIndex = i,
                Content = content,
                Embedding = embedding,
                SemanticSignature = semanticSignature,
                LexicalSignature = lexicalSignature,
                SourceTitle = resource.Title
            });
        }

        // Store chunks
        await vectorStoreService.StoreChunksAsync(resourceId, courseId, chunks);
    }

    /// <summary>
    /// Re-embed all resources in a course (useful after import or update).
    /// </summary>
    public async Task ReindexCourseAsync(string courseId, CancellationToken ct = default)
    {
        var course = await GetCourseAsync(courseId);
        if (course == null)
            return;

        // Clear existing chunks for this course
        await vectorStoreService.RemoveChunksForCurriculumAsync(courseId);

        // Re-embed each resource
        foreach (var resourceId in course.ResourceIds)
        {
            await ChunkAndEmbedResourceAsync(resourceId, courseId, ct);
        }
    }

    public async Task RemoveResourceFromCourseAsync(string courseId, string resourceId)
    {
        var courses = await GetAllCoursesAsync();
        var course = courses.FirstOrDefault(c => c.Id == courseId);

        if (course != null)
        {
            course.ResourceIds.Remove(resourceId);
            await SaveCoursesAsync(courses);
            
            // Remove chunks for this resource
            await vectorStoreService.RemoveChunksForResourceAsync(resourceId);
        }
    }

    public async Task<List<CourseResource>> GetCourseResourcesAsync(string courseId)
    {
        var course = await GetCourseAsync(courseId);
        if (course == null)
            return new List<CourseResource>();

        var allResources = await GetAllResourcesAsync();
        return allResources.Where(r => course.ResourceIds.Contains(r.Id)).ToList();
    }

    // Active course management
    public async Task SetActiveCourseAsync(string? courseId)
    {
        if (string.IsNullOrEmpty(courseId))
        {
            await ClearActiveCourseAsync();
        }
        else
        {
            await SecureStorage.SetAsync(ActiveCourseKey, courseId);
        }
    }

    public async Task<string?> GetActiveCourseIdAsync()
    {
        try
        {
            return await SecureStorage.GetAsync(ActiveCourseKey);
        }
        catch
        {
            return null;
        }
    }

    public async Task<Course?> GetActiveCourseAsync()
    {
        var activeId = await GetActiveCourseIdAsync();
        if (string.IsNullOrEmpty(activeId))
            return null;

        return await GetCourseAsync(activeId);
    }

    public async Task ClearActiveCourseAsync()
    {
        SecureStorage.Remove(ActiveCourseKey);
        await Task.CompletedTask;
    }

    // Reset progress for a course for a given user
    public async Task ResetCourseForUserAsync(string userId, string courseId)
    {
        try
        {
            // Remove active course if it matches
            var active = await GetActiveCourseIdAsync();
            if (active == courseId)
            {
                await ClearActiveCourseAsync();
            }

            // Remove any stored progress key for this course for this user
            var progressKey = $"COURSE_PROGRESS_{userId}_{courseId}";
            SecureStorage.Remove(progressKey);
        }
        catch { }

        await Task.CompletedTask;
    }

    // Get combined content for chat context - NOW USES RAG!
    public async Task<string> GetActiveCourseContentAsync()
    {
        var activeCourse = await GetActiveCourseAsync();
        if (activeCourse == null)
            return "";

        var resources = await GetCourseResourcesAsync(activeCourse.Id);
        if (resources.Count == 0)
            return "";

        // Just return course metadata, not full content
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"=== COURSE: {activeCourse.Name} ===");
        sb.AppendLine("The student is learning from this course. Use RAG-retrieved content to answer questions.");
        sb.AppendLine();
        sb.AppendLine("Available resources:");
        foreach (var resource in resources)
        {
            sb.AppendLine($"- {resource.Title}");
        }
        sb.AppendLine();
        sb.AppendLine("Note: Specific content will be provided via context retrieval.");
        return sb.ToString();
    }

    /// <summary>
    /// Get relevant content chunks for a query using RAG.
    /// This is the efficient method that only retrieves relevant content.
    /// </summary>
    public async Task<string> GetRelevantContentAsync(string query, int topK = 5, CancellationToken ct = default)
    {
        var activeCourse = await GetActiveCourseAsync();
        if (activeCourse == null)
            return "";

        // Search for relevant chunks
        var chunks = await vectorStoreService.SearchAsync(
            query, 
            activeCourse.Id, 
            embeddingService, 
            topK, 
            minSimilarity: 0.25f, 
            ct);

        if (chunks.Count == 0)
        {
            // Fallback: if no chunks found, might need to index
            return await GetFallbackContentAsync(activeCourse);
        }

        // Build context from retrieved chunks
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"=== RELEVANT CONTENT FROM: {activeCourse.Name} ===");
        sb.AppendLine("Answer based ONLY on the following retrieved content:");
        sb.AppendLine();

        var groupedBySource = chunks.GroupBy(c => c.SourceTitle);
        foreach (var group in groupedBySource)
        {
            sb.AppendLine($"--- From: {group.Key} ---");
            foreach (var chunk in group.OrderBy(c => c.ChunkIndex))
            {
                sb.AppendLine(chunk.Content);
                sb.AppendLine();
            }
        }

        sb.AppendLine("=== END RETRIEVED CONTENT ===");
        sb.AppendLine("If the answer is not in the above content, say the topic is not covered in the current course.");
        
        return sb.ToString();
    }

    /// <summary>
    /// Fallback for when RAG hasn't been indexed yet.
    /// Returns truncated content to avoid huge payloads.
    /// </summary>
    private async Task<string> GetFallbackContentAsync(Course course)
    {
        var resources = await GetCourseResourcesAsync(course.Id);
        if (resources.Count == 0)
            return "";

        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"=== COURSE: {course.Name} (FALLBACK - NOT INDEXED) ===");
        sb.AppendLine("Note: Content has not been indexed for efficient search. Showing truncated content.");
        sb.AppendLine();

        const int maxCharsPerResource = 2000;
        foreach (var resource in resources)
        {
            sb.AppendLine($"--- {resource.Title} ---");
            var content = resource.Content.Length > maxCharsPerResource 
                ? resource.Content[..maxCharsPerResource] + "... [truncated]"
                : resource.Content;
            sb.AppendLine(content);
            sb.AppendLine();
        }

        return sb.ToString();
    }

    /// <summary>
    /// Gets an overview of all resources in a course for curriculum planning.
    /// This provides a holistic view by extracting key information from each resource.
    /// </summary>
    public async Task<string> GetCourseOverviewAsync(string? courseId = null, CancellationToken ct = default)
    {
        var course = courseId != null 
            ? await GetCourseAsync(courseId) 
            : await GetActiveCourseAsync();
            
        if (course == null)
            return "";

        var resources = await GetCourseResourcesAsync(course.Id);
        if (resources.Count == 0)
            return "";

        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"=== COURSE OVERVIEW: {course.Name} ===");
        if (!string.IsNullOrWhiteSpace(course.Description))
        {
            sb.AppendLine($"Description: {course.Description}");
        }
        sb.AppendLine($"Total Resources: {resources.Count}");
        sb.AppendLine();

        // Provide structured information about each resource
        sb.AppendLine("=== RESOURCES IN THIS COURSE ===");
        foreach (var resource in resources)
        {
            sb.AppendLine($"### {resource.Title}");
            if (!string.IsNullOrWhiteSpace(resource.Author))
                sb.AppendLine($"Author: {resource.Author}");
            if (!string.IsNullOrWhiteSpace(resource.Year))
                sb.AppendLine($"Year: {resource.Year}");
            if (!string.IsNullOrWhiteSpace(resource.Description))
                sb.AppendLine($"Description: {resource.Description}");
            
            // Extract a representative sample from the content (first ~1500 chars)
            // This helps the AI understand what each resource covers
            if (!string.IsNullOrWhiteSpace(resource.Content))
            {
                var sample = resource.Content.Length > 1500 
                    ? resource.Content[..1500] + "..."
                    : resource.Content;
                sb.AppendLine($"Content Preview:");
                sb.AppendLine(sample);
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }

    /// <summary>
    /// Gets unified context for curriculum planning that spans ALL resources in the course.
    /// Combines course overview with broad RAG sampling from each resource.
    /// This is specifically designed for generating course outlines/topics.
    /// </summary>
    public async Task<string> GetUnifiedCurriculumContextAsync(string? courseId = null, CancellationToken ct = default)
    {
        var course = courseId != null 
            ? await GetCourseAsync(courseId) 
            : await GetActiveCourseAsync();
            
        if (course == null)
            return "";

        var resources = await GetCourseResourcesAsync(course.Id);
        if (resources.Count == 0)
            return "";

        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"=== UNIFIED CURRICULUM CONTEXT: {course.Name} ===");
        sb.AppendLine("You are creating a curriculum from MULTIPLE resources. Consider ALL of them together.");
        sb.AppendLine($"Total Resources: {resources.Count}");
        sb.AppendLine();

        // List all resources first
        sb.AppendLine("=== AVAILABLE RESOURCES ===");
        for (int i = 0; i < resources.Count; i++)
        {
            var r = resources[i];
            sb.AppendLine($"{i + 1}. {r.Title}" + 
                (string.IsNullOrWhiteSpace(r.Description) ? "" : $" - {r.Description}"));
        }
        sb.AppendLine();

        // For each resource, get representative chunks to capture key topics
        sb.AppendLine("=== KEY CONTENT FROM EACH RESOURCE ===");
        foreach (var resource in resources)
        {
            sb.AppendLine($"--- FROM: {resource.Title} ---");
            
            // Get all chunks for this resource and sample them
            var chunks = await vectorStoreService.GetChunksForResourceAsync(resource.Id);
            
            if (chunks.Count > 0)
            {
                // Sample chunks evenly across the resource to get representative coverage
                var sampleCount = Math.Min(5, chunks.Count);
                var step = chunks.Count / sampleCount;
                
                for (int i = 0; i < sampleCount; i++)
                {
                    var chunk = chunks[Math.Min(i * step, chunks.Count - 1)];
                    sb.AppendLine(chunk.Content);
                    sb.AppendLine();
                }
            }
            else
            {
                // Fallback: sample from raw content
                var contentLength = resource.Content.Length;
                var sampleSize = Math.Min(2000, contentLength);
                
                // Get beginning sample
                sb.AppendLine(resource.Content[..Math.Min(1000, contentLength)]);
                sb.AppendLine();
                
                // Get middle sample if content is long enough
                if (contentLength > 2000)
                {
                    var midStart = contentLength / 2 - 500;
                    sb.AppendLine(resource.Content.Substring(midStart, Math.Min(1000, contentLength - midStart)));
                    sb.AppendLine();
                }
            }
        }

        sb.AppendLine("=== END UNIFIED CONTEXT ===");
        sb.AppendLine();
        sb.AppendLine("IMPORTANT: Create a curriculum that synthesizes knowledge ACROSS all these resources.");
        sb.AppendLine("Topics should integrate information from multiple resources where relevant.");
        sb.AppendLine("Consider common themes, complementary information, and logical learning progression.");

        return sb.ToString();
    }
}
