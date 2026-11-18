namespace Tutor.Core.Services;

/// <summary>
/// The one place Tutor's on-disk data root is decided: <c>%LOCALAPPDATA%\Tutor</c> on a dev machine, or
/// the directory named by the <c>TUTOR_DATA_DIR</c> environment variable (TUT-T47).
/// <para>
/// A deployed host must set the variable: on Azure App Service <c>%LOCALAPPDATA%</c> is
/// <c>C:\local\LocalAppData</c>, which is wiped whenever the site moves or restarts, while
/// <c>C:\home</c> persists (so <c>TUTOR_DATA_DIR=C:\home\data\Tutor</c>).
/// </para>
/// </summary>
public static class TutorDataRoot
{
    /// <summary>Environment variable that overrides the data root, used verbatim.</summary>
    public const string EnvironmentVariable = "TUTOR_DATA_DIR";

    /// <summary>The override, or null when the variable is unset or blank.</summary>
    public static string? Override
    {
        get
        {
            var value = Environment.GetEnvironmentVariable(EnvironmentVariable);
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }
    }

    /// <summary>The data root in effect.</summary>
    public static string Path => Override
        ?? System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Tutor");
}