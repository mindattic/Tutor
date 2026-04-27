using Tutor.Core.Services;

namespace Tutor.Cli.Commands;

public sealed class ListCommand
{
    private readonly CourseService courseService;

    public ListCommand(CourseService courseService)
    {
        this.courseService = courseService;
    }

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
