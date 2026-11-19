using MindAttic.Authentication;

namespace Tutor.Shared.Services;

/// <summary>Tutor's own authorization policies, registered by the web host next to the library's <see cref="MaPolicies"/>.</summary>
public static class TutorPolicies
{
    /// <summary>
    /// May create, change and spend on course content: /courses, /instructions, /knowledge-graph, Auto-Link
    /// Orphans and chat-driven concept-map expansion (TUT-T59, plan item 0.2(a)). Admin only to start.
    /// </summary>
    public const string Author = "Tutor.Author";

    /// <summary>The roles that satisfy <see cref="Author"/>. Add a role here (e.g. "Teacher") to widen authoring.</summary>
    public static readonly IReadOnlyList<string> AuthorRoles = [MaRoles.Admin];
}
