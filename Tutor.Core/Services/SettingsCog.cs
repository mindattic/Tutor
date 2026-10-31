using MindAttic.Vault.Launch;

namespace Tutor.Core.Services;

/// <summary>
/// What Tutor's Settings cog does (LAMP, TUT-T31): opens MindAttic.Vault.Desktop at the Tutor tab,
/// scrolled to one setting when given. Tutor has no settings editor of its own. Called server-side
/// (Tutor.Blazor runs on the user's machine); <see cref="Link"/> is the fallback a remote browser can use.
/// </summary>
public static class SettingsCog
{
    /// <summary>The outcome; <see cref="Message"/> is what to show the user.</summary>
    public sealed record Result(bool Opened, string Message);

    /// <summary>Opens Vault.Desktop at the Tutor tab (or one setting's row, by its full name).</summary>
    public static Result Open(string? setting = null)
    {
        if (VaultSettings.Open(TutorSettings.AppId, setting))
            return new Result(true, "Opened Tutor's settings in MindAttic Vault.");
        return new Result(false,
            "MindAttic Vault isn't installed or couldn't start, so settings can't be opened here. Install " +
            "MindAttic.Vault.Desktop (or set MINDATTIC_VAULT_DESKTOP_EXE), or use the Vault link.");
    }

    /// <summary>The <c>mindattic-vault://</c> link to Tutor's tab, for a browser that isn't on the server's machine.</summary>
    public static string Link(string? setting = null) => VaultSettings.Link(TutorSettings.AppId, setting);

    /// <summary>A setting's full name, e.g. <c>MindAttic:Settings:Tutor:theme</c>.</summary>
    public static string FullName(string property) =>
        $"{MindAttic.Vault.Settings.Manifest.SettingsManifest.PrefixFor(TutorSettings.AppId)}:{char.ToLowerInvariant(property[0])}{property[1..]}";
}
