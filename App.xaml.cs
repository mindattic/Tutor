using Tutor.Services.Logging;

namespace Tutor
{
    public partial class App : Application
    {
        private readonly LogStorageService _logStorageService;
        private readonly SettingsService _settingsService;

        public App(LogStorageService logStorageService, SettingsService settingsService)
        {
            _logStorageService = logStorageService;
            _settingsService = settingsService;
            InitializeComponent();
            
            // Initialize logging system (loads persisted logs)
            _logStorageService.Initialize();
            
            // Load log level settings
            _ = _settingsService.LoadLogSettingsAsync();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new MainPage()) { Title = "Tutor" };
        }
    }
}
