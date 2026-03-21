using Tutor.Core.Services.Abstractions;

public sealed class InstructionsService
{
    private const string StorageKeyPrefix = "INSTRUCTION_";
    private const string CountKey = "INSTRUCTION_COUNT";

    private readonly ISecurePreferences _securePreferences;

    public InstructionsService(ISecurePreferences securePreferences)
    {
        _securePreferences = securePreferences;
    }

    // Hardcoded internal instructions for consistent, direct responses
    private static readonly List<string> InternalInstructions =
    [
        "No glazing or excessive praise (avoid 'Great question!', 'Excellent!', 'That's a wonderful question!')",
        "No preamble - get straight to the content",
        "No filler phrases ('Let me explain...', 'I'd be happy to...', 'Sure thing!')",
        "Be concise and direct",
        "Use clear, simple language appropriate for a student",
        "Bold **key terms** that are important concepts",
        "Use bullet points for clarity",
        "Never user numbered lists",
        "Keep paragraphs short (2-3 sentences max)",
        "End teaching sections with a brief check-in question, not excessive encouragement",
        "When correcting mistakes, be kind but direct",
        "Focus on substance over style"
    ];

    public async Task<List<string>> GetAllInstructionsAsync()
    {
        var instructions = new List<string>();
        
        try
        {
            var countStr = await _securePreferences.GetAsync(CountKey);
            var count = int.TryParse(countStr, out var c) ? c : 0;

            for (int i = 0; i < count; i++)
            {
                var instruction = await _securePreferences.GetAsync($"{StorageKeyPrefix}{i}");
                if (!string.IsNullOrWhiteSpace(instruction))
                {
                    instructions.Add(instruction);
                }
            }
        }
        catch
        {
            // SecureStorage can fail on some platforms
        }

        return instructions;
    }

    public async Task<string> GetCombinedInstructionsAsync()
    {
        var userInstructions = await GetAllInstructionsAsync();
        
        // Combine internal instructions with user instructions
        var allInstructions = new List<string>(InternalInstructions);
        allInstructions.AddRange(userInstructions);
        
        // Format as bullet list
        var formatted = allInstructions.Select(i => $"- {i}");
        return "RESPONSE FORMAT RULES (ALWAYS FOLLOW):\n" + string.Join("\n", formatted);
    }
}
