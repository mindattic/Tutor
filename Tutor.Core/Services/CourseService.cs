using System.Text.Json;
using Tutor.Core.Models;
using Tutor.Core.Services.Abstractions;
using Tutor.Core.Services.Logging;

namespace Tutor.Core.Services;

public sealed class CourseService
{
    private const string ResourcesKey = "COURSE_RESOURCES_DATA";
    private const string CoursesKey = "COURSES_DATA";
    private const string ActiveCourseKey = "ACTIVE_COURSE";

    private readonly ISecurePreferences _securePreferences;
    private readonly ChunkingService chunkingService;
    private readonly EmbeddingService embeddingService;
    private readonly VectorStoreService vectorStoreService;
    private readonly ContentFormatterService contentFormatterService;
    private readonly FileResourceService fileResourceService;
    private readonly LSHService lshService;
    private readonly SimHashService simHashService;
    private readonly ConceptMapCollectionService? conceptMapCollectionService;

    private List<CourseResource>? cachedResources;
    private List<Course>? cachedCourses;

    public CourseService(
        ISecurePreferences securePreferences,
        ChunkingService chunkingService,
        EmbeddingService embeddingService,
        VectorStoreService vectorStoreService,
        ContentFormatterService contentFormatterService,
        FileResourceService fileResourceService,
        LSHService lshService,
        SimHashService simHashService,
        ConceptMapCollectionService? conceptMapCollectionService = null)
    {
        _securePreferences = securePreferences;
        this.chunkingService = chunkingService;
        this.embeddingService = embeddingService;
        this.vectorStoreService = vectorStoreService;
        this.contentFormatterService = contentFormatterService;
        this.fileResourceService = fileResourceService;
        this.lshService = lshService;
        this.simHashService = simHashService;
        this.conceptMapCollectionService = conceptMapCollectionService;
        Log.Debug("CourseService initialized");
    }

    // Resource management
    public async Task<List<CourseResource>> GetAllResourcesAsync()
    {
        if (cachedResources != null)
            return cachedResources;

        try
        {
            var json = await _securePreferences.GetAsync(ResourcesKey);
            if (string.IsNullOrEmpty(json))
            {
                cachedResources = new List<CourseResource>();
                Log.Debug("CourseService: No resources found, returning empty list");
                return cachedResources;
            }

            cachedResources = JsonSerializer.Deserialize<List<CourseResource>>(json) ?? new List<CourseResource>();
            Log.Debug($"CourseService: Loaded {cachedResources.Count} resources from storage");
            return cachedResources;
        }
        catch (Exception ex)
        {
            Log.Error($"CourseService: Failed to load resources - {ex.Message}", ex);
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
        Log.Info($"CourseService: Saving resource '{resource.Title}' (ID: {resource.Id})");
        var resources = await GetAllResourcesAsync();
        var existing = resources.FirstOrDefault(r => r.Id == resource.Id);

        if (existing != null)
        {
            resources.Remove(existing);
            resource.UpdatedAt = DateTime.UtcNow;
            Log.Debug($"CourseService: Updating existing resource '{resource.Title}'");
        }

        resources.Add(resource);
        await SaveResourcesAsync(resources);
    }

    /// <summary>
    /// Core save method - saves resource without formatting or processing.
    /// Used by ResourceProcessingService for the initial save step.
    /// Returns the resource ID.
    /// </summary>
    public async Task<string> SaveResourceCoreAsync(CourseResource resource)
    {
        if (string.IsNullOrEmpty(resource.Id))
        {
            resource.Id = Guid.NewGuid().ToString();
        }

        await SaveResourceAsync(resource);


        // Also save to file system for GitHub
        if (!string.IsNullOrWhiteSpace(resource.Content))
        {
            await fileResourceService.SaveOriginalFileAsync(
                resource.FileName ?? $"{resource.Title}.txt",
                resource.Content);
        }

        return resource.Id;
    }

    /// <summary>
    /// Updates just the content of a resource (used after async formatting).
    /// </summary>
    public async Task UpdateResourceContentAsync(string resourceId, string content, bool isFormatted = false)
    {
        var resource = await GetResourceAsync(resourceId);
        if (resource == null)
            return;

        if (isFormatted)
        {
            resource.FormattedContent = content;
            
            // Save formatted to file system
            await fileResourceService.SaveFormattedFileAsync(
                resource.Title,
                content,
                resource.FileName);
        }
        else
        {
            resource.Content = content;
        }

        resource.UpdatedAt = DateTime.UtcNow;
        await SaveResourceAsync(resource);
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
                FileName = AppendGuidToFileName(resource.FileName ?? "", newId),
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
        await _securePreferences.SetAsync(ResourcesKey, json);
    }

    /// <summary>
    /// Saves all resources to storage (used for batch updates/migrations).
    /// </summary>
    public async Task SaveAllResourcesAsync(List<CourseResource> resources)
    {
        await SaveResourcesAsync(resources);
    }

    // Course management
    public async Task<List<Course>> GetAllCoursesAsync()
    {
        if (cachedCourses != null)
            return cachedCourses;

        try
        {
            var json = await _securePreferences.GetAsync(CoursesKey);
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
        Log.Info($"CourseService: Creating course '{name}'");
        var course = new Course
        {
            Name = name,
            Description = description,
            ResourceIds = new List<string>()
        };

        var courses = await GetAllCoursesAsync();
        courses.Add(course);
        await SaveCoursesAsync(courses);
        
        Log.Info($"CourseService: Course '{name}' created with ID: {course.Id}");

        return course;
    }

    public async Task SaveCourseAsync(Course course)
    {
        Log.Debug($"CourseService: Saving course '{course.Name}' (ID: {course.Id})");
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
        Log.Info($"CourseService: Renaming course {courseId} to '{newName}'");
        var courses = await GetAllCoursesAsync();
        var course = courses.FirstOrDefault(c => c.Id == courseId);

        if (course != null)
        {
            var oldName = course.Name;
            course.Name = newName;
            await SaveCoursesAsync(courses);
            Log.Info($"CourseService: Course renamed from '{oldName}' to '{newName}'");
        }
    }

    public async Task DeleteCourseAsync(string id)
    {
        var courses = await GetAllCoursesAsync();
        var course = courses.FirstOrDefault(c => c.Id == id);

        if (course != null)
        {
            Log.Info($"CourseService: Deleting course '{course.Name}' (ID: {id})");
            courses.Remove(course);
            await SaveCoursesAsync(courses);

            // Clear active course if it was deleted
            var activeId = await GetActiveCourseIdAsync();
            if (activeId == id)
            {
                await ClearActiveCourseAsync();
            }
            Log.Info($"CourseService: Course '{course.Name}' deleted");
        }
    }

    private async Task SaveCoursesAsync(List<Course> courses)
    {
        cachedCourses = courses;
        var json = JsonSerializer.Serialize(courses);
        await _securePreferences.SetAsync(CoursesKey, json);
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
            
            // Rebuild ConceptMap collection for the course
            await RebuildCourseConceptMapCollectionAsync(course);
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

        // Filter out empty chunks (shouldn't happen but safeguard)
        var validChunks = textChunks
            .Select((text, index) => (text, index))
            .Where(x => !string.IsNullOrWhiteSpace(x.text))
            .ToList();

        if (validChunks.Count == 0)
            return;

        var textsToEmbed = validChunks.Select(x => x.text).ToList();

        // Generate embeddings for all chunks (EmbeddingService handles batching automatically)
        Log.Debug($"CourseService: Embedding {textsToEmbed.Count} chunks for resource '{resource.Title}'");
        var embeddings = await embeddingService.GetEmbeddingsAsync(textsToEmbed, ct);
        
        if (embeddings.Count != textsToEmbed.Count)
        {
            Log.Error($"CourseService: Embedding count mismatch - expected {textsToEmbed.Count}, got {embeddings.Count}");
            return;
        }

        Log.Debug($"CourseService: Successfully generated {embeddings.Count} embeddings");

        // Create ContentChunk objects with embeddings and signatures
        var chunks = new List<ContentChunk>();
        for (int i = 0; i < validChunks.Count; i++)
        {
            var embedding = embeddings[i];
            var content = validChunks[i].text;
            var originalIndex = validChunks[i].index;

            // Compute semantic signature from embedding using LSH
            var semanticSignature = lshService.GetSignature(embedding);

            // Compute lexical signature from content using SimHash
            var lexicalSignature = simHashService.GetSignature64(content);

            chunks.Add(new ContentChunk
            {
                ResourceId = resourceId,
                CurriculumId = courseId, // Note: ContentChunk still uses CurriculumId internally for storage
                ChunkIndex = originalIndex,
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
            
            // Rebuild ConceptMap collection for the course
            await RebuildCourseConceptMapCollectionAsync(course);
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

    /// <summary>
    /// Rebuilds the KnowledgeBaseCollection for a course from its resources' KnowledgeBases.
    /// This should be called whenever resources are added/removed from a course.
    /// </summary>
    public async Task RebuildCourseConceptMapCollectionAsync(Course course, CancellationToken ct = default)
    {
        if (conceptMapCollectionService == null)
        {
            Log.Warn("CourseService: ConceptMapCollectionService not available, skipping collection rebuild");
            return;
        }

        var resources = await GetCourseResourcesAsync(course.Id);
        
        // Rebuild the collection from resources that have ConceptMaps
        var collection = await conceptMapCollectionService.RebuildForCourseAsync(course, resources, ct);
        
        // Update course with the collection ID
        if (course.ConceptMapCollectionId != collection.Id)
        {
            course.ConceptMapCollectionId = collection.Id;
            var courses = await GetAllCoursesAsync();
            var existingCourse = courses.FirstOrDefault(c => c.Id == course.Id);
            if (existingCourse != null)
            {
                existingCourse.ConceptMapCollectionId = collection.Id;
                await SaveCoursesAsync(courses);
            }
        }
        
        Log.Info($"CourseService: Rebuilt ConceptMap collection for '{course.Name}' with {collection.ConceptMapIds.Count} ConceptMaps");
    }

    /// <summary>
    /// Gets the LoadedConceptMapCollection for a course (all ConceptMaps loaded in memory).
    /// Returns null if no collection exists or if ConceptMapCollectionService is not available.
    /// </summary>
    public async Task<LoadedConceptMapCollection?> GetCourseConceptMapCollectionAsync(string courseId, CancellationToken ct = default)
    {
        if (conceptMapCollectionService == null)
            return null;

        var course = await GetCourseAsync(courseId);
        if (course == null || string.IsNullOrEmpty(course.ConceptMapCollectionId))
            return null;

        return await conceptMapCollectionService.LoadWithConceptMapsAsync(course.ConceptMapCollectionId, ct);
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
            await _securePreferences.SetAsync(ActiveCourseKey, courseId);
        }
    }

    public async Task<string?> GetActiveCourseIdAsync()
    {
        try
        {
            return await _securePreferences.GetAsync(ActiveCourseKey);
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
        _securePreferences.Remove(ActiveCourseKey);
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
            _securePreferences.Remove(progressKey);
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

        // Include concepts from the ConceptMapCollection if available
        var conceptMapCollection = await GetCourseConceptMapCollectionAsync(course.Id, ct);
        if (conceptMapCollection != null && conceptMapCollection.TotalConcepts > 0)
        {
            sb.AppendLine("=== KEY CONCEPTS FROM CONCEPT MAPS ===");
            sb.AppendLine($"Total concepts across all resources: {conceptMapCollection.TotalConcepts}");
            sb.AppendLine();
            
            // Group concepts by their source ConceptMap and list the most important ones
            foreach (var cm in conceptMapCollection.ConceptMaps)
            {
                var resource = resources.FirstOrDefault(r => r.ConceptMapId == cm.Id);
                var sourceName = resource?.Title ?? cm.Name;
                
                sb.AppendLine($"--- Concepts from: {sourceName} ({cm.Concepts.Count} concepts) ---");
                
                // Get concepts ordered by complexity (foundational first)
                var orderedConcepts = cm.ComplexityOrder
                    .OrderBy(c => c.Level)
                    .ThenBy(c => c.PrerequisiteCount)
                    .Take(15)  // Limit to top 15 per resource
                    .Select(c => cm.GetConcept(c.ConceptId))
                    .Where(c => c != null)
                    .ToList();
                
                if (orderedConcepts.Count > 0)
                {
                    foreach (var concept in orderedConcepts)
                    {
                        sb.AppendLine($"  - {concept!.Title}: {concept.Summary}");
                    }
                }
                else
                {
                    // Fallback: just list first 10 concepts
                    foreach (var concept in cm.Concepts.Take(10))
                    {
                        sb.AppendLine($"  - {concept.Title}: {concept.Summary}");
                    }
                }
                sb.AppendLine();
            }
            
            // Show cross-resource relationships if any exist
            var allRelations = conceptMapCollection.GetAllRelations().ToList();
            if (allRelations.Count > 0)
            {
                sb.AppendLine("=== KEY CONCEPT RELATIONSHIPS ===");
                var prereqs = allRelations
                    .Where(r => r.RelationType == ConceptRelationType.Prerequisite)
                    .Take(10)
                    .ToList();
                
                if (prereqs.Count > 0)
                {
                    sb.AppendLine("Prerequisites (learn these first):");
                    foreach (var rel in prereqs)
                    {
                        sb.AppendLine($"  - {rel.SourceConceptId} -> {rel.TargetConceptId}");
                    }
                }
                sb.AppendLine();
            }
        }

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
        sb.AppendLine("Use the key concepts and relationships to structure the learning path.");

        return sb.ToString();
    }


    /// <summary>
    /// Gets all resources for a course along with their ConceptMap build status.
    /// Useful for displaying resource status in the UI.
    /// </summary>
    public async Task<List<(CourseResource Resource, bool HasConceptMap)>> GetCourseResourcesWithConceptMapStatusAsync(string courseId)
    {
        var resources = await GetCourseResourcesAsync(courseId);
        return resources.Select(r => (r, r.HasConceptMap)).ToList();
    }

    /// <summary>
    /// Gets the count of resources that have ConceptMaps built.
    /// </summary>
    public async Task<(int Total, int WithConceptMap)> GetResourceConceptMapCountsAsync(string courseId)
    {
        var resources = await GetCourseResourcesAsync(courseId);
        var withCm = resources.Count(r => r.HasConceptMap);
        return (resources.Count, withCm);
    }

    /// <summary>
    /// Gets ConceptMap IDs for all resources in a course that have them built.
    /// Used to build or update the course's ConceptMapCollection.
    /// </summary>
    public async Task<List<string>> GetResourceConceptMapIdsAsync(string courseId)
    {
        var resources = await GetCourseResourcesAsync(courseId);
        return resources
            .Where(r => r.HasConceptMap && !string.IsNullOrEmpty(r.ConceptMapId))
            .Select(r => r.ConceptMapId!)
            .ToList();
    }

    /// <summary>
    /// Updates the ConceptMapCollectionId for a course.
    /// Called after building or updating the course's collection.
    /// </summary>
    public async Task UpdateCourseConceptMapCollectionAsync(string courseId, string collectionId)
    {
        var course = await GetCourseAsync(courseId);
        if (course == null)
            return;

        course.ConceptMapCollectionId = collectionId;
        course.UpdatedAt = DateTime.UtcNow;
        await SaveCourseAsync(course);
        
        
        Log.Info($"CourseService: Updated course '{course.Name}' with ConceptMap collection: {collectionId}");
    }
}
