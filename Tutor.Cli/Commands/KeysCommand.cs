using Tutor.Core.Services;

namespace Tutor.Cli.Commands;

/// <summary>
/// <c>tutor keys --provider claude|openai|gemini|deepseek
///   (--list | --set-key &lt;key&gt; [--set-key &lt;key&gt; ...] | --add-key &lt;key&gt; | --remove-key &lt;key&gt; | --clear)</c>
/// — manages the BYO API-key pool each LLM service (<see cref="Tutor.Core.Services.ClaudeService"/> and
/// its siblings) tries in order, failing over key-to-key on an auth/rate-limit/server error
/// (<see cref="KeyPoolFailover"/>). One or more <c>--set-key</c> flags REPLACE the whole pool;
/// <c>--add-key</c>/<c>--remove-key</c> edit it incrementally; <c>--list</c> shows what's configured
/// (masked) without changing anything. Backed by <see cref="ApiKeyPoolService"/>, the same CRUD
/// surface the MCP tools use, so either front door sees the other's writes immediately.
/// </summary>
public sealed class KeysCommand
{
    private readonly ApiKeyPoolService pool;

    public KeysCommand(ApiKeyPoolService pool)
    {
        this.pool = pool;
    }

    public async Task<int> RunAsync(string[] args, CancellationToken ct = default)
    {
        string? provider = null;
        var setKeys = new List<string>();
        string? addKey = null, removeKey = null;
        var clear = false;
        var list = false;

        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--provider":   if (i + 1 < args.Length) provider = args[++i].ToLowerInvariant(); break;
                case "--set-key":    if (i + 1 < args.Length) setKeys.Add(args[++i]); break;
                case "--add-key":    if (i + 1 < args.Length) addKey = args[++i]; break;
                case "--remove-key": if (i + 1 < args.Length) removeKey = args[++i]; break;
                case "--clear":      clear = true; break;
                case "--list":       list = true; break;
            }
        }

        if (provider is null || !ApiKeyPoolService.IsSupportedProvider(provider))
        {
            Console.Error.WriteLine(
                $"Usage: tutor keys --provider {string.Join("|", ApiKeyPoolService.Providers)} " +
                "(--list | --set-key <key> [--set-key <key> ...] | --add-key <key> | --remove-key <key> | --clear)");
            return 64;
        }

        if (list)
        {
            var current = await pool.GetPoolAsync(provider);
            Console.WriteLine(current.Count == 0
                ? $"{provider}: no BYO key configured — using the shared default, if one is configured."
                : $"{provider}: {current.Count} key(s), tried in order:\n" +
                  string.Join("\n", current.Select((k, idx) => $"  {idx + 1}. {Mask(k)}")));
            return 0;
        }

        if (clear)
        {
            await pool.ClearAsync(provider);
            Console.WriteLine($"{provider}: key pool cleared — falling back to the shared default.");
            return 0;
        }

        if (addKey is not null)
        {
            var count = await pool.AddKeyAsync(provider, addKey);
            Console.WriteLine($"{provider}: added {Mask(addKey)} — pool now has {count} key(s).");
            return 0;
        }

        if (removeKey is not null)
        {
            var (removed, remaining) = await pool.RemoveKeyAsync(provider, removeKey);
            Console.WriteLine(removed
                ? $"{provider}: removed {Mask(removeKey)} — pool now has {remaining} key(s)."
                : $"{provider}: {Mask(removeKey)} was not in the pool — nothing changed.");
            return removed ? 0 : 1;
        }

        if (setKeys.Count == 0)
        {
            Console.Error.WriteLine("One of --list, --set-key <key> (repeatable), --add-key, --remove-key, or --clear is required.");
            return 64;
        }

        await pool.SetPoolAsync(provider, setKeys);
        Console.WriteLine(setKeys.Count == 1
            ? $"{provider}: BYO key saved."
            : $"{provider}: BYO key pool saved ({setKeys.Count} keys) — tried in order, failing over on error.");
        return 0;
    }

    private static string Mask(string key) => key.Length >= 4 ? "…" + key[^4..] : "(short key)";
}
