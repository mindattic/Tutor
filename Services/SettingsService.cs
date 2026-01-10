using Microsoft.Maui.Storage;

public sealed class SettingsService
{
    private const string EnterToSendKey = "ENTER_TO_SEND";
    private const string QuizQuestionCountKey = "QUIZ_QUESTION_COUNT";
    private const string GradingScaleKey = "GRADING_SCALE";

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
}

public enum GradingScale
{
    Standard,    // A=90, B=80, C=70, D=60
    PlusMinus,   // A+, A, A-, B+, etc.
    Lenient,     // A=85, B=75, C=65, D=55
    Strict       // A=95, B=85, C=75, D=65
}
