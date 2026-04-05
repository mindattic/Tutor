using System.Text.Json;
using Tutor.Core.Services.Abstractions;
using Tutor.Core.Services.Logging;

public sealed class SettingsService
{
    private const string EnterToSendKey = "ENTER_TO_SEND";
    private const string QuizQuestionCountKey = "QUIZ_QUESTION_COUNT";
    private const string GradingScaleKey = "GRADING_SCALE";
    private const string LessonStateKey = "LESSON_STATE";
    private const string BorderRadiusKey = "BORDER_RADIUS";
    private const string DateFormatKey = "DATE_FORMAT";
    private const string ParagraphSpacingKey = "PARAGRAPH_SPACING";
    
    private readonly ISecurePreferences securePreferences;

    public SettingsService(ISecurePreferences securePreferences)
    {
        this.securePreferences = securePreferences;
    }

    // Log level settings keys
    private const string LogTraceKey = "LOG_TRACE";
    private const string LogDebugKey = "LOG_DEBUG";
    private const string LogInfoKey = "LOG_INFO";
    private const string LogWarningKey = "LOG_WARNING";
    private const string LogErrorKey = "LOG_ERROR";
    private const string LogCriticalKey = "LOG_CRITICAL";

    /// <summary>
    /// Current date format pattern. Updated when settings are loaded/changed.
    /// </summary>
    public static string CurrentDateFormat { get; private set; } = "yyyy-MM-dd hh:mm:ss tt";

    /// <summary>
    /// Formats a DateTime using the current date format setting.
    /// </summary>
    public static string FormatDate(DateTime dateTime)
    {
        return dateTime.ToString(CurrentDateFormat);
    }

    // Date format setting
    public async Task<string> GetDateFormatAsync()
    {
        try
        {
            var value = await securePreferences.GetAsync(DateFormatKey);
            var format = string.IsNullOrEmpty(value) ? "yyyy-MM-dd hh:mm:ss tt" : value;
            CurrentDateFormat = format;
            return format;
        }
        catch
        {
            return "yyyy-MM-dd hh:mm:ss tt";
        }
    }

    public async Task SetDateFormatAsync(string format)
    {
        try
        {
            await securePreferences.SetAsync(DateFormatKey, format);
            CurrentDateFormat = format;
        }
        catch
        {
            // Ignore errors
        }
    }

    // Log level settings
    public async Task<bool> GetLogTraceEnabledAsync()
    {
        try
        {
            var value = await securePreferences.GetAsync(LogTraceKey);
            return value == "true"; // Default false
        }
        catch { return false; }
    }

    public async Task SetLogTraceEnabledAsync(bool enabled)
    {
        try
        {
            await securePreferences.SetAsync(LogTraceKey, enabled.ToString().ToLower());
            Log.Settings.LogTrace = enabled;
        }
        catch { }
    }

    public async Task<bool> GetLogDebugEnabledAsync()
    {
        try
        {
            var value = await securePreferences.GetAsync(LogDebugKey);
            return string.IsNullOrEmpty(value) || value == "true"; // Default true
        }
        catch { return true; }
    }

    public async Task SetLogDebugEnabledAsync(bool enabled)
    {
        try
        {
            await securePreferences.SetAsync(LogDebugKey, enabled.ToString().ToLower());
            Log.Settings.LogDebug = enabled;
        }
        catch { }
    }

    public async Task<bool> GetLogInfoEnabledAsync()
    {
        try
        {
            var value = await securePreferences.GetAsync(LogInfoKey);
            return string.IsNullOrEmpty(value) || value == "true"; // Default true
        }
        catch { return true; }
    }

    public async Task SetLogInfoEnabledAsync(bool enabled)
    {
        try
        {
            await securePreferences.SetAsync(LogInfoKey, enabled.ToString().ToLower());
            Log.Settings.LogInfo = enabled;
        }
        catch { }
    }

    public async Task<bool> GetLogWarningEnabledAsync()
    {
        try
        {
            var value = await securePreferences.GetAsync(LogWarningKey);
            return string.IsNullOrEmpty(value) || value == "true"; // Default true
        }
        catch { return true; }
    }

    public async Task SetLogWarningEnabledAsync(bool enabled)
    {
        try
        {
            await securePreferences.SetAsync(LogWarningKey, enabled.ToString().ToLower());
            Log.Settings.LogWarning = enabled;
        }
        catch { }
    }

    public async Task<bool> GetLogErrorEnabledAsync()
    {
        try
        {
            var value = await securePreferences.GetAsync(LogErrorKey);
            return string.IsNullOrEmpty(value) || value == "true"; // Default true
        }
        catch { return true; }
    }

    public async Task SetLogErrorEnabledAsync(bool enabled)
    {
        try
        {
            await securePreferences.SetAsync(LogErrorKey, enabled.ToString().ToLower());
            Log.Settings.LogError = enabled;
        }
        catch { }
    }

    public async Task<bool> GetLogCriticalEnabledAsync()
    {
        try
        {
            var value = await securePreferences.GetAsync(LogCriticalKey);
            return string.IsNullOrEmpty(value) || value == "true"; // Default true (always on)
        }
        catch { return true; }
    }

    public async Task SetLogCriticalEnabledAsync(bool enabled)
    {
        try
        {
            await securePreferences.SetAsync(LogCriticalKey, enabled.ToString().ToLower());
            Log.Settings.LogCritical = enabled;
        }
        catch { }
    }

    /// <summary>
    /// Loads all log level settings and applies them to Log.Settings.
    /// Call this on app startup.
    /// </summary>
    public async Task LoadLogSettingsAsync()
    {
        Log.Settings.LogTrace = await GetLogTraceEnabledAsync();
        Log.Settings.LogDebug = await GetLogDebugEnabledAsync();
        Log.Settings.LogInfo = await GetLogInfoEnabledAsync();
        Log.Settings.LogWarning = await GetLogWarningEnabledAsync();
        Log.Settings.LogError = await GetLogErrorEnabledAsync();
        Log.Settings.LogCritical = await GetLogCriticalEnabledAsync();
        
        // Also load date format
        await GetDateFormatAsync();
    }

    // Border radius setting (0-10px, default 6px)
    public async Task<int> GetBorderRadiusAsync()
    {
        try
        {
            var value = await securePreferences.GetAsync(BorderRadiusKey);
            return int.TryParse(value, out var radius) ? Math.Clamp(radius, 0, 10) : 6;
        }
        catch
        {
            return 6;
        }
    }

    public async Task SetBorderRadiusAsync(int radius)
    {
        try
        {
            await securePreferences.SetAsync(BorderRadiusKey, Math.Clamp(radius, 0, 10).ToString());
        }
        catch
        {
            // Ignore errors
        }
    }

    // Paragraph spacing setting (0.5-2.0em, default 1.0em)
    public async Task<double> GetParagraphSpacingAsync()
    {
        try
        {
            var value = await securePreferences.GetAsync(ParagraphSpacingKey);
            return double.TryParse(value, out var spacing) ? Math.Clamp(spacing, 0.5, 2.0) : 1.0;
        }
        catch
        {
            return 1.0;
        }
    }

    public async Task SetParagraphSpacingAsync(double spacing)
    {
        try
        {
            await securePreferences.SetAsync(ParagraphSpacingKey, Math.Clamp(spacing, 0.5, 2.0).ToString("F1"));
        }
        catch
        {
            // Ignore errors
        }
    }

    // Enter to send setting
    public async Task<bool> GetEnterToSendAsync()
    {
        try
        {
            var value = await securePreferences.GetAsync(EnterToSendKey);
            return string.IsNullOrEmpty(value) || value == "true";
        }
        catch
        {
            return true;
        }
    }

    public async Task SetEnterToSendAsync(bool value)
    {
        try
        {
            await securePreferences.SetAsync(EnterToSendKey, value.ToString().ToLower());
        }
        catch
        {
            // Ignore errors
        }
    }

    // Quiz question count setting
    public async Task<int> GetQuizQuestionCountAsync()
    {
        try
        {
            var value = await securePreferences.GetAsync(QuizQuestionCountKey);
            return int.TryParse(value, out var count) ? count : 3;
        }
        catch
        {
            return 3;
        }
    }

    public async Task SetQuizQuestionCountAsync(int count)
    {
        try
        {
            await securePreferences.SetAsync(QuizQuestionCountKey, count.ToString());
        }
        catch
        {
            // Ignore errors
        }
    }

    // Grading scale setting
    public async Task<GradingScale> GetGradingScaleAsync()
    {
        try
        {
            var value = await securePreferences.GetAsync(GradingScaleKey);
            return Enum.TryParse<GradingScale>(value, out var scale) ? scale : GradingScale.Standard;
        }
        catch
        {
            return GradingScale.Standard;
        }
    }

    public async Task SetGradingScaleAsync(GradingScale scale)
    {
        try
        {
            await securePreferences.SetAsync(GradingScaleKey, scale.ToString());
        }
        catch
        {
            // Ignore errors
        }
    }

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

    // Avatar and display name settings
    private const string UserDisplayNameKey = "USER_DISPLAY_NAME";
    private const string AssistantDisplayNameKey = "ASSISTANT_DISPLAY_NAME";
    private const string UserAvatarIconKey = "USER_AVATAR_ICON";
    private const string AssistantAvatarIconKey = "ASSISTANT_AVATAR_ICON";
    private const string UserCustomAvatarKey = "USER_CUSTOM_AVATAR";
    private const string AssistantCustomAvatarKey = "ASSISTANT_CUSTOM_AVATAR";

    public async Task<string> GetUserDisplayNameAsync()
    {
        try
        {
            var value = await securePreferences.GetAsync(UserDisplayNameKey);
            return string.IsNullOrEmpty(value) ? "User" : value;
        }
        catch
        {
            return "User";
        }
    }

    public async Task SetUserDisplayNameAsync(string name)
    {
        try
        {
            await securePreferences.SetAsync(UserDisplayNameKey, name);
        }
        catch { }
    }

    public async Task<string> GetAssistantDisplayNameAsync()
    {
        try
        {
            var value = await securePreferences.GetAsync(AssistantDisplayNameKey);
            return string.IsNullOrEmpty(value) ? "Assistant" : value;
        }
        catch
        {
            return "Assistant";
        }
    }

    public async Task SetAssistantDisplayNameAsync(string name)
    {
        try
        {
            await securePreferences.SetAsync(AssistantDisplayNameKey, name);
        }
        catch { }
    }

    public async Task<string> GetUserAvatarIconAsync()
    {
        try
        {
            var value = await securePreferences.GetAsync(UserAvatarIconKey);
            return string.IsNullOrEmpty(value) ? "bi-person-fill" : value;
        }
        catch
        {
            return "bi-person-fill";
        }
    }

    public async Task SetUserAvatarIconAsync(string icon)
    {
        try
        {
            await securePreferences.SetAsync(UserAvatarIconKey, icon);
        }
        catch { }
    }

    public async Task<string> GetAssistantAvatarIconAsync()
    {
        try
        {
            var value = await securePreferences.GetAsync(AssistantAvatarIconKey);
            return string.IsNullOrEmpty(value) ? "bi-robot" : value;
        }
        catch
        {
            return "bi-robot";
        }
    }

    public async Task SetAssistantAvatarIconAsync(string icon)
    {
        try
        {
            await securePreferences.SetAsync(AssistantAvatarIconKey, icon);
        }
        catch { }
    }

    public async Task<string?> GetUserCustomAvatarAsync()
    {
        try
        {
            return await securePreferences.GetAsync(UserCustomAvatarKey);
        }
        catch
        {
            return null;
        }
    }

    public async Task SetUserCustomAvatarAsync(string? dataUrl)
    {
        try
        {
            if (string.IsNullOrEmpty(dataUrl))
            {
                securePreferences.Remove(UserCustomAvatarKey);
            }
            else
            {
                await securePreferences.SetAsync(UserCustomAvatarKey, dataUrl);
            }
        }
        catch { }
    }

    public async Task<string?> GetAssistantCustomAvatarAsync()
    {
        try
        {
            return await securePreferences.GetAsync(AssistantCustomAvatarKey);
        }
        catch
        {
            return null;
        }
    }

    public async Task SetAssistantCustomAvatarAsync(string? dataUrl)
    {
        try
        {
            if (string.IsNullOrEmpty(dataUrl))
            {
                securePreferences.Remove(AssistantCustomAvatarKey);
            }
            else
            {
                await securePreferences.SetAsync(AssistantCustomAvatarKey, dataUrl);
            }
        }
        catch { }
    }

    // Orphan Linking Settings
    private const string OrphanLinkingMaxIterationsKey = "ORPHAN_LINKING_MAX_ITERATIONS";
    private const string OrphanLinkingMinConfidenceKey = "ORPHAN_LINKING_MIN_CONFIDENCE";

    /// <summary>
    /// Default maximum iterations for orphan linking (default: 10).
    /// </summary>
    public const int DefaultOrphanLinkingMaxIterations = 10;

    /// <summary>
    /// Default minimum confidence threshold for orphan linking (default: 0.3).
    /// </summary>
    public const float DefaultOrphanLinkingMinConfidence = 0.3f;

    public async Task<int> GetOrphanLinkingMaxIterationsAsync()
    {
        try
        {
            var value = await securePreferences.GetAsync(OrphanLinkingMaxIterationsKey);
            return int.TryParse(value, out var iterations) ? iterations : DefaultOrphanLinkingMaxIterations;
        }
        catch
        {
            return DefaultOrphanLinkingMaxIterations;
        }
    }

    public async Task SetOrphanLinkingMaxIterationsAsync(int iterations)
    {
        try
        {
            await securePreferences.SetAsync(OrphanLinkingMaxIterationsKey, iterations.ToString());
        }
        catch { }
    }

    public async Task<float> GetOrphanLinkingMinConfidenceAsync()
    {
        try
        {
            var value = await securePreferences.GetAsync(OrphanLinkingMinConfidenceKey);
            return float.TryParse(value, out var confidence) ? confidence : DefaultOrphanLinkingMinConfidence;
        }
        catch
        {
            return DefaultOrphanLinkingMinConfidence;
        }
    }


    public async Task SetOrphanLinkingMinConfidenceAsync(float confidence)
    {
        try
        {
            await securePreferences.SetAsync(OrphanLinkingMinConfidenceKey, confidence.ToString());
        }
        catch { }
    }

    // Reading time settings
    private const string ReadingTimeSecondsKey = "ReadingTimeSeconds";
    private const string UseCalculatedReadingTimeKey = "UseCalculatedReadingTime";

    /// <summary>
    /// Gets the minimum reading time in seconds (default: 5)
    /// </summary>
    public async Task<int> GetReadingTimeSecondsAsync()
    {
        try
        {
            var val = await securePreferences.GetAsync(ReadingTimeSecondsKey);
            if (int.TryParse(val, out var seconds))
                return seconds;
        }
        catch { }
        return 5; // Default 5 seconds
    }

    /// <summary>
    /// Sets the minimum reading time in seconds
    /// </summary>
    public async Task SetReadingTimeSecondsAsync(int seconds)
    {
        try
        {
            await securePreferences.SetAsync(ReadingTimeSecondsKey, seconds.ToString());
        }
        catch { }
    }

    /// <summary>
    /// Gets whether to use calculated reading time based on text length (default: false)
    /// </summary>
    public async Task<bool> GetUseCalculatedReadingTimeAsync()
    {
        try
        {
            var val = await securePreferences.GetAsync(UseCalculatedReadingTimeKey);
            if (bool.TryParse(val, out var use))
                return use;
        }
        catch { }
        return false; // Default: use fixed time
    }

    /// <summary>
    /// Sets whether to use calculated reading time based on text length
    /// </summary>
    public async Task SetUseCalculatedReadingTimeAsync(bool use)
    {
        try
        {
            await securePreferences.SetAsync(UseCalculatedReadingTimeKey, use.ToString());
        }
        catch { }
    }
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
