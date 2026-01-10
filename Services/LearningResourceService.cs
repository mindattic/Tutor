using System.Text.Json;
using Tutor.Models;

namespace Tutor.Services;

public sealed class LearningResourceService
{
    private const string LearningResourcesKey = "LEARNING_RESOURCES_DATA";
    private const string CurriculaKey = "CURRICULA_DATA";
    private const string ActiveCurriculumKey = "ACTIVE_CURRICULUM";

    private List<LearningResource>? cachedLearningResources;
    private List<Curriculum>? cachedCurricula;

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

    // Get combined content for chat context
    public async Task<string> GetActiveCurriculumContentAsync()
    {
        var activeCurriculum = await GetActiveCurriculumAsync();
        if (activeCurriculum == null)
            return "";

        var resources = await GetCurriculumResourcesAsync(activeCurriculum.Id);
        if (resources.Count == 0)
            return "";

        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"=== CURRICULUM: {activeCurriculum.Name} ===");
        sb.AppendLine("You must ONLY answer questions based on the following resources. If the answer is not contained in these resources, politely explain that the topic is not covered in the current curriculum.");
        sb.AppendLine();

        foreach (var resource in resources)
        {
            sb.AppendLine($"--- {resource.Title} ---");
            if (!string.IsNullOrEmpty(resource.Author))
                sb.AppendLine($"Author: {resource.Author}");
            if (!string.IsNullOrEmpty(resource.Year))
                sb.AppendLine($"Year: {resource.Year}");
            sb.AppendLine();
            sb.AppendLine(resource.Content);
            sb.AppendLine();
        }

        sb.AppendLine("=== END CURRICULUM ===");
        return sb.ToString();
    }
}
