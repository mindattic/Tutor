using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Tutor.Core.Models;
using Tutor.Core.Services;

namespace Tutor.Shared.Services;

/// <summary>
/// Course authoring - the actions that change shared course content or spend on it (course and resource
/// edits, builds, the knowledge graph, global instructions, Auto-Link Orphans, chat-driven concept expansion)
/// - each authorized on the server against <see cref="TutorPolicies.Author"/> before it does anything
/// (TUT-T59, plan item 0.2(a)). Pages hide these controls from non-authors, but hiding is presentation: a
/// Blazor Server event handler runs with whatever principal the circuit carries, so the check lives here,
/// the way <see cref="LibraryAdminActions"/> guards the library.
/// </summary>
public sealed class CourseAuthoringActions
{
    private readonly IAuthorizationService authorization;
    private readonly OrphanConceptLinkerService orphanLinker;
    private readonly ConceptMapStorageService conceptMapStorage;
    private readonly TutorSettingsSource settings;

    public CourseAuthoringActions(
        IAuthorizationService authorization,
        OrphanConceptLinkerService orphanLinker,
        ConceptMapStorageService conceptMapStorage,
        TutorSettingsSource settings)
    {
        this.authorization = authorization;
        this.orphanLinker = orphanLinker;
        this.conceptMapStorage = conceptMapStorage;
        this.settings = settings;
    }

    /// <summary>The message shown when a non-author reaches an authoring action.</summary>
    public const string DeniedMessage = "Only a course author can change course content.";

    /// <summary>True when <paramref name="user"/> satisfies the Author policy.</summary>
    public async Task<bool> IsAuthorAsync(ClaimsPrincipal? user) =>
        user is not null && (await authorization.AuthorizeAsync(user, resource: null, TutorPolicies.Author)).Succeeded;

    /// <summary>Throws <see cref="UnauthorizedAccessException"/> unless <paramref name="user"/> is an author.</summary>
    public async Task EnsureAuthorAsync(ClaimsPrincipal? user)
    {
        if (!await IsAuthorAsync(user))
            throw new UnauthorizedAccessException(DeniedMessage);
    }

    /// <summary>
    /// Whether a chat message from <paramref name="user"/> may expand the SHARED course concept maps (plan item
    /// 0.2(c)). Only authors: a student's question must never write into the maps every learner sees.
    /// </summary>
    public Task<bool> CanExpandSharedConceptMapsAsync(ClaimsPrincipal? user) => IsAuthorAsync(user);

    /// <summary>
    /// Auto-Link Orphans: AI passes that add relations to orphaned concepts in each map, saving every map that
    /// changed. Passes and confidence come from Tutor's settings. Returns the number of links added.
    /// </summary>
    public async Task<int> LinkOrphansAsync(ClaimsPrincipal? user, IEnumerable<ConceptMap> maps,
        Action<ConceptMap>? onMapStarted = null, CancellationToken ct = default)
    {
        await EnsureAuthorAsync(user);
        var s = settings.Current;
        var applied = 0;
        foreach (var map in maps)
        {
            ct.ThrowIfCancellationRequested();
            onMapStarted?.Invoke(map);
            var result = await orphanLinker.LinkAllOrphansIterativelyAsync(
                map, s.OrphanLinkingMaxIterations, (float)s.OrphanLinkingMinConfidence, ct);
            if (result.AppliedLinkCount > 0)
            {
                await conceptMapStorage.SaveAsync(map);
                applied += result.AppliedLinkCount;
            }
        }
        return applied;
    }
}
