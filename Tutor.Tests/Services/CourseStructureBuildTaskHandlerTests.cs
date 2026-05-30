using System.Text.Json;
using Tutor.Core.Services.Queue;

namespace Tutor.Tests.Services;

public class CourseStructureBuildTaskHandlerTests
{
    private static BackgroundQueueItem ItemWith(string payloadJson) => new()
    {
        TaskType = BackgroundTaskType.CourseStructureBuild,
        PayloadJson = payloadJson
    };

    [Test]
    public void TaskType_IsCourseStructureBuild()
    {
        Assert.That(new CourseStructureBuildTaskHandler().TaskType,
            Is.EqualTo(BackgroundTaskType.CourseStructureBuild));
    }

    [Test]
    public void Validate_ValidPayload_IsValid()
    {
        var handler = new CourseStructureBuildTaskHandler();
        var payload = JsonSerializer.Serialize(new CourseStructureBuildPayload
        {
            CourseId = "c1",
            ConceptMapCollectionId = "collection_c1",
            Name = "Course"
        });

        Assert.That(handler.Validate(ItemWith(payload)).IsValid, Is.True);
    }

    [Test]
    public void Validate_MissingCourseId_IsInvalid()
    {
        var handler = new CourseStructureBuildTaskHandler();
        var payload = JsonSerializer.Serialize(new CourseStructureBuildPayload
        {
            CourseId = "",
            ConceptMapCollectionId = "collection_c1"
        });

        Assert.That(handler.Validate(ItemWith(payload)).IsValid, Is.False);
    }

    [Test]
    public void Validate_GarbageJson_IsInvalid()
    {
        var handler = new CourseStructureBuildTaskHandler();
        Assert.That(handler.Validate(ItemWith("}{ not json")).IsValid, Is.False);
    }
}
