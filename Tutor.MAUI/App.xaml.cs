using Tutor.Core.Services;
using Tutor.Core.Services.Logging;
using Tutor.Core.Services.Queue;

namespace Tutor.MAUI
{
    /// <summary>
    /// MAUI application shell for the Tutor platform. Handles application lifecycle events
    /// including initializing the logging subsystem, loading log-level settings, bootstrapping
    /// the background task queue on startup, and persisting queue state when the app is suspended.
    /// </summary>
    public partial class App : Application
    {
        private readonly LogStorageService _logStorageService;
        private readonly SettingsService _settingsService;
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes the application, loading persisted logs, log-level settings,
        /// and bootstrapping the background queue service for async task processing.
        /// </summary>
        public App(
            LogStorageService logStorageService,
            SettingsService settingsService,
            IServiceProvider serviceProvider)
        {
            _logStorageService = logStorageService;
            _settingsService = settingsService;
            _serviceProvider = serviceProvider;
            InitializeComponent();

            // Initialize logging system (loads persisted logs)
            _logStorageService.Initialize();

            // Load log level settings
            _ = _settingsService.LoadLogSettingsAsync();

            // Initialize background queue service (static singleton)
            _ = BackgroundQueueService.InitializeAsync(_serviceProvider);
        }

        /// <summary>Creates the application window with the main Blazor WebView page.</summary>
        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new MainPage()) { Title = "Tutor" };
        }

        /// <summary>Persists background queue state when the app enters the background.</summary>
        protected override void OnSleep()
        {
            base.OnSleep();
            // Save queue state when app goes to background
            _ = BackgroundQueueService.SaveStateAsync();
        }
    }
}
