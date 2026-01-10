using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection; 
using System.Net.Http;
using Tutor.Services;

namespace Tutor
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();


            // Register OpenAI configuration and service
            builder.Services.AddSingleton(new OpenAIOptions
            {
                // Change this if you want a different model
                Model = "gpt-4.1-mini"
            });

            // HttpClient + OpenAI service
            builder.Services.AddHttpClient<OpenAIService>();

            // Instructions service
            builder.Services.AddSingleton<InstructionsService>();

            // Settings service
            builder.Services.AddSingleton<SettingsService>();

            // Theme service
            builder.Services.AddSingleton<ThemeService>();

            // Learning Resource service
            builder.Services.AddSingleton<Tutor.Services.LearningResourceService>();

            // Core Concept service
            builder.Services.AddSingleton<CoreConceptService>();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
