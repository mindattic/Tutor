using System.Text.Json;
using Tutor.Core.Models;
using Tutor.Core.Services.Abstractions;

namespace Tutor.Core.Services;

/// <summary>
/// Service for managing core concepts that the student has learned during lessons.
/// Persists concepts per-user using SecureStorage.
/// </summary>
public class CoreConceptService
{
    private const string ConceptsKeyPrefix = "CORE_CONCEPTS_";
    private readonly ISecurePreferences securePreferences;
    private readonly List<CoreConcept> concepts = new();
    private string currentUserId = "default_user";

    public CoreConceptService(ISecurePreferences securePreferences)
    {
        this.securePreferences = securePreferences;
    }

    public event Action? OnConceptsChanged;

    public IReadOnlyList<CoreConcept> GetConcepts() => concepts.AsReadOnly();

    public async Task LoadForUserAsync(string userId)
    {
        currentUserId = string.IsNullOrEmpty(userId) ? "default_user" : userId;
        try
        {
            var json = await securePreferences.GetAsync(ConceptsKeyPrefix + currentUserId);
            if (string.IsNullOrEmpty(json))
            {
                concepts.Clear();
                return;
            }

            var list = JsonSerializer.Deserialize<List<CoreConcept>>(json);
            concepts.Clear();
            if (list != null)
                concepts.AddRange(list);
        }
        catch
        {
            concepts.Clear();
        }

        OnConceptsChanged?.Invoke();
    }

    private async Task SaveForUserAsync()
    {
        try
        {
            var json = JsonSerializer.Serialize(concepts);
            await securePreferences.SetAsync(ConceptsKeyPrefix + currentUserId, json);
        }
        catch { }
    }

    public async Task<bool> AddConceptAsync(string term, string description)
    {
        if (string.IsNullOrWhiteSpace(term) || string.IsNullOrWhiteSpace(description))
            return false;

        if (concepts.Any(c => c.Term.Equals(term, StringComparison.OrdinalIgnoreCase)))
            return false;

        concepts.Add(new CoreConcept
        {
            Term = term.Trim(),
            Description = description.Trim(),
            AddedAt = DateTime.Now
        });

        await SaveForUserAsync();
        OnConceptsChanged?.Invoke();
        return true;
    }

    public async Task<bool> UpdateConceptAsync(string term, string newDescription)
    {
        var index = concepts.FindIndex(c => c.Term.Equals(term, StringComparison.OrdinalIgnoreCase));
        if (index < 0) return false;

        concepts[index] = concepts[index] with { Description = newDescription.Trim() };
        await SaveForUserAsync();
        OnConceptsChanged?.Invoke();
        return true;
    }

    public async Task<bool> RemoveConceptAsync(string term)
    {
        var removed = concepts.RemoveAll(c => c.Term.Equals(term, StringComparison.OrdinalIgnoreCase)) > 0;
        if (removed)
        {
            await SaveForUserAsync();
            OnConceptsChanged?.Invoke();
        }
        return removed;
    }

    public CoreConcept? GetConcept(string term)
    {
        return concepts.FirstOrDefault(c => c.Term.Equals(term, StringComparison.OrdinalIgnoreCase));
    }

    public async Task ClearAllAsync()
    {
        concepts.Clear();
        try { securePreferences.Remove(ConceptsKeyPrefix + currentUserId); } catch { }
        OnConceptsChanged?.Invoke();
        await Task.CompletedTask;
    }

    public int Count => concepts.Count;
}
