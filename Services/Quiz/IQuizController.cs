using Tutor.Models;

namespace Tutor.Services;

/// <summary>
/// Interface for quiz operations.
/// Designed for future API integration.
/// </summary>
public interface IQuizController
{
    /// <summary>Generate quiz questions for given sections</summary>
    Task<List<QuizQuestion>> GenerateQuestionsAsync(
        string courseId, 
        List<string> sectionIds, 
        List<string> conceptIds,
        int questionCount = 5);
    
    /// <summary>Evaluate an answer</summary>
    Task<(bool isCorrect, string? feedback)> EvaluateAnswerAsync(
        QuizQuestion question, 
        string userAnswer);
    
    /// <summary>Save quiz result</summary>
    Task SaveQuizResultAsync(QuizResult result);
    
    /// <summary>Get quiz results for a user</summary>
    Task<List<QuizResult>> GetQuizResultsAsync(string userId, string? courseId = null);
}
