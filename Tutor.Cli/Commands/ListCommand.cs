using Tutor.Core.Services;

namespace Tutor.Cli.Commands;

/// <summary>
/// <c>tutor list</c> — prints every course on this machine with its id, name,
/// and resource count.
/// </summary>
public sealed class ListCommand
{
    private readonly CourseService courseService;

    public ListCommand(CourseService courseService)
    {
        this.courseService = courseService;
    }

    /// <summary>Always returns 0; takes no arguments.</summary>
    public async Task<int> RunAsync(string[] args, CancellationToken ct = default)
    {
        var courses = await courseService.GetAllCoursesAsync();
        if (courses.Count == 0)
        {
            Console.WriteLine("(no courses)");
            return 0;
        }

        Console.WriteLine($"{courses.Count} course(s):");
        foreach (var c in courses.OrderBy(c => c.Name))
        {
            Console.WriteLine($"  {c.Id}  {c.Name}  ({c.ResourceIds.Count} resources)");
        }
        return 0;
    }
}
