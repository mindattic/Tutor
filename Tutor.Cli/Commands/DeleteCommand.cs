using Tutor.Core.Services.Packaging;

namespace Tutor.Cli.Commands;

/// <summary>
/// <c>tutor delete &lt;course-id&gt; [--dry-run]</c> — the explicit hard remove:
/// cascades resources, embeddings, concept maps, structures, the per-course
/// ConceptMapCollection file, the installed-registry row, and the retained
/// bundle blob. Prefer unloading (soft-disable) from the in-app course library
/// when the goal is just to hide a course.
/// </summary>
public sealed class DeleteCommand
{
    private readonly CourseInstallService installService;
    private readonly CourseDeleteService deleteService;

    public DeleteCommand(CourseInstallService installService, CourseDeleteService deleteService)
    {
        this.installService = installService;
        this.deleteService = deleteService;
    }

    /// <summary>
    /// Returns 0 on success / dry-run, 1 if the course is not found, 64 on usage errors.
    /// </summary>
    public async Task<int> RunAsync(string[] args, CancellationToken ct = default)
    {
        var (positionals, options) = Args.Parse(args);
        if (positionals.Count == 0)
        {
            Console.Error.WriteLine("Usage: tutor delete <course-id> [--dry-run]");
            return 64;
        }

        var courseId = positionals[0];
        var dryRun = options.ContainsKey("dry-run");

        var plan = await deleteService.PlanAsync(courseId, ct);
        if (plan == null)
        {
            Console.Error.WriteLine($"Course '{courseId}' not found.");
            return 1;
        }

        Console.WriteLine($"{(dryRun ? "[dry-run] Would delete" : "Deleting")} course '{plan.CourseName}' ({plan.CourseId})");
        Console.WriteLine($"  resources:                {plan.ResourceCount}");
        Console.WriteLine($"  concept maps:             {plan.ConceptMapCount}");
        Console.WriteLine($"  course structure:         {(plan.HasStructure ? "yes" : "no")}");
        Console.WriteLine($"  concept map collection:   {(plan.HasConceptMapCollection ? "yes" : "no")}");

        if (dryRun) return 0;

        await installService.RemoveAsync(courseId, ct);

        Console.WriteLine("Done.");
        return 0;
    }
}
