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
    private readonly ISecurePreferences _securePreferences;
    private readonly List<CoreConcept> _concepts = new();
    private string currentUserId = "default_user";

    public CoreConceptService(ISecurePreferences securePreferences)
    {
        _securePreferences = securePreferences;
    }

    public event Action? OnConceptsChanged;

    public IReadOnlyList<CoreConcept> GetConcepts() => _concepts.AsReadOnly();

    public async Task LoadForUserAsync(string userId)
    {
        currentUserId = string.IsNullOrEmpty(userId) ? "default_user" : userId;
        try
        {
            var json = await _securePreferences.GetAsync(ConceptsKeyPrefix + currentUserId);
            if (string.IsNullOrEmpty(json))
            {
                _concepts.Clear();
                return;
            }

            var list = JsonSerializer.Deserialize<List<CoreConcept>>(json);
            _concepts.Clear();
            if (list != null)
                _concepts.AddRange(list);
        }
        catch
        {
            _concepts.Clear();
        }

        OnConceptsChanged?.Invoke();
    }

    private async Task SaveForUserAsync()
    {
        try
        {
            var json = JsonSerializer.Serialize(_concepts);
            await _securePreferences.SetAsync(ConceptsKeyPrefix + currentUserId, json);
        }
        catch { }
    }

    public async Task<bool> AddConceptAsync(string term, string description)
    {
        if (string.IsNullOrWhiteSpace(term) || string.IsNullOrWhiteSpace(description))
            return false;

        if (_concepts.Any(c => c.Term.Equals(term, StringComparison.OrdinalIgnoreCase)))
            return false;

        _concepts.Add(new CoreConcept
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
        var index = _concepts.FindIndex(c => c.Term.Equals(term, StringComparison.OrdinalIgnoreCase));
        if (index < 0) return false;

        _concepts[index] = _concepts[index] with { Description = newDescription.Trim() };
        await SaveForUserAsync();
        OnConceptsChanged?.Invoke();
        return true;
    }

    public async Task<bool> RemoveConceptAsync(string term)
    {
        var removed = _concepts.RemoveAll(c => c.Term.Equals(term, StringComparison.OrdinalIgnoreCase)) > 0;
        if (removed)
        {
            await SaveForUserAsync();
            OnConceptsChanged?.Invoke();
        }
        return removed;
    }

    public CoreConcept? GetConcept(string term)
    {
        return _concepts.FirstOrDefault(c => c.Term.Equals(term, StringComparison.OrdinalIgnoreCase));
    }

    public async Task ClearAllAsync()
    {
        _concepts.Clear();
        try { _securePreferences.Remove(ConceptsKeyPrefix + currentUserId); } catch { }
        OnConceptsChanged?.Invoke();
        await Task.CompletedTask;
    }

    public int Count => _concepts.Count;
}
