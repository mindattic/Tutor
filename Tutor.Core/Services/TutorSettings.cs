using System.Globalization;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using MindAttic.Authentication.Options;
using MindAttic.Vault.Settings;
using MindAttic.Vault.Settings.Manifest;

namespace Tutor.Core.Services;

/// <summary>
/// Tutor's app-level settings (LAMP, TUT-T31), kept in <c>%LOCALAPPDATA%\MindAttic\Tutor\settings.json</c>
/// (per-machine, like the <c>%LOCALAPPDATA%\Tutor\Settings</c> files they replace) and edited only in
/// MindAttic.Vault.Desktop. Property initializers are exactly the defaults Tutor used before LAMP.
/// Per-user and content data (the signed-in user, course lists, lesson state) stays in
/// <c>secure-preferences.json</c>; LLM keys live in Vault's shared named lists.
/// </summary>
[SettingsApp(AppId, "Tutor", Local = true, Icon = "school",
    Description = "AI tutor that builds courses from books and teaches them with concept maps and quizzes.")]
[UsesCredential("openai", Required = true, Purpose = "Embeddings for every course, and the ChatGPT teacher")]
[UsesCredential("claude", Purpose = "The Claude teacher (Tutor.Cli's default)")]
[UsesCredential("gemini", Purpose = "The Gemini teacher")]
[UsesCredential("deepseek", Purpose = "The DeepSeek teacher")]
[UsesCredential("kimi", Purpose = "The Kimi teacher")]
public sealed class TutorSettings
{
    /// <summary>Settings app id: Tutor's folder under <c>%LOCALAPPDATA%\MindAttic</c>.</summary>
    public const string AppId = "Tutor";

    public const string DefaultDateFormat = "yyyy-MM-dd hh:mm:ss tt";

    /// <summary>
    /// Where MindAttic.Authentication binds Tutor's auth options from (its <c>SettingsSection</c>): the
    /// <see cref="Auth"/> group, exposed by <c>AddMindAtticVaultConfiguration()</c> (or App Configuration).
    /// </summary>
    public const string AuthSettingsSection = "MindAttic:Settings:" + AppId + ":auth";

    /// <summary>Tutor's documented local HTTPS address (launchSettings), the reset-link origin it shipped with.</summary>
    public const string DefaultPublicBaseUrl = "https://localhost:7200";

    // ---- General ------------------------------------------------------------------------

    [Setting("Theme", Group = "General", Order = 1, Help = "Color theme for the whole app.")]
    [SettingChoices("Light", "Dark", "Summer", "Winter", "Autumn", "Spring")]
    public string Theme { get; set; } = "Light";

    [Setting("Active AI model", Group = "General", Order = 2,
        Help = "Which provider acts as the teacher. Embeddings always use OpenAI. Tutor.Cli sets this to its --llm choice (Claude by default) on each run.")]
    [SettingChoices("ChatGPT", "Claude", "DeepSeek", "Gemini")]
    public string ActiveModel { get; set; } = "ChatGPT";

    [Setting("Press Enter to send", Group = "General", Order = 3,
        Help = "On: Enter sends a chat message and Shift+Enter starts a new line.")]
    public bool EnterToSend { get; set; } = true;

    // ---- Appearance ---------------------------------------------------------------------

    [Setting("User display name", Group = "Appearance", Order = 1, Help = "Name shown on your chat messages.")]
    public string UserDisplayName { get; set; } = "User";

    [Setting("User avatar icon", Group = "Appearance", Order = 2, Help = "Bootstrap icon class used when no custom avatar image is set.")]
    [SettingChoices(
        "bi-person-fill", "bi-person-circle", "bi-person-badge-fill", "bi-emoji-smile-fill", "bi-emoji-sunglasses-fill",
        "bi-emoji-heart-eyes-fill", "bi-star-fill", "bi-heart-fill", "bi-lightning-fill", "bi-fire", "bi-rocket-takeoff-fill", "bi-trophy-fill",
        "bi-robot", "bi-cpu-fill", "bi-lightbulb-fill", "bi-book-fill", "bi-mortarboard-fill", "bi-chat-dots-fill", "bi-magic", "bi-gem",
        "bi-shield-fill", "bi-globe", "bi-moon-fill", "bi-sun-fill", "bi-cloud-fill", "bi-music-note-beamed", "bi-controller", "bi-puzzle-fill",
        "bi-flower1", "bi-tree-fill")]
    public string UserAvatarIcon { get; set; } = "bi-person-fill";

    [Setting("User avatar image", Group = "Appearance", Order = 3,
        Help = "Image URL or data: URL shown instead of the icon. Empty uses the icon.")]
    public string? UserCustomAvatar { get; set; }

    [Setting("Assistant display name", Group = "Appearance", Order = 4, Help = "Name shown on the tutor's chat messages.")]
    public string AssistantDisplayName { get; set; } = "Assistant";

    [Setting("Assistant avatar icon", Group = "Appearance", Order = 5, Help = "Bootstrap icon class used when no custom avatar image is set.")]
    [SettingChoices(
        "bi-person-fill", "bi-person-circle", "bi-person-badge-fill", "bi-emoji-smile-fill", "bi-emoji-sunglasses-fill",
        "bi-emoji-heart-eyes-fill", "bi-star-fill", "bi-heart-fill", "bi-lightning-fill", "bi-fire", "bi-rocket-takeoff-fill", "bi-trophy-fill",
        "bi-robot", "bi-cpu-fill", "bi-lightbulb-fill", "bi-book-fill", "bi-mortarboard-fill", "bi-chat-dots-fill", "bi-magic", "bi-gem",
        "bi-shield-fill", "bi-globe", "bi-moon-fill", "bi-sun-fill", "bi-cloud-fill", "bi-music-note-beamed", "bi-controller", "bi-puzzle-fill",
        "bi-flower1", "bi-tree-fill")]
    public string AssistantAvatarIcon { get; set; } = "bi-robot";

    [Setting("Assistant avatar image", Group = "Appearance", Order = 6,
        Help = "Image URL or data: URL shown instead of the icon. Empty uses the icon.")]
    public string? AssistantCustomAvatar { get; set; }

    [Setting("Border radius (px)", Group = "Appearance", Order = 7,
        Help = "Corner roundness of buttons, inputs and cards: 0 is square, 10 is rounded.")]
    [SettingRange(0, 10)]
    public int BorderRadius { get; set; } = 6;

    [Setting("Paragraph spacing (em)", Group = "Appearance", Order = 8,
        Help = "Space between paragraphs in chat messages: 0.5 is tight, 2.0 is loose.")]
    [SettingRange(0.5, 2.0)]
    public double ParagraphSpacing { get; set; } = 1.0;

    [Setting("Date and time format", Group = "Appearance", Order = 9,
        Help = ".NET date format used everywhere Tutor shows a date, e.g. in the log viewer.")]
    [SettingChoices("yyyy-MM-dd hh:mm:ss tt", "yyyy-MM-dd HH:mm:ss", "MM/dd/yyyy hh:mm:ss tt", "dd/MM/yyyy HH:mm:ss",
        "MMM dd, yyyy h:mm tt", "MMMM dd, yyyy h:mm:ss tt", "yyyy-MM-dd HH:mm:ss.fff")]
    public string DateFormat { get; set; } = DefaultDateFormat;

    // ---- Storage ------------------------------------------------------------------------

    [Setting("Learning resources folder", Group = "Storage", Order = 1,
        Help = "Where original and formatted resources and course files are saved. Empty uses the Resources folder next to the project (or %LOCALAPPDATA%\\Tutor\\Resources).")]
    [PathPicker(PathKind.Folder)]
    public string? LearningResourcesDirectory { get; set; }

    [Setting("Save formatted resources as .md", Group = "Storage", Order = 2,
        Help = "Writes each AI-formatted resource to the Formatted folder as a Markdown file.")]
    public bool SaveFormattedAsMarkdown { get; set; } = true;

    [Setting("Preserve original uploads", Group = "Storage", Order = 3,
        Help = "Keeps a copy of every uploaded file in the Original folder.")]
    public bool PreserveOriginalFiles { get; set; } = true;

    // ---- Learning -----------------------------------------------------------------------

    [Setting("Fixed reading time (seconds)", Group = "Learning", Order = 1,
        Help = "How long a section must be on screen before it counts as read (when the calculated time is off).")]
    [SettingRange(1, 3600)]
    public int ReadingTimeSeconds { get; set; } = 5;

    [Setting("Calculate reading time from length", Group = "Learning", Order = 2,
        Help = "On: the reading time is worked out at about 200 words a minute (at least 5 seconds) instead of the fixed time.")]
    public bool UseCalculatedReadingTime { get; set; }

    // ---- Quiz ---------------------------------------------------------------------------

    [Setting("Quiz questions", Group = "Quiz", Order = 1, Help = "How many questions each quiz asks.")]
    [SettingRange(1, 50)]
    public int QuizQuestionCount { get; set; } = 3;

    [Setting("Grading scale", Group = "Quiz", Order = 2,
        Help = "Standard A=90/B=80/C=70/D=60; PlusMinus adds +/-; Lenient A=85/B=75/C=65/D=55; Strict A=95/B=85/C=75/D=65.")]
    public GradingScale GradingScale { get; set; } = GradingScale.Standard;

    [Setting("Max concept-linking passes", Group = "Quiz", Order = 3,
        Help = "How many AI passes are made to link orphaned concepts in a concept map. Higher means more AI calls. A resource can override it.")]
    [SettingRange(1, 50)]
    public int OrphanLinkingMaxIterations { get; set; } = 10;

    [Setting("Min concept-link confidence", Group = "Quiz", Order = 4,
        Help = "Weakest AI-suggested link between concepts that is kept (0.1 permissive to 0.7 strict). A resource can override it.")]
    [SettingRange(0, 1)]
    public double OrphanLinkingMinConfidence { get; set; } = 0.3;

    // ---- Logging ------------------------------------------------------------------------

    [Setting("Log trace", Group = "Logging", Order = 1, Help = "Capture detailed diagnostic messages.")]
    public bool LogTrace { get; set; }

    [Setting("Log debug", Group = "Logging", Order = 2, Help = "Capture debug messages.")]
    public bool LogDebug { get; set; } = true;

    [Setting("Log info", Group = "Logging", Order = 3, Help = "Capture general operational messages.")]
    public bool LogInfo { get; set; } = true;

    [Setting("Log warnings", Group = "Logging", Order = 4, Help = "Capture potential issues.")]
    public bool LogWarning { get; set; } = true;

    [Setting("Log errors", Group = "Logging", Order = 5, Help = "Capture operation failures.")]
    public bool LogError { get; set; } = true;

    [Setting("Log critical", Group = "Logging", Order = 6, Help = "Capture critical errors (always recommended).")]
    public bool LogCritical { get; set; } = true;

    // ---- Sign-in security (MindAttic.Authentication) -----------------------------------

    /// <summary>
    /// MindAttic.Authentication's non-secret options (hashing, password policy, MFA, sessions, password
    /// reset). Secrets (pepper, reset-token key) stay in the Security bucket. Defaults are the library's,
    /// plus the reset-link origin Tutor's appsettings.json shipped with.
    /// </summary>
    [Setting("Sign-in security", Group = "Sign-in security", Order = 1,
        Help = "MindAttic.Authentication options for Tutor's accounts. Secrets (pepper, reset-token key) are not here.")]
    public MindAtticAuthSettings Auth { get; set; } = new() { Reset = { PublicBaseUrl = DefaultPublicBaseUrl } };

    // ---- Loading ------------------------------------------------------------------------

    /// <summary>The <c>secure-preferences.json</c> file the settings lived in before LAMP.</summary>
    public const string LegacyPreferencesFile = "secure-preferences.json";

    /// <summary>The <c>storage.settings.json</c> file the storage options lived in before LAMP.</summary>
    public const string LegacyStorageFile = "storage.settings.json";

    /// <summary>
    /// The deployed values, when the host's configuration carries <c>MindAttic:Settings:Tutor</c>
    /// (App Configuration); null otherwise.
    /// </summary>
    public static TutorSettings? FromConfiguration(IConfiguration? configuration)
    {
        var section = configuration?.GetSection(SettingsManifest.PrefixFor(AppId));
        return section is not null && section.Exists() ? section.Get<TutorSettings>() ?? new TutorSettings() : null;
    }

    /// <summary>
    /// Loads Tutor's settings: the deployed configuration when present; otherwise the Vault file,
    /// importing today's values once when the Vault file doesn't exist yet — from
    /// <paramref name="legacyDirectory"/> (default <see cref="StoragePaths.SettingsDirectory"/>) and, for
    /// the auth options, from <paramref name="configuration"/>'s <c>MindAttic:Auth</c> section
    /// (appsettings.json). The old files are only read, never changed (they are the rollback).
    /// </summary>
    public static TutorSettings Load(IConfiguration? configuration = null, ISettingsStore<TutorSettings>? store = null,
        string? legacyDirectory = null)
    {
        if (FromConfiguration(configuration) is { } deployed) return deployed;

        store ??= JsonSettingsStore<TutorSettings>.ForSettingsApp();
        if (!store.Exists())
        {
            var imported = ImportLegacy(legacyDirectory ?? StoragePaths.SettingsDirectory);
            if (configuration?.GetSection(MindAtticAuthSettings.LegacySection) is { } auth && auth.Exists())
            {
                imported ??= new TutorSettings();
                imported.Auth = MindAtticAuthSettings.FromConfiguration(configuration);
            }
            if (imported is not null) store.Save(imported);
        }
        return store.LoadOrSeed();
    }

    /// <summary>
    /// Today's values from the pre-LAMP files, parsed exactly as the old getters did (missing or
    /// unparseable values fall back to the same defaults). Null when neither file exists.
    /// </summary>
    public static TutorSettings? ImportLegacy(string legacyDirectory)
    {
        var prefs = ReadDictionary(Path.Combine(legacyDirectory, LegacyPreferencesFile));
        var storage = ReadStorage(Path.Combine(legacyDirectory, LegacyStorageFile));
        if (prefs is null && storage is null) return null;
        prefs ??= new Dictionary<string, string>();

        string? Get(string key) => prefs.TryGetValue(key, out var v) ? v : null;
        string Text(string key, string fallback) => Get(key) is { Length: > 0 } v ? v : fallback;
        bool DefaultTrue(string key) => Get(key) is not { Length: > 0 } v || v == "true";
        int Int(string key, int fallback) => int.TryParse(Get(key), out var v) ? v : fallback;

        var s = new TutorSettings
        {
            Theme = Text("APP_THEME", "Light"),
            ActiveModel = Get("SELECTED_MODEL") is "Claude" or "DeepSeek" or "Gemini" ? Get("SELECTED_MODEL")! : "ChatGPT",
            EnterToSend = DefaultTrue("ENTER_TO_SEND"),

            UserDisplayName = Text("USER_DISPLAY_NAME", "User"),
            UserAvatarIcon = Text("USER_AVATAR_ICON", "bi-person-fill"),
            UserCustomAvatar = Get("USER_CUSTOM_AVATAR") is { Length: > 0 } ua ? ua : null,
            AssistantDisplayName = Text("ASSISTANT_DISPLAY_NAME", "Assistant"),
            AssistantAvatarIcon = Text("ASSISTANT_AVATAR_ICON", "bi-robot"),
            AssistantCustomAvatar = Get("ASSISTANT_CUSTOM_AVATAR") is { Length: > 0 } aa ? aa : null,
            BorderRadius = int.TryParse(Get("BORDER_RADIUS"), out var radius) ? Math.Clamp(radius, 0, 10) : 6,
            ParagraphSpacing = double.TryParse(Get("PARAGRAPH_SPACING"), out var spacing) ? Math.Clamp(spacing, 0.5, 2.0) : 1.0,
            DateFormat = Text("DATE_FORMAT", DefaultDateFormat),

            ReadingTimeSeconds = Int("ReadingTimeSeconds", 5),
            UseCalculatedReadingTime = bool.TryParse(Get("UseCalculatedReadingTime"), out var calc) && calc,

            QuizQuestionCount = Int("QUIZ_QUESTION_COUNT", 3),
            GradingScale = Enum.TryParse<GradingScale>(Get("GRADING_SCALE"), out var scale) ? scale : GradingScale.Standard,
            OrphanLinkingMaxIterations = Int("ORPHAN_LINKING_MAX_ITERATIONS", 10),
            OrphanLinkingMinConfidence = float.TryParse(Get("ORPHAN_LINKING_MIN_CONFIDENCE"), out var conf)
                ? Math.Round((double)conf, 4) : 0.3,

            LogTrace = Get("LOG_TRACE") == "true",
            LogDebug = DefaultTrue("LOG_DEBUG"),
            LogInfo = DefaultTrue("LOG_INFO"),
            LogWarning = DefaultTrue("LOG_WARNING"),
            LogError = DefaultTrue("LOG_ERROR"),
            LogCritical = DefaultTrue("LOG_CRITICAL"),
        };

        if (storage is not null)
        {
            s.LearningResourcesDirectory = string.IsNullOrWhiteSpace(storage.LearningResourcesDirectory) ? null : storage.LearningResourcesDirectory;
            s.SaveFormattedAsMarkdown = storage.SaveFormattedAsMarkdown;
            s.PreserveOriginalFiles = storage.PreserveOriginalFiles;
        }
        return s;
    }

    private static Dictionary<string, string>? ReadDictionary(string path)
    {
        if (!File.Exists(path)) return null;
        try { return JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(path)) ?? new(); }
        catch { return new(); }
    }

    private sealed class LegacyStorage
    {
        public string? LearningResourcesDirectory { get; set; }
        public bool SaveFormattedAsMarkdown { get; set; } = true;
        public bool PreserveOriginalFiles { get; set; } = true;
    }

    private static LegacyStorage? ReadStorage(string path)
    {
        if (!File.Exists(path)) return null;
        try { return JsonSerializer.Deserialize<LegacyStorage>(File.ReadAllText(path)) ?? new LegacyStorage(); }
        catch { return new LegacyStorage(); }
    }
}

/// <summary>
/// Where Tutor reads its settings at run time: the deployed configuration when present, else the
/// Vault file (re-read on every access, so an edit saved in Vault.Desktop applies on the next read).
/// </summary>
public sealed class TutorSettingsSource
{
    private readonly ISettingsStore<TutorSettings> store;
    private readonly IConfiguration? configuration;

    public TutorSettingsSource(ISettingsStore<TutorSettings> store, IConfiguration? configuration = null)
    {
        this.store = store ?? throw new ArgumentNullException(nameof(store));
        this.configuration = configuration;
    }

    /// <summary>The settings in effect now.</summary>
    public TutorSettings Current => TutorSettings.FromConfiguration(configuration) ?? store.Load();

    /// <summary>The backing store (for diagnostics and tests).</summary>
    public ISettingsStore<TutorSettings> Store => store;

    /// <summary>
    /// Changes a setting through the store. Only for run controls that already wrote a setting before
    /// LAMP (Tutor.Cli's <c>--llm</c>); there is no settings editor in Tutor. Ignored when the values
    /// come from deployed configuration (read-only, M.VALT-LAW-4).
    /// </summary>
    public void Update(Action<TutorSettings> change)
    {
        if (TutorSettings.FromConfiguration(configuration) is not null) return;
        store.Update(change);
    }

    /// <summary>What hosts call once at startup: imports today's values on first run, seeds, writes the manifest.</summary>
    public static TutorSettingsSource Open(IConfiguration? configuration = null, string? legacyDirectory = null)
    {
        var store = JsonSettingsStore<TutorSettings>.ForSettingsApp();
        TutorSettings.Load(configuration, store, legacyDirectory);
        return new TutorSettingsSource(store, configuration);
    }
}
