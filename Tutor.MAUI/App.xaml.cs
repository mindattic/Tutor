using Tutor.Core.Services;
using Tutor.Core.Services.Logging;
using Tutor.Core.Services.Queue;

namespace Tutor.MAUI
{
    public partial class App : Application
    {
        private readonly LogStorageService _logStorageService;
        private readonly SettingsService _settingsService;
        private readonly IServiceProvider _serviceProvider;

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

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new MainPage()) { Title = "Tutor" };
        }

        protected override void OnSleep()
        {
            base.OnSleep();
            // Save queue state when app goes to background
            _ = BackgroundQueueService.SaveStateAsync();
        }
    }
}
