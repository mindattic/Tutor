namespace Tutor.Cli.Commands;

/// <summary>
/// Tiny option parser for <c>--flag value</c> style arguments. Keeps the CLI free
/// of a dependency on System.CommandLine while we're still in the prototype phase.
/// </summary>
internal static class Args
{
    /// <summary>
    /// Splits <paramref name="args"/> into bare positionals and a <c>--name value</c>
    /// dictionary. A flag without a following value is treated as <c>"true"</c>.
    /// </summary>
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

    /// <summary>
    /// Convenience accessor that returns null when the option is missing.
    /// </summary>
    public static string? Get(this Dictionary<string, string> opts, string name)
        => opts.TryGetValue(name, out var v) ? v : null;
}
