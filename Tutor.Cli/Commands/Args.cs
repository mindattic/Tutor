namespace Tutor.Cli.Commands;

// Tiny option parser for `--flag value` style arguments. Keeps the CLI free
// of a dependency on System.CommandLine while we're still in the prototype phase.
internal static class Args
{
    public static (List<string> Positionals, Dictionary<string, string> Options) Parse(string[] args)
    {
        var positionals = new List<string>();
        var options = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        for (var i = 0; i < args.Length; i++)
        {
            var a = args[i];
            if (a.StartsWith("--"))
            {
                var name = a[2..];
                var value = i + 1 < args.Length && !args[i + 1].StartsWith("--") ? args[++i] : "true";
                options[name] = value;
            }
            else
            {
                positionals.Add(a);
            }
        }

        return (positionals, options);
    }

    public static string? Get(this Dictionary<string, string> opts, string name)
        => opts.TryGetValue(name, out var v) ? v : null;
}
