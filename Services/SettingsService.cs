using Microsoft.Maui.Storage;
using System.Text.Json;

public sealed class SettingsService
{
    private const string EnterToSendKey = "ENTER_TO_SEND";
    private const string QuizQuestionCountKey = "QUIZ_QUESTION_COUNT";
    private const string GradingScaleKey = "GRADING_SCALE";
    private const string LessonStateKey = "LESSON_STATE";

    // Enter to send setting
    public async Task<bool> GetEnterToSendAsync()
    {
        try
        {
            var value = await SecureStorage.GetAsync(EnterToSendKey);
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
            await SecureStorage.SetAsync(EnterToSendKey, value.ToString().ToLower());
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
            var value = await SecureStorage.GetAsync(QuizQuestionCountKey);
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
            await SecureStorage.SetAsync(QuizQuestionCountKey, count.ToString());
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
            var value = await SecureStorage.GetAsync(GradingScaleKey);
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
            await SecureStorage.SetAsync(GradingScaleKey, scale.ToString());
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
            var json = await SecureStorage.GetAsync(LessonStateKey);
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
                SecureStorage.Remove(LessonStateKey);
            }
            else
            {
                var json = JsonSerializer.Serialize(state);
                await SecureStorage.SetAsync(LessonStateKey, json);
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
            SecureStorage.Remove(LessonStateKey);
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
            var value = await SecureStorage.GetAsync(UserDisplayNameKey);
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
            await SecureStorage.SetAsync(UserDisplayNameKey, name);
        }
        catch { }
    }

    public async Task<string> GetAssistantDisplayNameAsync()
    {
        try
        {
            var value = await SecureStorage.GetAsync(AssistantDisplayNameKey);
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
            await SecureStorage.SetAsync(AssistantDisplayNameKey, name);
        }
        catch { }
    }

    public async Task<string> GetUserAvatarIconAsync()
    {
        try
        {
            var value = await SecureStorage.GetAsync(UserAvatarIconKey);
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
            await SecureStorage.SetAsync(UserAvatarIconKey, icon);
        }
        catch { }
    }

    public async Task<string> GetAssistantAvatarIconAsync()
    {
        try
        {
            var value = await SecureStorage.GetAsync(AssistantAvatarIconKey);
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
            await SecureStorage.SetAsync(AssistantAvatarIconKey, icon);
        }
        catch { }
    }

    public async Task<string?> GetUserCustomAvatarAsync()
    {
        try
        {
            return await SecureStorage.GetAsync(UserCustomAvatarKey);
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
                SecureStorage.Remove(UserCustomAvatarKey);
            }
            else
            {
                await SecureStorage.SetAsync(UserCustomAvatarKey, dataUrl);
            }
        }
        catch { }
    }

    public async Task<string?> GetAssistantCustomAvatarAsync()
    {
        try
        {
            return await SecureStorage.GetAsync(AssistantCustomAvatarKey);
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
                SecureStorage.Remove(AssistantCustomAvatarKey);
            }
            else
            {
                await SecureStorage.SetAsync(AssistantCustomAvatarKey, dataUrl);
            }
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
    public List<int> VisitedTopicIndices { get; set; } = [];
    public List<ChatMessageState> Messages { get; set; } = [];
}

public class ChatMessageState
{
    public string Role { get; set; } = "";
    public string Text { get; set; } = "";
    public string FullJson { get; set; } = "";
}
