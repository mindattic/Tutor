using System.Text.Json;
using Tutor.Models;

namespace Tutor.Services;

public sealed class LearningResourceService
{
    private const string LearningResourcesKey = "LEARNING_RESOURCES_DATA";
    private const string CurriculaKey = "CURRICULA_DATA";
    private const string ActiveCurriculumKey = "ACTIVE_CURRICULUM";

    private readonly ChunkingService chunkingService;
    private readonly EmbeddingService embeddingService;
    private readonly VectorStoreService vectorStoreService;
    private readonly ContentFormatterService contentFormatterService;

    private List<LearningResource>? cachedLearningResources;
    private List<Curriculum>? cachedCurricula;

    public LearningResourceService(
        ChunkingService chunkingService,
        EmbeddingService embeddingService,
        VectorStoreService vectorStoreService,
        ContentFormatterService contentFormatterService)
    {
        this.chunkingService = chunkingService;
        this.embeddingService = embeddingService;
        this.vectorStoreService = vectorStoreService;
        this.contentFormatterService = contentFormatterService;
    }

    // Learning Resources management
    public async Task<List<LearningResource>> GetAllResourcesAsync()
    {
        if (cachedLearningResources != null)
            return cachedLearningResources;

        try
        {
            var json = await SecureStorage.GetAsync(LearningResourcesKey);
            if (string.IsNullOrEmpty(json))
            {
                cachedLearningResources = new List<LearningResource>();
                return cachedLearningResources;
            }

            cachedLearningResources = JsonSerializer.Deserialize<List<LearningResource>>(json) ?? new List<LearningResource>();
            return cachedLearningResources;
        }
        catch
        {
            cachedLearningResources = new List<LearningResource>();
            return cachedLearningResources;
        }
    }

    public async Task<LearningResource?> GetResourceAsync(string id)
    {
        var learningResources = await GetAllResourcesAsync();
        return learningResources.FirstOrDefault(r => r.Id == id);
    }

    public async Task SaveResourceAsync(LearningResource learningResource)
    {
        var learningResources = await GetAllResourcesAsync();
        var existing = learningResources.FirstOrDefault(r => r.Id == learningResource.Id);

        if (existing != null)
        {
            learningResources.Remove(existing);
            learningResource.UpdatedAt = DateTime.UtcNow;
        }

        learningResources.Add(learningResource);
        await SaveLearningResourcesAsync(learningResources);
    }

    /// <summary>
    /// Save a resource with optional auto-formatting of content.
    /// When autoFormat is true, creates a NEW formatted resource and keeps the original intact.
    /// Returns the ID of the saved resource (original if not formatted, new if formatted).
    /// </summary>
    public async Task<string> SaveResourceAsync(LearningResource learningResource, bool autoFormat, CancellationToken ct = default)
    {
        return await SaveResourceAsync(learningResource, autoFormat, null, ct);
    }

    /// <summary>
    /// Save a resource with optional auto-formatting of content and progress reporting.
    /// When autoFormat is true, creates a NEW formatted resource and keeps the original intact.
    /// Returns the ID of the saved resource (original if not formatted, new if formatted).
    /// </summary>
    public async Task<string> SaveResourceAsync(LearningResource learningResource, bool autoFormat, Action<int, int>? onFormatProgress, CancellationToken ct = default)
    {
        if (autoFormat && !string.IsNullOrWhiteSpace(learningResource.Content))
        {
            // Save the original resource first
            await SaveResourceAsync(learningResource);

            // Create a new resource with formatted content
            var formattedContent = await contentFormatterService.FormatContentAsync(learningResource.Content, onFormatProgress, ct);
            var newId = Guid.NewGuid().ToString();
            
            var formattedResource = new LearningResource
            {
                Id = newId,
                Title = $"{learningResource.Title} (Formatted)",
                Author = learningResource.Author,
                Year = learningResource.Year,
                Description = $"AI-formatted version of: {learningResource.Title}",
                Content = formattedContent,
                Type = learningResource.Type,
                FileName = AppendGuidToFileName(learningResource.FileName, newId),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await SaveResourceAsync(formattedResource);
            return formattedResource.Id;
        }

        await SaveResourceAsync(learningResource);
        return learningResource.Id;
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
        var learningResources = await GetAllResourcesAsync();
        var learningResource = learningResources.FirstOrDefault(r => r.Id == id);

        if (learningResource != null)
        {
            learningResources.Remove(learningResource);
            await SaveLearningResourcesAsync(learningResources);

            // Remove from all curricula
            var curricula = await GetAllCurriculaAsync();
            foreach (var curriculum in curricula)
            {
                curriculum.ResourceIds.Remove(id);
            }
            await SaveCurriculaAsync(curricula);
        }
    }

    private async Task SaveLearningResourcesAsync(List<LearningResource> learningResources)
    {
        cachedLearningResources = learningResources;
        var json = JsonSerializer.Serialize(learningResources);
        await SecureStorage.SetAsync(LearningResourcesKey, json);
    }

    // Curricula management
    public async Task<List<Curriculum>> GetAllCurriculaAsync()
    {
        if (cachedCurricula != null)
            return cachedCurricula;

        try
        {
            var json = await SecureStorage.GetAsync(CurriculaKey);
            if (string.IsNullOrEmpty(json))
            {
                cachedCurricula = new List<Curriculum>();
                return cachedCurricula;
            }

            cachedCurricula = JsonSerializer.Deserialize<List<Curriculum>>(json) ?? new List<Curriculum>();
            return cachedCurricula;
        }
        catch
        {
            cachedCurricula = new List<Curriculum>();
            return cachedCurricula;
        }
    }

    public async Task<Curriculum?> GetCurriculumAsync(string id)
    {
        var curricula = await GetAllCurriculaAsync();
        return curricula.FirstOrDefault(c => c.Id == id);
    }

    public async Task<Curriculum> CreateCurriculumAsync(string name, string description = "")
    {
        var curriculum = new Curriculum
        {
            Name = name,
            Description = description,
            ResourceIds = new List<string>()
        };

        var curricula = await GetAllCurriculaAsync();
        curricula.Add(curriculum);
        await SaveCurriculaAsync(curricula);

        return curriculum;
    }

    public async Task SaveCurriculumAsync(Curriculum curriculum)
    {
        var curricula = await GetAllCurriculaAsync();
        var existing = curricula.FirstOrDefault(c => c.Id == curriculum.Id);

        if (existing != null)
        {
            curricula.Remove(existing);
        }

        curricula.Add(curriculum);
        await SaveCurriculaAsync(curricula);
    }

    public async Task RenameCurriculumAsync(string curriculumId, string newName)
    {
        var curricula = await GetAllCurriculaAsync();
        var curriculum = curricula.FirstOrDefault(c => c.Id == curriculumId);

        if (curriculum != null)
        {
            curriculum.Name = newName;
            await SaveCurriculaAsync(curricula);
        }
    }

    public async Task DeleteCurriculumAsync(string id)
    {
        var curricula = await GetAllCurriculaAsync();
        var curriculum = curricula.FirstOrDefault(c => c.Id == id);

        if (curriculum != null)
        {
            curricula.Remove(curriculum);
            await SaveCurriculaAsync(curricula);

            // Clear active curriculum if it was deleted
            var activeId = await GetActiveCurriculumIdAsync();
            if (activeId == id)
            {
                await ClearActiveCurriculumAsync();
            }
        }
    }

    private async Task SaveCurriculaAsync(List<Curriculum> curricula)
    {
        cachedCurricula = curricula;
        var json = JsonSerializer.Serialize(curricula);
        await SecureStorage.SetAsync(CurriculaKey, json);
    }

    // Curriculum resource management
    public async Task AddResourceToCurriculumAsync(string curriculumId, string resourceId)
    {
        var curricula = await GetAllCurriculaAsync();
        var curriculum = curricula.FirstOrDefault(c => c.Id == curriculumId);

        if (curriculum != null && !curriculum.ResourceIds.Contains(resourceId))
        {
            curriculum.ResourceIds.Add(resourceId);
            await SaveCurriculaAsync(curricula);
            
            // Chunk and embed the resource for RAG
            await ChunkAndEmbedResourceAsync(resourceId, curriculumId);
        }
    }

    /// <summary>
    /// Chunk and embed a resource's content for RAG retrieval.
    /// </summary>
    public async Task ChunkAndEmbedResourceAsync(string resourceId, string curriculumId, CancellationToken ct = default)
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

        // Create ContentChunk objects
        var chunks = new List<ContentChunk>();
        for (int i = 0; i < textChunks.Count; i++)
        {
            chunks.Add(new ContentChunk
            {
                ResourceId = resourceId,
                CurriculumId = curriculumId,
                ChunkIndex = i,
                Content = textChunks[i],
                Embedding = embeddings[i],
                SourceTitle = resource.Title
            });
        }

        // Store chunks
        await vectorStoreService.StoreChunksAsync(resourceId, curriculumId, chunks);
    }

    /// <summary>
    /// Re-embed all resources in a curriculum (useful after import or update).
    /// </summary>
    public async Task ReindexCurriculumAsync(string curriculumId, CancellationToken ct = default)
    {
        var curriculum = await GetCurriculumAsync(curriculumId);
        if (curriculum == null)
            return;

        // Clear existing chunks for this curriculum
        await vectorStoreService.RemoveChunksForCurriculumAsync(curriculumId);

        // Re-embed each resource
        foreach (var resourceId in curriculum.ResourceIds)
        {
            await ChunkAndEmbedResourceAsync(resourceId, curriculumId, ct);
        }
    }

    public async Task RemoveResourceFromCurriculumAsync(string curriculumId, string resourceId)
    {
        var curricula = await GetAllCurriculaAsync();
        var curriculum = curricula.FirstOrDefault(c => c.Id == curriculumId);

        if (curriculum != null)
        {
            curriculum.ResourceIds.Remove(resourceId);
            await SaveCurriculaAsync(curricula);
            
            // Remove chunks for this resource
            await vectorStoreService.RemoveChunksForResourceAsync(resourceId);
        }
    }

    public async Task<List<LearningResource>> GetCurriculumResourcesAsync(string curriculumId)
    {
        var curriculum = await GetCurriculumAsync(curriculumId);
        if (curriculum == null)
            return new List<LearningResource>();

        var allResources = await GetAllResourcesAsync();
        return allResources.Where(r => curriculum.ResourceIds.Contains(r.Id)).ToList();
    }

    // Active curriculum management
    public async Task SetActiveCurriculumAsync(string? curriculumId)
    {
        if (string.IsNullOrEmpty(curriculumId))
        {
            await ClearActiveCurriculumAsync();
        }
        else
        {
            await SecureStorage.SetAsync(ActiveCurriculumKey, curriculumId);
        }
    }

    public async Task<string?> GetActiveCurriculumIdAsync()
    {
        try
        {
            return await SecureStorage.GetAsync(ActiveCurriculumKey);
        }
        catch
        {
            return null;
        }
    }

    public async Task<Curriculum?> GetActiveCurriculumAsync()
    {
        var activeId = await GetActiveCurriculumIdAsync();
        if (string.IsNullOrEmpty(activeId))
            return null;

        return await GetCurriculumAsync(activeId);
    }

    public async Task ClearActiveCurriculumAsync()
    {
        SecureStorage.Remove(ActiveCurriculumKey);
        await Task.CompletedTask;
    }

    // Reset progress for a curriculum for a given user (placeholder - currently clears learned markers and active curriculum)
    public async Task ResetCurriculumForUserAsync(string userId, string curriculumId)
    {
        // For now, simply clear the active curriculum and any user-specific markers stored in SecureStorage.
        // Future: track per-user progress and remove it here.
        try
        {
            // Remove active curriculum if it matches
            var active = await GetActiveCurriculumIdAsync();
            if (active == curriculumId)
            {
                await ClearActiveCurriculumAsync();
            }

            // Remove any stored progress key for this curriculum for this user
            var progressKey = $"CURRICULUM_PROGRESS_{userId}_{curriculumId}";
            SecureStorage.Remove(progressKey);
        }
        catch { }

        await Task.CompletedTask;
    }

    // Get combined content for chat context - NOW USES RAG!
    // This is the OLD method that sends ALL content - kept for fallback
    public async Task<string> GetActiveCurriculumContentAsync()
    {
        // Use RAG-based retrieval instead of full content
        // Return minimal context - actual content retrieved per query
        var activeCurriculum = await GetActiveCurriculumAsync();
        if (activeCurriculum == null)
            return "";

        var resources = await GetCurriculumResourcesAsync(activeCurriculum.Id);
        if (resources.Count == 0)
            return "";

        // Just return curriculum metadata, not full content
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"=== CURRICULUM: {activeCurriculum.Name} ===");
        sb.AppendLine("The student is learning from this curriculum. Use RAG-retrieved content to answer questions.");
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
        var activeCurriculum = await GetActiveCurriculumAsync();
        if (activeCurriculum == null)
            return "";

        // Search for relevant chunks
        var chunks = await vectorStoreService.SearchAsync(
            query, 
            activeCurriculum.Id, 
            embeddingService, 
            topK, 
            minSimilarity: 0.25f, 
            ct);

        if (chunks.Count == 0)
        {
            // Fallback: if no chunks found, might need to index
            return await GetFallbackContentAsync(activeCurriculum);
        }

        // Build context from retrieved chunks
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"=== RELEVANT CONTENT FROM: {activeCurriculum.Name} ===");
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
        sb.AppendLine("If the answer is not in the above content, say the topic is not covered in the current curriculum.");
        
        return sb.ToString();
    }

    /// <summary>
    /// Fallback for when RAG hasn't been indexed yet.
    /// Returns truncated content to avoid huge payloads.
    /// </summary>
    private async Task<string> GetFallbackContentAsync(Curriculum curriculum)
    {
        var resources = await GetCurriculumResourcesAsync(curriculum.Id);
        if (resources.Count == 0)
            return "";

        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"=== CURRICULUM: {curriculum.Name} (FALLBACK - NOT INDEXED) ===");
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
}
