using System.Text.Json;
using Tutor.Core.Services;
using Tutor.Core.Services.Abstractions;
using Tutor.Core.Services.Logging;

/// <summary>
/// Read-only access to Tutor's app-level settings (LAMP, TUT-T31): every value comes from
/// <see cref="TutorSettings"/> (edited only in MindAttic.Vault.Desktop), re-read on each call so a
/// change saved in Vault applies on the next read. Lesson state is per-user run state and stays in
/// <see cref="ISecurePreferences"/>.
/// </summary>
public sealed class SettingsService
{
    private const string LessonStateKey = "LESSON_STATE";

    private readonly TutorSettingsSource settings;
    private readonly ISecurePreferences securePreferences;

    public SettingsService(TutorSettingsSource settings, ISecurePreferences securePreferences)
    {
        this.settings = settings;
        this.securePreferences = securePreferences;
    }

    private TutorSettings S => settings.Current;

    /// <summary>
    /// Current date format pattern. Updated when settings are loaded.
    /// </summary>
    public static string CurrentDateFormat { get; private set; } = TutorSettings.DefaultDateFormat;

    /// <summary>
    /// Formats a DateTime using the current date format setting.
    /// </summary>
    public static string FormatDate(DateTime dateTime)
    {
        return dateTime.ToString(CurrentDateFormat);
    }

    public Task<string> GetDateFormatAsync()
    {
        var format = string.IsNullOrEmpty(S.DateFormat) ? TutorSettings.DefaultDateFormat : S.DateFormat;
        CurrentDateFormat = format;
        return Task.FromResult(format);
    }

    public Task<bool> GetLogTraceEnabledAsync() => Task.FromResult(S.LogTrace);
    public Task<bool> GetLogDebugEnabledAsync() => Task.FromResult(S.LogDebug);
    public Task<bool> GetLogInfoEnabledAsync() => Task.FromResult(S.LogInfo);
    public Task<bool> GetLogWarningEnabledAsync() => Task.FromResult(S.LogWarning);
    public Task<bool> GetLogErrorEnabledAsync() => Task.FromResult(S.LogError);
    public Task<bool> GetLogCriticalEnabledAsync() => Task.FromResult(S.LogCritical);

    /// <summary>
    /// Loads all log level settings and applies them to Log.Settings.
    /// Call this on app startup.
    /// </summary>
    public async Task LoadLogSettingsAsync()
    {
        var s = S;
        Log.Settings.LogTrace = s.LogTrace;
        Log.Settings.LogDebug = s.LogDebug;
        Log.Settings.LogInfo = s.LogInfo;
        Log.Settings.LogWarning = s.LogWarning;
        Log.Settings.LogError = s.LogError;
        Log.Settings.LogCritical = s.LogCritical;

        // Also load date format
        await GetDateFormatAsync();
    }

    /// <summary>Border radius in px, clamped to 0-10 (default 6).</summary>
    public Task<int> GetBorderRadiusAsync() => Task.FromResult(Math.Clamp(S.BorderRadius, 0, 10));

    /// <summary>Paragraph spacing in em, clamped to 0.5-2.0 (default 1.0).</summary>
    public Task<double> GetParagraphSpacingAsync() => Task.FromResult(Math.Clamp(S.ParagraphSpacing, 0.5, 2.0));

    public Task<bool> GetEnterToSendAsync() => Task.FromResult(S.EnterToSend);

    public Task<int> GetQuizQuestionCountAsync() => Task.FromResult(S.QuizQuestionCount);

    public Task<GradingScale> GetGradingScaleAsync() =>
        Task.FromResult(Enum.IsDefined(S.GradingScale) ? S.GradingScale : GradingScale.Standard);

    public string GetGradeForScore(int correctAnswers, int totalQuestions, GradingScale scale)
    {
        if (totalQuestions == 0) return "N/A";
        
        var percentage = (double)correctAnswers / totalQuestions * 100;
        
        return scale switch
        {
            GradingScale.Standard => GetStandardGrade(percentage),
            GradingScale.PlusMinus => GetPlusMinusGrade(percentage),
            GradingScale.Lenient => GetLenientGrade(percentage),
            GradingScale.Strict => GetStrictGrade(percentage),
            _ => GetStandardGrade(percentage)
        };
    }

    private string GetStandardGrade(double percentage) => percentage switch
    {
        >= 90 => "A",
        >= 80 => "B",
        >= 70 => "C",
        >= 60 => "D",
        _ => "F"
    };

    private string GetPlusMinusGrade(double percentage) => percentage switch
    {
        >= 97 => "A+",
        >= 93 => "A",
        >= 90 => "A-",
        >= 87 => "B+",
        >= 83 => "B",
        >= 80 => "B-",
        >= 77 => "C+",
        >= 73 => "C",
        >= 70 => "C-",
        >= 67 => "D+",
        >= 63 => "D",
        >= 60 => "D-",
        _ => "F"
    };

    private string GetLenientGrade(double percentage) => percentage switch
    {
        >= 85 => "A",
        >= 75 => "B",
        >= 65 => "C",
        >= 55 => "D",
        _ => "F"
    };

    private string GetStrictGrade(double percentage) => percentage switch
    {
        >= 95 => "A",
        >= 85 => "B",
        >= 75 => "C",
        >= 65 => "D",
        _ => "F"
    };

    // Lesson state persistence
    public async Task<LessonState?> GetLessonStateAsync()
    {
        try
        {
            var json = await securePreferences.GetAsync(LessonStateKey);
            if (string.IsNullOrEmpty(json)) return null;
            return JsonSerializer.Deserialize<LessonState>(json);
        }
        catch
        {
            return null;
        }
    }

    public async Task SaveLessonStateAsync(LessonState? state)
    {
        try
        {
            if (state == null)
            {
                securePreferences.Remove(LessonStateKey);
            }
            else
            {
                var json = JsonSerializer.Serialize(state);
                await securePreferences.SetAsync(LessonStateKey, json);
            }
        }
        catch
        {
            // Ignore errors
        }
    }

    public async Task ClearLessonStateAsync()
    {
        try
        {
            securePreferences.Remove(LessonStateKey);
        }
        catch
        {
            // Ignore errors
        }
    }

    public Task<string> GetUserDisplayNameAsync() =>
        Task.FromResult(string.IsNullOrEmpty(S.UserDisplayName) ? "User" : S.UserDisplayName);

    public Task<string> GetAssistantDisplayNameAsync() =>
        Task.FromResult(string.IsNullOrEmpty(S.AssistantDisplayName) ? "Assistant" : S.AssistantDisplayName);

    public Task<string> GetUserAvatarIconAsync() =>
        Task.FromResult(string.IsNullOrEmpty(S.UserAvatarIcon) ? "bi-person-fill" : S.UserAvatarIcon);

    public Task<string> GetAssistantAvatarIconAsync() =>
        Task.FromResult(string.IsNullOrEmpty(S.AssistantAvatarIcon) ? "bi-robot" : S.AssistantAvatarIcon);

    public Task<string?> GetUserCustomAvatarAsync() =>
        Task.FromResult(string.IsNullOrEmpty(S.UserCustomAvatar) ? null : S.UserCustomAvatar);

    public Task<string?> GetAssistantCustomAvatarAsync() =>
        Task.FromResult(string.IsNullOrEmpty(S.AssistantCustomAvatar) ? null : S.AssistantCustomAvatar);

    /// <summary>Default maximum iterations for orphan linking (default: 10).</summary>
    public const int DefaultOrphanLinkingMaxIterations = 10;

    /// <summary>Default minimum confidence threshold for orphan linking (default: 0.3).</summary>
    public const float DefaultOrphanLinkingMinConfidence = 0.3f;

    public Task<int> GetOrphanLinkingMaxIterationsAsync() => Task.FromResult(S.OrphanLinkingMaxIterations);

    public Task<float> GetOrphanLinkingMinConfidenceAsync() => Task.FromResult((float)S.OrphanLinkingMinConfidence);

    /// <summary>Minimum reading time in seconds (default: 5).</summary>
    public Task<int> GetReadingTimeSecondsAsync() => Task.FromResult(S.ReadingTimeSeconds);

    /// <summary>Whether reading time is calculated from text length (default: false).</summary>
    public Task<bool> GetUseCalculatedReadingTimeAsync() => Task.FromResult(S.UseCalculatedReadingTime);
}

public enum GradingScale
{
    Standard,    // A=90, B=80, C=70, D=60
    PlusMinus,   // A+, A, A-, B+, etc.
    Lenient,     // A=85, B=75, C=65, D=55
    Strict       // A=95, B=85, C=75, D=65
}

public class LessonState
{
    public string CurriculumId { get; set; } = "";
    public int CurrentLessonIndex { get; set; }
    public bool HasStartedLearning { get; set; }
    public List<string> LessonTopics { get; set; } = [];
    public List<string> LearnedConcepts { get; set; } = [];
    public List<string> EncounteredConcepts { get; set; } = [];
    public List<int> VisitedTopicIndices { get; set; } = [];
    public List<ChatMessageState> Messages { get; set; } = [];
}

public class ChatMessageState
{
    public string Role { get; set; } = "";
    public string Text { get; set; } = "";
    public string FullJson { get; set; } = "";
}
