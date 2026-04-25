using System.Text.Json;
using Tutor.Core.Models;

namespace Tutor.Core.Services;

/// <summary>
/// Shared LLM credential store at %APPDATA%/MindAttic/LLM/.
///
/// Every MindAttic application reads from this single location so a credential
/// rotation in one app (Tutor, StreetSamurai, LLMThinkTank, etc.) is
/// immediately visible to all the others.
///
/// On-disk layout (multiple coexisting formats are read; writes mirror BOTH so
/// older apps keep working):
///   %APPDATA%/MindAttic/LLM/
///     providers.json   — canonical rich format used by LLMThinkTank + LLMVoting:
///                        { "claude": { "type": "anthropic", "apiKey": "...",
///                                      "model": "...", "maxTokens": 2048 }, ... }
///     &lt;providerId&gt;.json — Tutor/StreetSamurai legacy per-provider files:
///                        { "apiKey": "...", "model": "...", "extra": { ... } }
///
/// Resolution precedence:
///   1. Environment variable  (Azure / CI override; resolved by callers)
///   2. &lt;providerId&gt;.json     (this store's native format — Tutor wrote it here historically)
///   3. providers.json        (canonical shared format — read so keys saved by other apps work)
/// </summary>
public class LlmCredentialStore
{
    /// <summary>%APPDATA%/MindAttic/LLM/. Created on first write.</summary>
    public static string Root { get; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "MindAttic", "LLM");

    private const string ProvidersJsonFile = "providers.json";

    private static readonly JsonSerializerOptions jsonOpts = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    private readonly string root;
    private readonly object writeLock = new();

    public LlmCredentialStore() : this(Root) { }

    /// <summary>Constructor with explicit root (for tests).</summary>
    public LlmCredentialStore(string rootDir)
    {
        this.root = rootDir;
    }

    public string PathFor(string providerId) =>
        Path.Combine(root, $"{Normalize(providerId)}.json");

    public string ProvidersJsonPath => Path.Combine(root, ProvidersJsonFile);

    public bool Exists(string providerId) =>
        File.Exists(PathFor(providerId)) || ProvidersJsonHas(Normalize(providerId));

    public LlmCredential? Read(string providerId)
    {
        var id = Normalize(providerId);
        // 1. Native per-provider file (current Tutor format)
        var path = PathFor(id);
        if (File.Exists(path))
        {
            try
            {
                var json = File.ReadAllText(path);
                var cred = JsonSerializer.Deserialize<LlmCredential>(json, jsonOpts);
                if (cred is not null) return cred;
            }
            catch { }
        }

        // 2. Canonical providers.json (used by LLMThinkTank / LLMVoting / StreetSamurai)
        return ReadFromProvidersJson(id);
    }

    public void Write(string providerId, LlmCredential cred)
    {
        var id = Normalize(providerId);
        lock (writeLock)
        {
            Directory.CreateDirectory(root);

            // Native: <id>.json (preserves backwards-compat readers)
            var json = JsonSerializer.Serialize(cred, jsonOpts);
            File.WriteAllText(PathFor(id), json);

            // Canonical: providers.json (so other MindAttic apps see the change)
            UpsertProvidersJson(id, cred);
        }
    }

    public string GetApiKey(string providerId) => Read(providerId)?.ApiKey ?? "";
    public string GetModel(string providerId)  => Read(providerId)?.Model  ?? "";
    public string GetExtra(string providerId, string key) =>
        Read(providerId) is { } c && c.Extra.TryGetValue(key, out var v) ? v : "";

    public void SetApiKey(string providerId, string apiKey)
    {
        var cred = Read(providerId) ?? new LlmCredential();
        cred.ApiKey = apiKey ?? "";
        Write(providerId, cred);
    }

    public void SetModel(string providerId, string model)
    {
        var cred = Read(providerId) ?? new LlmCredential();
        cred.Model = model ?? "";
        Write(providerId, cred);
    }

    public void SetExtra(string providerId, string key, string value)
    {
        var cred = Read(providerId) ?? new LlmCredential();
        cred.Extra[key] = value ?? "";
        Write(providerId, cred);
    }

    public void Delete(string providerId)
    {
        var id = Normalize(providerId);
        lock (writeLock)
        {
            var path = PathFor(id);
            if (File.Exists(path)) File.Delete(path);
            RemoveFromProvidersJson(id);
        }
    }

    /// <summary>
    /// Provider IDs that currently have a credential anywhere in the shared folder
    /// (per-provider files OR providers.json).
    /// </summary>
    public IEnumerable<string> ListProviders()
    {
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (Directory.Exists(root))
        {
            foreach (var f in Directory.GetFiles(root, "*.json"))
            {
                var name = Path.GetFileNameWithoutExtension(f);
                if (string.Equals(name, "providers", StringComparison.OrdinalIgnoreCase)) continue;
                if (string.Equals(name, "credentials", StringComparison.OrdinalIgnoreCase)) continue;
                seen.Add(name);
            }
        }
        foreach (var id in LoadProvidersRawSafe().Keys) seen.Add(id);
        return seen;
    }

    // ── providers.json (canonical) interop ─────────────────────────────────────

    private LlmCredential? ReadFromProvidersJson(string providerId)
    {
        var providers = LoadProvidersRawSafe();
        if (!providers.TryGetValue(providerId, out var json)) return null;
        try
        {
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.ValueKind != JsonValueKind.Object) return null;

            var cred = new LlmCredential();
            foreach (var prop in doc.RootElement.EnumerateObject())
            {
                if (prop.NameEquals("apiKey") && prop.Value.ValueKind == JsonValueKind.String)
                    cred.ApiKey = prop.Value.GetString() ?? "";
                else if (prop.NameEquals("model") && prop.Value.ValueKind == JsonValueKind.String)
                    cred.Model = prop.Value.GetString() ?? "";
                else if (prop.NameEquals("type") || prop.NameEquals("maxTokens"))
                    continue; // canonical-only metadata; not surfaced as Extra
                else if (prop.Value.ValueKind == JsonValueKind.String)
                    cred.Extra[prop.Name] = prop.Value.GetString() ?? "";
                else
                    cred.Extra[prop.Name] = prop.Value.GetRawText();
            }
            return cred;
        }
        catch { return null; }
    }

    private bool ProvidersJsonHas(string providerId) =>
        LoadProvidersRawSafe().ContainsKey(providerId);

    private Dictionary<string, string> LoadProvidersRawSafe()
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        try
        {
            var path = ProvidersJsonPath;
            if (!File.Exists(path)) return result;
            var raw = File.ReadAllText(path);
            if (string.IsNullOrWhiteSpace(raw)) return result;
            using var doc = JsonDocument.Parse(raw);
            if (doc.RootElement.ValueKind != JsonValueKind.Object) return result;
            foreach (var prop in doc.RootElement.EnumerateObject())
            {
                if (prop.Value.ValueKind != JsonValueKind.Object) continue;
                result[prop.Name] = prop.Value.GetRawText();
            }
        }
        catch { }
        return result;
    }

    private void UpsertProvidersJson(string providerId, LlmCredential cred)
    {
        var providers = LoadProvidersRawSafe();

        // Preserve any existing fields (type, maxTokens, etc.) we don't manage.
        string? type = null;
        int? maxTokens = null;
        if (providers.TryGetValue(providerId, out var existing))
        {
            try
            {
                using var doc = JsonDocument.Parse(existing);
                if (doc.RootElement.TryGetProperty("type", out var t) && t.ValueKind == JsonValueKind.String) type = t.GetString();
                if (doc.RootElement.TryGetProperty("maxTokens", out var mt) && mt.ValueKind == JsonValueKind.Number) maxTokens = mt.GetInt32();
            }
            catch { }
        }
        type ??= providerId.Equals("claude", StringComparison.OrdinalIgnoreCase) ? "anthropic"
              :  providerId.Equals("gemini", StringComparison.OrdinalIgnoreCase) ? "google"
              :  "bearer";

        using var ms = new MemoryStream();
        using (var w = new Utf8JsonWriter(ms, new JsonWriterOptions { Indented = true }))
        {
            w.WriteStartObject();
            w.WriteString("type", type);
            w.WriteString("apiKey", cred.ApiKey ?? "");
            if (!string.IsNullOrWhiteSpace(cred.Model))
                w.WriteString("model", cred.Model);
            if (maxTokens.HasValue)
                w.WriteNumber("maxTokens", maxTokens.Value);
            foreach (var (k, v) in cred.Extra)
                w.WriteString(k, v ?? "");
            w.WriteEndObject();
        }
        providers[providerId] = System.Text.Encoding.UTF8.GetString(ms.ToArray());
        WriteProvidersJson(providers);
    }

    private void RemoveFromProvidersJson(string providerId)
    {
        var providers = LoadProvidersRawSafe();
        if (providers.Remove(providerId))
            WriteProvidersJson(providers);
    }

    private void WriteProvidersJson(IDictionary<string, string> providers)
    {
        Directory.CreateDirectory(root);
        using var ms = new MemoryStream();
        using (var w = new Utf8JsonWriter(ms, new JsonWriterOptions { Indented = true }))
        {
            w.WriteStartObject();
            foreach (var (id, json) in providers.OrderBy(kv => kv.Key, StringComparer.OrdinalIgnoreCase))
            {
                w.WritePropertyName(id);
                try
                {
                    using var doc = JsonDocument.Parse(string.IsNullOrWhiteSpace(json) ? "{}" : json);
                    doc.RootElement.WriteTo(w);
                }
                catch
                {
                    w.WriteStartObject();
                    w.WriteEndObject();
                }
            }
            w.WriteEndObject();
        }
        File.WriteAllBytes(ProvidersJsonPath, ms.ToArray());
    }

    private static string Normalize(string providerId) =>
        providerId.Trim().ToLowerInvariant().Replace(' ', '-').Replace('_', '-');
}
