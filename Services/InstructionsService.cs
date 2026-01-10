using Microsoft.Maui.Storage;

public sealed class InstructionsService
{
    private const string StorageKeyPrefix = "INSTRUCTION_";
    private const string CountKey = "INSTRUCTION_COUNT";

    public async Task<List<string>> GetAllInstructionsAsync()
    {
        var instructions = new List<string>();
        
        try
        {
            var countStr = await SecureStorage.GetAsync(CountKey);
            var count = int.TryParse(countStr, out var c) ? c : 0;

            for (int i = 0; i < count; i++)
            {
                var instruction = await SecureStorage.GetAsync($"{StorageKeyPrefix}{i}");
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
        var instructions = await GetAllInstructionsAsync();
        if (instructions.Count == 0) return "";
        
        return string.Join("\n\n", instructions);
    }
}
