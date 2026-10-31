using Tutor.Core.Services;

namespace Tutor.Cli.Commands;

/// <summary>
/// <c>tutor keys [--provider claude|openai|gemini|deepseek|kimi] [--open]</c> — shows which keys Tutor
/// would use, by name only (never the values), in the order it tries them. Keys are added, changed and
/// assigned only in MindAttic.Vault.Desktop (LAMP, TUT-T31); <c>--open</c> opens Vault at Tutor's tab.
/// The old <c>--set-key/--add-key/--remove-key/--clear</c> editing switches are gone.
/// </summary>
public sealed class KeysCommand
{
    /// <summary>Providers Tutor calls.</summary>
    public static readonly string[] Providers = ["claude", "openai", "gemini", "deepseek", "kimi"];

    private static readonly string[] EditSwitches = ["--set-key", "--add-key", "--remove-key", "--clear"];

    private readonly TutorKeys keys;

    public KeysCommand(TutorKeys keys)
    {
        this.keys = keys;
    }

    public Task<int> RunAsync(string[] args, CancellationToken ct = default)
    {
        string? provider = null;
        var open = false;
        for (var i = 0; i < args.Length; i++)
        {
            if (args[i] == "--provider" && i + 1 < args.Length) provider = args[++i].ToLowerInvariant();
            else if (args[i] == "--open") open = true;
            else if (EditSwitches.Contains(args[i]))
            {
                Console.Error.WriteLine("Keys are edited in MindAttic Vault now (Keys tab, and Tutor's tab for which keys Tutor uses). " +
                                        "Run `tutor keys --open` to open it.");
                return Task.FromResult(64);
            }
        }

        if (provider is not null && !Providers.Contains(provider))
        {
            Console.Error.WriteLine($"Usage: tutor keys [--provider {string.Join("|", Providers)}] [--open]");
            return Task.FromResult(64);
        }

        if (open)
        {
            var result = SettingsCog.Open();
            (result.Opened ? Console.Out : Console.Error).WriteLine(result.Message);
            return Task.FromResult(result.Opened ? 0 : 1);
        }

        foreach (var p in provider is null ? Providers : new[] { provider })
        {
            var names = keys.KeyNames(p);
            Console.WriteLine(names.Count == 0
                ? $"{p}: no usable key — add one in MindAttic Vault."
                : $"{p}: {names.Count} key(s), tried in order: {string.Join(", ", names)}");
        }
        return Task.FromResult(0);
    }
}
