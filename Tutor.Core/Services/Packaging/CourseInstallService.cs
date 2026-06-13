using Tutor.Core.Services.Logging;

namespace Tutor.Core.Services.Packaging;

/// <summary>
/// The one code path both front doors (CLI and Blazor) use for the course
/// lifecycle: inspect → plan → install (registry row + verbatim blob retained),
/// load/unload (soft enable/disable), and remove (hard cascade + registry row +
/// blob). Wraps <see cref="BundleImporter"/>/<see cref="CourseDeleteService"/>
/// with the identity, integrity, and registry layers.
/// </summary>
public sealed class CourseInstallService
{
    private readonly BundleImporter importer;
    private readonly CourseDeleteService deleteService;
    private readonly InstalledCourseRegistry registry;
    private readonly CourseBlobStore blobStore;

    public CourseInstallService(
        BundleImporter importer,
        CourseDeleteService deleteService,
        InstalledCourseRegistry registry,
        CourseBlobStore blobStore)
    {
        this.importer = importer;
        this.deleteService = deleteService;
        this.registry = registry;
        this.blobStore = blobStore;
    }

    /// <summary>
    /// Validates the bundle and resolves the install plan against the registry
    /// without changing anything — what the UI shows before asking to confirm.
    /// </summary>
    public async Task<CourseInstallPreview> PreviewAsync(string bundlePath, bool allowDuplicate = false, CancellationToken ct = default)
    {
        var inspection = await importer.InspectAsync(bundlePath, ct);

        InstallPlan? plan = null;
        if (inspection.Validation.IsValid && inspection.Manifest != null)
        {
            var installed = await registry.GetAllAsync(ct);
            plan = CourseInstallResolver.Plan(
                inspection.Manifest.CourseKey, inspection.Manifest.CourseVersion, installed, allowDuplicate);
        }

        return new CourseInstallPreview(inspection.Manifest, inspection.Validation, plan);
    }

    /// <summary>
    /// Full install: validate → plan → import → record registry row → retain the
    /// verbatim bundle. Returns the outcome including plans that decided not to
    /// install (already installed, downgrade refused).
    /// </summary>
    public async Task<CourseInstallOutcome> InstallAsync(
        string bundlePath,
        string? overrideCourseName = null,
        bool allowDuplicate = false,
        CancellationToken ct = default)
    {
        var preview = await PreviewAsync(bundlePath, allowDuplicate, ct);

        if (!preview.Validation.IsValid || preview.Manifest == null || preview.Plan == null)
            return new CourseInstallOutcome(preview, null, null);

        if (!preview.Plan.ShouldInstall)
            return new CourseInstallOutcome(preview, null, null);

        // Side-by-side installs of the same key collide on the course *name*;
        // the importer's duplicate-name guard is relaxed for those on purpose.
        var importAllowsDuplicate = allowDuplicate || preview.Plan.Kind == InstallPlanKind.Upgrade;
        var result = await importer.ImportAsync(bundlePath, overrideCourseName, importAllowsDuplicate, ct);

        var blobPath = blobStore.Save(bundlePath, result.CourseKey, result.CourseVersion);

        var row = new InstalledCourse
        {
            CourseId = result.Course.Id,
            CourseKey = result.CourseKey,
            CourseVersion = result.CourseVersion,
            Name = result.Course.Name,
            Sha256 = result.Sha256,
            BlobPath = blobPath,
            Enabled = true,
            InstalledUtc = DateTime.UtcNow,
        };
        await registry.RecordInstallAsync(row, ct);

        Log.Info($"CourseInstallService: installed '{row.CourseKey}' v{row.CourseVersion} as course {row.CourseId} ({preview.Plan.Kind})");
        return new CourseInstallOutcome(preview, result, row);
    }

    /// <summary>Load = re-enable an unloaded course. Returns false when not registered.</summary>
    public Task<bool> LoadAsync(string courseId, CancellationToken ct = default) =>
        registry.SetEnabledAsync(courseId, true, ct);

    /// <summary>
    /// Unload = hide the course from the learning view without destroying its
    /// data or any learner's progress. Returns false when not registered.
    /// </summary>
    public Task<bool> UnloadAsync(string courseId, CancellationToken ct = default) =>
        registry.SetEnabledAsync(courseId, false, ct);

    /// <summary>
    /// The explicit hard remove: cascades the data delete, drops the registry
    /// row, and deletes the retained blob. Returns null when the course does not
    /// exist in storage or the registry.
    /// </summary>
    public async Task<CourseDeletePlan?> RemoveAsync(string courseId, CancellationToken ct = default)
    {
        var plan = await deleteService.DeleteAsync(courseId, ct);

        var row = await registry.RemoveAsync(courseId, ct);
        if (row != null)
        {
            try { blobStore.Delete(row.CourseKey, row.CourseVersion); }
            catch (Exception ex) { Log.Warn($"CourseInstallService: could not delete retained blob - {ex.Message}"); }
        }

        return plan ?? (row != null
            ? new CourseDeletePlan(courseId, row.Name, 0, 0, false, false)
            : null);
    }
}

/// <summary>Validation + plan shown to a user before they confirm an install.</summary>
public sealed record CourseInstallPreview(
    BundleManifest? Manifest,
    ValidationResult Validation,
    InstallPlan? Plan);

/// <summary>
/// What an install attempt did. <see cref="Result"/> and <see cref="Installed"/>
/// are null when validation failed or the plan decided not to install — the
/// preview carries the reason either way.
/// </summary>
public sealed record CourseInstallOutcome(
    CourseInstallPreview Preview,
    ImportBundleResult? Result,
    InstalledCourse? Installed)
{
    public bool DidInstall => Installed != null;
}
