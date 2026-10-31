using Tutor.Core.Services;

/// <summary>
/// The color theme in effect: <see cref="TutorSettings.Theme"/>, edited in MindAttic.Vault.Desktop (LAMP, TUT-T31).
/// </summary>
public sealed class ThemeService
{
    private readonly TutorSettingsSource settings;

    public static readonly string[] AvailableThemes = ["Light", "Dark", "Summer", "Winter", "Autumn", "Spring"];

    public string CurrentTheme { get; private set; } = "Light";

    public ThemeService(TutorSettingsSource settings)
    {
        this.settings = settings;
    }

    public async Task InitializeAsync()
    {
        CurrentTheme = await GetThemeAsync();
    }

    public Task<string> GetThemeAsync()
    {
        try
        {
            var theme = settings.Current.Theme;
            return Task.FromResult(string.IsNullOrEmpty(theme) ? "Light" : theme);
        }
        catch
        {
            return Task.FromResult("Light");
        }
    }
}
