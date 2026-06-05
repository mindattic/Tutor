using Microsoft.AspNetCore.Http;
using MindAttic.Authentication;
using Tutor.Core.Models;

namespace Tutor.Core.Services;

/// <summary>
/// Service for managing quiz sessions and results.
/// </summary>
public class QuizService
{
    private readonly IQuizController quizController;
    private readonly IHttpContextAccessor httpContextAccessor;
    private QuizSession? currentSession;

    /// <summary>Event fired when quiz session state changes</summary>
    public event Action? OnQuizStateChanged;

    public QuizService(IQuizController quizController, IHttpContextAccessor httpContextAccessor)
    {
        this.quizController = quizController;
        this.httpContextAccessor = httpContextAccessor;
    }

    // The MindAttic.Authentication user id (AuthUser.Id) from the current principal. NOTE: in a
    // long-lived Blazor Server circuit HttpContext is null after the initial render, so this resolves
    // to "anonymous" for in-circuit quiz starts. TODO: thread the user id from the calling component's
    // AuthenticationState for reliable per-user quiz attribution (pre-existing single-session singleton).
    private string CurrentUserId =>
        httpContextAccessor.HttpContext?.User.FindFirst(MaClaims.UserId)?.Value ?? "anonymous";

    /// <summary>Gets whether there's an active quiz session</summary>
    public bool HasActiveSession => currentSession != null && !currentSession.IsComplete;

    /// <summary>Gets the current quiz session</summary>
    public QuizSession? CurrentSession => currentSession;

    /// <summary>
    /// Start a new quiz session for the given sections.
    /// </summary>
    public async Task<QuizSession?> StartQuizAsync(
        string courseId,
        List<string> sectionIds,
        List<string> conceptIds,
        int questionCount = 5)
    {
        var userId = CurrentUserId;
        
        var questions = await quizController.GenerateQuestionsAsync(
            courseId, sectionIds, conceptIds, questionCount);

        if (questions.Count == 0)
            return null;

        currentSession = new QuizSession
        {
            UserId = userId,
            CourseId = courseId,
            SectionIds = sectionIds,
            Questions = questions,
            CurrentQuestionIndex = 0,
            StartedAt = DateTime.UtcNow
        };

        OnQuizStateChanged?.Invoke();
        return currentSession;
    }

    /// <summary>
    /// Submit an answer for the current question.
    /// </summary>
    public async Task<(bool isCorrect, string? feedback, bool quizComplete)> SubmitAnswerAsync(string answer)
    {
        if (currentSession == null || currentSession.IsComplete)
            return (false, "No active quiz session.", true);

        var question = currentSession.CurrentQuestion;
        if (question == null)
            return (false, "No current question.", true);

        var (isCorrect, feedback) = await quizController.EvaluateAnswerAsync(question, answer);

        var quizAnswer = new QuizAnswer
        {
            QuestionId = question.Id,
            UserAnswer = answer,
            IsCorrect = isCorrect,
            AnsweredAt = DateTime.UtcNow
        };

        currentSession.Answers.Add(quizAnswer);
        currentSession.CurrentQuestionIndex++;

        var quizComplete = currentSession.IsComplete;

        if (quizComplete)
        {
            // Save the result
            var result = currentSession.ToResult();
            await quizController.SaveQuizResultAsync(result);
        }

        OnQuizStateChanged?.Invoke();
        return (isCorrect, feedback, quizComplete);
    }

    /// <summary>
    /// Get the current question.
    /// </summary>
    public QuizQuestion? GetCurrentQuestion()
    {
        return currentSession?.CurrentQuestion;
    }

    /// <summary>
    /// Get the quiz result for the current session.
    /// </summary>
    public QuizResult? GetCurrentResult()
    {
        return currentSession?.IsComplete == true ? currentSession.ToResult() : null;
    }

    /// <summary>
    /// Cancel the current quiz session.
    /// </summary>
    public void CancelQuiz()
    {
        currentSession = null;
        OnQuizStateChanged?.Invoke();
    }

    /// <summary>
    /// Get quiz history for the current user.
    /// </summary>
    public async Task<List<QuizResult>> GetQuizHistoryAsync(string? courseId = null)
    {
        var userId = CurrentUserId;
        return await quizController.GetQuizResultsAsync(userId, courseId);
    }

    /// <summary>
    /// Check if a section's quiz has been completed.
    /// </summary>
    public async Task<bool> IsSectionQuizCompletedAsync(string courseId, string sectionId)
    {
        var history = await GetQuizHistoryAsync(courseId);
        return history.Any(r => r.SectionIds.Contains(sectionId) && r.Passed);
    }

    /// <summary>
    /// Get the best score for a section.
    /// </summary>
    public async Task<double?> GetBestScoreForSectionAsync(string courseId, string sectionId)
    {
        var history = await GetQuizHistoryAsync(courseId);
        var sectionResults = history.Where(r => r.SectionIds.Contains(sectionId)).ToList();
        
        if (sectionResults.Count == 0)
            return null;
        
        return sectionResults.Max(r => r.ScorePercentage);
    }
}
