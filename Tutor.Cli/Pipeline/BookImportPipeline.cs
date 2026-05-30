using Tutor.Core.Models;

namespace Tutor.Cli.Pipeline;

/// <summary>
/// Single-resource convenience wrapper over <see cref="CourseBuildPipeline"/>: one
/// book/file becomes a one-item course. Kept as its own type so the
/// <c>gutenberg</c>/<c>import</c>/<c>gutenberg-top10</c> commands read naturally.
/// </summary>
public sealed class BookImportPipeline
{
    private readonly CourseBuildPipeline builder;

    public BookImportPipeline(CourseBuildPipeline builder)
    {
        this.builder = builder;
    }

    /// <summary>
    /// Builds a one-resource course. Throws <see cref="InvalidOperationException"/>
    /// when a course of the same name already exists and
    /// <see cref="BookImportRequest.AllowDuplicate"/> is false.
    /// </summary>
    public async Task<ImportResult> ImportAsync(BookImportRequest req, CancellationToken ct = default)
    {
        var buildRequest = new CourseBuildRequest(
            CourseName: req.CourseName,
            CourseDescription: req.CourseDescription,
            Items: new[]
            {
                new CourseItem(
                    Title: req.ResourceTitle,
                    Author: req.Author,
                    Description: req.ResourceDescription,
                    FileName: req.FileName,
                    Content: req.Content),
            },
            QuizMode: req.QuizMode,
            AllowDuplicate: req.AllowDuplicate);

        var result = await builder.BuildAsync(buildRequest, ct);
        return new ImportResult(result.Course, result.Resources[0], result.Structure, result.Elapsed);
    }
}

/// <summary>
/// Inputs for <see cref="BookImportPipeline.ImportAsync"/>. <paramref name="AllowDuplicate"/>
/// suppresses the same-name guard for cases like distinct printings of the same book.
/// </summary>
public sealed record BookImportRequest(
    string CourseName,
    string CourseDescription,
    string ResourceTitle,
    string Author,
    string ResourceDescription,
    string FileName,
    string Content,
    QuizMode QuizMode = QuizMode.Both,
    bool AllowDuplicate = false);

/// <summary>
/// Output of <see cref="BookImportPipeline.ImportAsync"/> — the persisted course,
/// its single resource, the generated structure, and total wall-clock duration.
/// </summary>
public sealed record ImportResult(
    Course Course,
    CourseResource Resource,
    CourseStructure Structure,
    TimeSpan Elapsed);
