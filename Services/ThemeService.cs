using Microsoft.Maui.Storage;

public sealed class ThemeService
{
    private const string ThemeKey = "APP_THEME";
    
    public static readonly string[] AvailableThemes = ["Light", "Dark", "Summer", "Winter", "Autumn", "Spring"];
    
    public string CurrentTheme { get; private set; } = "Light";

    public async Task InitializeAsync()
    {
        CurrentTheme = await GetThemeAsync();
    }

    public async Task<string> GetThemeAsync()
    {
        try
        {
            var theme = await SecureStorage.GetAsync(ThemeKey);
            return string.IsNullOrEmpty(theme) ? "Light" : theme;
        }
        catch
        {
            return "Light";
        }
    }

    public async Task SetThemeAsync(string theme)
    {
        if (!AvailableThemes.Contains(theme))
            theme = "Light";
            
        try
        {
            await SecureStorage.SetAsync(ThemeKey, theme);
            CurrentTheme = theme;
        }
        catch
        {
            // Ignore errors
        }
    }
}
