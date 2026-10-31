using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using MindAttic.Legion;
using MindAttic.Vault.Configuration;
using MindAttic.Vault.Settings;
using MindAttic.Vault.Settings.Manifest;
using Tutor.Core.Services;
using Tutor.Core.Services.Abstractions;
using Tutor.Core.Services.Logging;
using Tutor.Tests.Fakes;

namespace Tutor.Tests.Lamp;

/// <summary>
/// LAMP (TUT-T31): one test per Tutor setting, each end to end — the value stored before LAMP (in
/// secure-preferences.json or storage.settings.json) imports into the right property, is saved in the
/// Vault settings.json under its manifest name and reloads unchanged, and is the value the code that
/// consumes it actually gets.
/// </summary>
[TestFixture, NonParallelizable]
public class TutorSettingTests
{
    private static readonly string Prefix = SettingsManifest.PrefixFor(TutorSettings.AppId);

    private string legacyDir = "";

    [SetUp]
    public void SetUp()
    {
        legacyDir = Path.Combine(TestVaultRoots.Root, "legacy-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(legacyDir);
    }

    // ---- helpers ------------------------------------------------------------------------

    /// <summary>Writes the pre-LAMP files, runs the one-time import into a fresh Vault store, returns the store.</summary>
    private JsonSettingsStore<TutorSettings> Import(Dictionary<string, string>? prefs = null, object? storage = null)
    {
        if (prefs is not null)
            File.WriteAllText(Path.Combine(legacyDir, TutorSettings.LegacyPreferencesFile), JsonSerializer.Serialize(prefs));
        if (storage is not null)
            File.WriteAllText(Path.Combine(legacyDir, TutorSettings.LegacyStorageFile), JsonSerializer.Serialize(storage));
        var store = TestLamp.Store();
        TutorSettings.Load(configuration: null, store: store, legacyDirectory: legacyDir);
        return store;
    }

    /// <summary>
    /// The value is in settings.json under the manifest field's property, that field's full name is
    /// <c>MindAttic:Settings:Tutor:{property}</c>, and a fresh load returns it.
    /// </summary>
    private static TutorSettings AssertStoredUnderManifestName(JsonSettingsStore<TutorSettings> store, string property, JsonNode? expected)
    {
        var field = SettingsManifestBuilder.Build<TutorSettings>().Fields.SingleOrDefault(f => f.Property == property);
        Assert.That(field, Is.Not.Null, $"manifest has no field '{property}'");
        Assert.That(field!.Name, Is.EqualTo($"{Prefix}:{property}"));
        var json = JsonNode.Parse(File.ReadAllText(store.FilePath))!.AsObject();
        Assert.That(json.ContainsKey(property), Is.True, $"settings.json has no '{property}'");
        Assert.That(json[property]?.ToJsonString(), Is.EqualTo(expected?.ToJsonString()), $"settings.json '{property}'");
        return store.Load();
    }

    private static SettingsService Service(JsonSettingsStore<TutorSettings> store) =>
        new(new TutorSettingsSource(store), new FakeSecurePreferences());

    private static void WithLogSettings(Action body)
    {
        var s = Log.Settings;
        var saved = (s.LogTrace, s.LogDebug, s.LogInfo, s.LogWarning, s.LogError, s.LogCritical);
        try { body(); }
        finally { (s.LogTrace, s.LogDebug, s.LogInfo, s.LogWarning, s.LogError, s.LogCritical) = saved; }
    }

    private static void WithStorageSource(JsonSettingsStore<TutorSettings> store, Action body)
    {
        var saved = StorageSettings.Source;
        StorageSettings.Source = new TutorSettingsSource(store);
        try { body(); }
        finally { StorageSettings.Source = saved; }
    }

    // ---- guard --------------------------------------------------------------------------

    [Test]
    public void EverySettingHasItsOwnTest()
    {
        var tested = GetType().GetMethods().Select(m => m.Name).Where(n => n.StartsWith("Setting_")).ToList();
        var fields = SettingsManifestBuilder.Build<TutorSettings>().Fields;
        Assert.That(fields, Has.Count.EqualTo(28), string.Join(", ", fields.Select(f => f.Property)));
        foreach (var field in fields)
        {
            var pascal = char.ToUpperInvariant(field.Property[0]) + field.Property[1..];
            Assert.That(tested.Count(t => t.StartsWith("Setting_" + pascal + "_")), Is.EqualTo(1), field.Property);
        }

        // The Auth group's leaves each get their own case of AuthSetting_BindsFromTutorsVaultTab.
        var leaves = AuthLeaves().Select(c => (string)c.Arguments[0]!).ToList();
        Assert.That(leaves, Has.Count.EqualTo(32));
        Assert.That(leaves.Distinct().Count(), Is.EqualTo(32));
        foreach (var name in leaves)
            Assert.DoesNotThrow(() => NonDefault(Leaf(name)), name);
    }

    // ---- Sign-in security (MindAttic.Authentication) -------------------------------------

    [Test]
    public void Setting_Auth_ImportsTodaysMindAtticAuthValuesAndRoundTrips()
    {
        var appsettings = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["MindAttic:Auth:Reset:PublicBaseUrl"] = "https://localhost:7200",
                ["MindAttic:Auth:Policy:MinLength"] = "14",
            }).Build();
        var store = TestLamp.Store();
        TutorSettings.Load(appsettings, store, legacyDir);

        var json = JsonNode.Parse(File.ReadAllText(store.FilePath))!;
        Assert.That(json["auth"]!["reset"]!["publicBaseUrl"]!.GetValue<string>(), Is.EqualTo("https://localhost:7200"));
        Assert.That(json["auth"]!["policy"]!["minLength"]!.GetValue<int>(), Is.EqualTo(14));
        Assert.That(SettingsManifestBuilder.Build<TutorSettings>().Find($"{Prefix}:auth:reset:publicBaseUrl"), Is.Not.Null,
            "manifest names it MindAttic:Settings:Tutor:auth:reset:publicBaseUrl");
        var loaded = store.Load();
        Assert.That(loaded.Auth.Reset.PublicBaseUrl, Is.EqualTo("https://localhost:7200"));
        Assert.That(loaded.Auth.Policy.MinLength, Is.EqualTo(14));
        Assert.That(new TutorSettings().Auth.Reset.PublicBaseUrl, Is.EqualTo("https://localhost:7200"),
            "first-run default = the value appsettings.json shipped");
    }

    private static readonly Dictionary<string, Type> AuthOptionTypes = new()
    {
        ["crypto"] = typeof(MindAttic.Authentication.Options.AuthCryptoOptions),
        ["policy"] = typeof(MindAttic.Authentication.Options.AuthPolicyOptions),
        ["mfa"] = typeof(MindAttic.Authentication.Options.MfaOptions),
        ["session"] = typeof(MindAttic.Authentication.Options.AuthSessionOptions),
        ["reset"] = typeof(MindAttic.Authentication.Options.AuthResetOptions),
    };

    /// <summary>One case per leaf of the manifest's Auth group (objects excluded).</summary>
    public static IEnumerable<TestCaseData> AuthLeaves() =>
        SettingsManifestBuilder.Build<TutorSettings>().AllFields()
            .Where(f => f.Name.StartsWith($"{Prefix}:auth:", StringComparison.Ordinal) && f.Kind != SettingKind.Object)
            .Select(f => new TestCaseData(f.Name).SetName($"AuthSetting_BindsFromTutorsVaultTab({f.Name[(Prefix.Length + 1)..]})"));

    private static SettingField Leaf(string name) => SettingsManifestBuilder.Build<TutorSettings>().Find(name)!;

    /// <summary>A valid value for the field that differs from its default.</summary>
    private static JsonNode NonDefault(SettingField field) => field.Kind switch
    {
        SettingKind.Integer => JsonValue.Create(field.Default!.GetValue<int>() + 1),
        SettingKind.Bool => JsonValue.Create(!field.Default!.GetValue<bool>()),
        SettingKind.TimeSpan => JsonValue.Create(TimeSpan.Parse(field.Default!.GetValue<string>()).Add(TimeSpan.FromMinutes(1)).ToString()),
        SettingKind.Url => JsonValue.Create("https://tutor-test.example/" + field.Property),
        SettingKind.Text when field.Property == "resetPath" => JsonValue.Create("/account/tutor-reset"),
        SettingKind.Text => JsonValue.Create(field.Default!.GetValue<string>() + "-tutor"),
        _ => throw new NotSupportedException($"{field.Name}: no test value for kind {field.Kind}"),
    };

    [TestCaseSource(nameof(AuthLeaves))]
    public void AuthSetting_BindsFromTutorsVaultTab(string fullName)
    {
        var field = Leaf(fullName);
        var path = fullName[(Prefix.Length + 1)..].Split(':');          // auth, group, property
        var value = NonDefault(field);

        // Tutor's Vault file in a temp local root (Local = true), with only this one value changed.
        var roaming = Path.Combine(TestVaultRoots.Root, "auth-roaming-" + Guid.NewGuid().ToString("N"));
        var local = Path.Combine(TestVaultRoots.Root, "auth-local-" + Guid.NewGuid().ToString("N"));
        var store = new JsonSettingsStore<TutorSettings>(Path.Combine(local, TutorSettings.AppId));
        store.LoadOrSeed();
        var json = JsonNode.Parse(File.ReadAllText(store.FilePath))!;
        json[path[0]]![path[1]]![path[2]] = value;
        File.WriteAllText(store.FilePath, json.ToJsonString());
        Assert.That(JsonNode.Parse(File.ReadAllText(store.FilePath))![path[0]]![path[1]]![path[2]]!.ToJsonString(),
            Is.EqualTo(value.ToJsonString()), "round-trips in settings.json");

        // The way Tutor.Blazor wires it: Vault configuration tree + SettingsSection on Tutor's auth group.
        var config = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
            .AddMindAtticVaultConfiguration(o => { o.RoamingRoot = roaming; o.LocalRoot = local; })
            .Build();
        var services = new ServiceCollection();
        services.AddLogging();
        MindAttic.Authentication.Web.MindAtticAuthExtensions.AddMindAtticAuthentication<Tutor.Core.Data.TutorAuthDbContext>(
            services, config, o => { o.AppName = "Tutor"; o.SettingsSection = TutorSettings.AuthSettingsSection; });
        using var sp = services.BuildServiceProvider();

        var optionsType = AuthOptionTypes[path[1]];
        var options = sp.GetRequiredService(typeof(Microsoft.Extensions.Options.IOptions<>).MakeGenericType(optionsType));
        var bound = options.GetType().GetProperty("Value")!.GetValue(options)!;
        var prop = optionsType.GetProperties().Single(p => p.Name.Equals(path[2], StringComparison.OrdinalIgnoreCase));
        var actual = JsonSerializer.SerializeToNode(prop.GetValue(bound), prop.PropertyType);
        Assert.That(actual!.ToJsonString(), Is.EqualTo(value.ToJsonString()), $"bound IOptions<{optionsType.Name}>.{prop.Name}");
    }

    // ---- General ------------------------------------------------------------------------

    [Test]
    public async Task Setting_Theme_ImportsRoundTripsAndIsTheThemeServiceTheme()
    {
        var store = Import(new() { ["APP_THEME"] = "Autumn" });
        Assert.That(AssertStoredUnderManifestName(store, "theme", "Autumn").Theme, Is.EqualTo("Autumn"));
        Assert.That(await new ThemeService(new TutorSettingsSource(store)).GetThemeAsync(), Is.EqualTo("Autumn"));
    }

    [Test]
    public void Setting_ActiveModel_ImportsRoundTripsAndRoutesChatToThatProvider()
    {
        var store = Import(new() { ["SELECTED_MODEL"] = "Claude" });
        Assert.That(AssertStoredUnderManifestName(store, "activeModel", "Claude").ActiveModel, Is.EqualTo("Claude"));

        var settings = new TutorSettingsSource(store);
        var services = new ServiceCollection();
        services.AddSingleton<ISecurePreferences>(new FakeSecurePreferences());
        services.AddSingleton(sp => new OpenAIOptions(sp.GetRequiredService<ISecurePreferences>()));
        services.AddLegionClient();
        services.AddTestLamp(settings);
        services.AddSingleton<OpenAIService>();
        services.AddSingleton<ClaudeService>();
        services.AddSingleton<DeepSeekService>();
        services.AddSingleton<GeminiService>();
        using var sp = services.BuildServiceProvider();
        var router = new LlmServiceRouter(settings, sp);
        Assert.That(router.GetCurrentService(), Is.InstanceOf<ClaudeService>());

        store.Update(s => s.ActiveModel = "Gemini");   // a change saved in Vault applies on the next call
        Assert.That(router.GetCurrentService(), Is.InstanceOf<GeminiService>());
    }

    [Test]
    public async Task Setting_EnterToSend_ImportsRoundTripsAndIsWhatChatReads()
    {
        var store = Import(new() { ["ENTER_TO_SEND"] = "false" });
        Assert.That(AssertStoredUnderManifestName(store, "enterToSend", false).EnterToSend, Is.False);
        Assert.That(await Service(store).GetEnterToSendAsync(), Is.False);
    }

    // ---- Appearance ---------------------------------------------------------------------

    [Test]
    public async Task Setting_UserDisplayName_ImportsRoundTripsAndIsWhatChatShows()
    {
        var store = Import(new() { ["USER_DISPLAY_NAME"] = "Ryan" });
        Assert.That(AssertStoredUnderManifestName(store, "userDisplayName", "Ryan").UserDisplayName, Is.EqualTo("Ryan"));
        Assert.That(await Service(store).GetUserDisplayNameAsync(), Is.EqualTo("Ryan"));
    }

    [Test]
    public async Task Setting_UserAvatarIcon_ImportsRoundTripsAndIsWhatChatShows()
    {
        var store = Import(new() { ["USER_AVATAR_ICON"] = "bi-rocket-takeoff-fill" });
        Assert.That(AssertStoredUnderManifestName(store, "userAvatarIcon", "bi-rocket-takeoff-fill").UserAvatarIcon, Is.EqualTo("bi-rocket-takeoff-fill"));
        Assert.That(await Service(store).GetUserAvatarIconAsync(), Is.EqualTo("bi-rocket-takeoff-fill"));
    }

    [Test]
    public async Task Setting_UserCustomAvatar_ImportsRoundTripsAndIsWhatChatShows()
    {
        const string dataUrl = "data:image/png;base64,iVBORw0KGgo=";
        var store = Import(new() { ["USER_CUSTOM_AVATAR"] = dataUrl });
        Assert.That(AssertStoredUnderManifestName(store, "userCustomAvatar", dataUrl).UserCustomAvatar, Is.EqualTo(dataUrl));
        Assert.That(await Service(store).GetUserCustomAvatarAsync(), Is.EqualTo(dataUrl));
    }

    [Test]
    public async Task Setting_AssistantDisplayName_ImportsRoundTripsAndIsWhatChatShows()
    {
        var store = Import(new() { ["ASSISTANT_DISPLAY_NAME"] = "Professor" });
        Assert.That(AssertStoredUnderManifestName(store, "assistantDisplayName", "Professor").AssistantDisplayName, Is.EqualTo("Professor"));
        Assert.That(await Service(store).GetAssistantDisplayNameAsync(), Is.EqualTo("Professor"));
    }

    [Test]
    public async Task Setting_AssistantAvatarIcon_ImportsRoundTripsAndIsWhatChatShows()
    {
        var store = Import(new() { ["ASSISTANT_AVATAR_ICON"] = "bi-magic" });
        Assert.That(AssertStoredUnderManifestName(store, "assistantAvatarIcon", "bi-magic").AssistantAvatarIcon, Is.EqualTo("bi-magic"));
        Assert.That(await Service(store).GetAssistantAvatarIconAsync(), Is.EqualTo("bi-magic"));
    }

    [Test]
    public async Task Setting_AssistantCustomAvatar_ImportsRoundTripsAndIsWhatChatShows()
    {
        const string dataUrl = "data:image/jpeg;base64,/9j/4AAQ";
        var store = Import(new() { ["ASSISTANT_CUSTOM_AVATAR"] = dataUrl });
        Assert.That(AssertStoredUnderManifestName(store, "assistantCustomAvatar", dataUrl).AssistantCustomAvatar, Is.EqualTo(dataUrl));
        Assert.That(await Service(store).GetAssistantCustomAvatarAsync(), Is.EqualTo(dataUrl));
    }

    [Test]
    public async Task Setting_BorderRadius_ImportsRoundTripsAndIsWhatTheLayoutApplies()
    {
        var store = Import(new() { ["BORDER_RADIUS"] = "9" });
        Assert.That(AssertStoredUnderManifestName(store, "borderRadius", 9).BorderRadius, Is.EqualTo(9));
        Assert.That(await Service(store).GetBorderRadiusAsync(), Is.EqualTo(9));
        Assert.That(File.ReadAllText(Path.Combine(RepoRoot, "Tutor.Shared", "Components", "Layout", "MainLayout.razor")),
            Does.Contain("GetBorderRadiusAsync"), "MainLayout applies it");
    }

    [Test]
    public async Task Setting_ParagraphSpacing_ImportsRoundTripsAndIsWhatTheLayoutApplies()
    {
        var store = Import(new() { ["PARAGRAPH_SPACING"] = 1.7.ToString("F1") });
        Assert.That(AssertStoredUnderManifestName(store, "paragraphSpacing", 1.7).ParagraphSpacing, Is.EqualTo(1.7));
        Assert.That(await Service(store).GetParagraphSpacingAsync(), Is.EqualTo(1.7));
        Assert.That(File.ReadAllText(Path.Combine(RepoRoot, "Tutor.Shared", "Components", "Layout", "MainLayout.razor")),
            Does.Contain("GetParagraphSpacingAsync"), "MainLayout applies it");
    }

    [Test]
    public async Task Setting_DateFormat_ImportsRoundTripsAndFormatsEveryDate()
    {
        var store = Import(new() { ["DATE_FORMAT"] = "yyyy-MM-dd HH:mm:ss" });
        Assert.That(AssertStoredUnderManifestName(store, "dateFormat", "yyyy-MM-dd HH:mm:ss").DateFormat, Is.EqualTo("yyyy-MM-dd HH:mm:ss"));

        var before = SettingsService.CurrentDateFormat;
        try
        {
            await WithLogSettingsAsync(() => Service(store).LoadLogSettingsAsync());
            Assert.That(SettingsService.FormatDate(new DateTime(2026, 1, 15, 18, 30, 45)), Is.EqualTo("2026-01-15 18:30:45"));
        }
        finally
        {
            await Service(TestLamp.Store()).GetDateFormatAsync();   // back to the default
            Assert.That(SettingsService.CurrentDateFormat, Is.EqualTo(before));
        }
    }

    private static async Task WithLogSettingsAsync(Func<Task> body)
    {
        var s = Log.Settings;
        var saved = (s.LogTrace, s.LogDebug, s.LogInfo, s.LogWarning, s.LogError, s.LogCritical);
        try { await body(); }
        finally { (s.LogTrace, s.LogDebug, s.LogInfo, s.LogWarning, s.LogError, s.LogCritical) = saved; }
    }

    // ---- Storage ------------------------------------------------------------------------

    [Test]
    public void Setting_LearningResourcesDirectory_ImportsRoundTripsAndIsWhereResourcesAreSaved()
    {
        var dir = Path.Combine(TestVaultRoots.Root, "resources-" + Guid.NewGuid().ToString("N"));
        var store = Import(storage: new { LearningResourcesDirectory = dir, SaveFormattedAsMarkdown = true, PreserveOriginalFiles = true });
        Assert.That(AssertStoredUnderManifestName(store, "learningResourcesDirectory", dir).LearningResourcesDirectory, Is.EqualTo(dir));

        WithStorageSource(store, () =>
        {
            Assert.That(StorageSettings.GetResolvedDirectory(), Is.EqualTo(Path.GetFullPath(dir)));
            new FileResourceService().SaveOriginalFileAsync("a.txt", "hello").GetAwaiter().GetResult();
            Assert.That(File.Exists(Path.Combine(dir, "Original", "a.txt")), Is.True);
        });
    }

    [Test]
    public void Setting_SaveFormattedAsMarkdown_ImportsRoundTripsAndControlsTheFormattedCopy()
    {
        var dir = Path.Combine(TestVaultRoots.Root, "resources-" + Guid.NewGuid().ToString("N"));
        var store = Import(storage: new { LearningResourcesDirectory = dir, SaveFormattedAsMarkdown = false, PreserveOriginalFiles = true });
        Assert.That(AssertStoredUnderManifestName(store, "saveFormattedAsMarkdown", false).SaveFormattedAsMarkdown, Is.False);

        WithStorageSource(store, () =>
        {
            var resource = new Tutor.Core.Models.CourseResource { Title = "Doc", Content = "raw", FileName = "doc.txt" };
            new FileResourceService().SaveResourceToFilesAsync(resource, "formatted").GetAwaiter().GetResult();
            Assert.That(Directory.EnumerateFiles(Path.Combine(dir, "Formatted")), Is.Empty, "no .md written when off");
            store.Update(s => s.SaveFormattedAsMarkdown = true);
            new FileResourceService().SaveResourceToFilesAsync(resource, "formatted").GetAwaiter().GetResult();
            Assert.That(File.Exists(Path.Combine(dir, "Formatted", "doc_formatted.md")), Is.True, ".md written when on");
        });
    }

    [Test]
    public void Setting_PreserveOriginalFiles_ImportsRoundTripsAndControlsTheOriginalCopy()
    {
        var dir = Path.Combine(TestVaultRoots.Root, "resources-" + Guid.NewGuid().ToString("N"));
        var store = Import(storage: new { LearningResourcesDirectory = dir, SaveFormattedAsMarkdown = true, PreserveOriginalFiles = false });
        Assert.That(AssertStoredUnderManifestName(store, "preserveOriginalFiles", false).PreserveOriginalFiles, Is.False);

        WithStorageSource(store, () =>
        {
            var resource = new Tutor.Core.Models.CourseResource { Title = "Doc", Content = "raw", FileName = "doc.txt" };
            new FileResourceService().SaveResourceToFilesAsync(resource).GetAwaiter().GetResult();
            Assert.That(File.Exists(Path.Combine(dir, "Original", "doc.txt")), Is.False, "original not kept when off");
            store.Update(s => s.PreserveOriginalFiles = true);
            new FileResourceService().SaveResourceToFilesAsync(resource).GetAwaiter().GetResult();
            Assert.That(File.Exists(Path.Combine(dir, "Original", "doc.txt")), Is.True, "original kept when on");
        });
    }

    // ---- Learning -----------------------------------------------------------------------

    [Test]
    public async Task Setting_ReadingTimeSeconds_ImportsRoundTripsAndIsTheReadingTimerTarget()
    {
        var store = Import(new() { ["ReadingTimeSeconds"] = "30" });
        Assert.That(AssertStoredUnderManifestName(store, "readingTimeSeconds", 30).ReadingTimeSeconds, Is.EqualTo(30));
        Assert.That(await Service(store).GetReadingTimeSecondsAsync(), Is.EqualTo(30));
    }

    [Test]
    public async Task Setting_UseCalculatedReadingTime_ImportsRoundTripsAndSwitchesTheReadingTimer()
    {
        var store = Import(new() { ["UseCalculatedReadingTime"] = "True" });
        Assert.That(AssertStoredUnderManifestName(store, "useCalculatedReadingTime", true).UseCalculatedReadingTime, Is.True);
        Assert.That(await Service(store).GetUseCalculatedReadingTimeAsync(), Is.True);
    }

    // ---- Quiz ---------------------------------------------------------------------------

    [Test]
    public async Task Setting_QuizQuestionCount_ImportsRoundTripsAndIsHowManyQuestionsAQuizAsks()
    {
        var store = Import(new() { ["QUIZ_QUESTION_COUNT"] = "10" });
        Assert.That(AssertStoredUnderManifestName(store, "quizQuestionCount", 10).QuizQuestionCount, Is.EqualTo(10));
        Assert.That(await Service(store).GetQuizQuestionCountAsync(), Is.EqualTo(10));
    }

    [Test]
    public async Task Setting_GradingScale_ImportsRoundTripsAndGradesQuizzes()
    {
        var store = Import(new() { ["GRADING_SCALE"] = "Strict" });
        Assert.That(AssertStoredUnderManifestName(store, "gradingScale", (int)GradingScale.Strict).GradingScale, Is.EqualTo(GradingScale.Strict));

        var service = Service(store);
        var scale = await service.GetGradingScaleAsync();
        Assert.That(scale, Is.EqualTo(GradingScale.Strict));
        Assert.That(service.GetGradeForScore(9, 10, scale), Is.EqualTo("B"), "90% is a B on the strict scale");
    }

    [Test]
    public async Task Setting_OrphanLinkingMaxIterations_ImportsRoundTripsAndIsTheGlobalLinkingLimit()
    {
        var store = Import(new() { ["ORPHAN_LINKING_MAX_ITERATIONS"] = "15" });
        Assert.That(AssertStoredUnderManifestName(store, "orphanLinkingMaxIterations", 15).OrphanLinkingMaxIterations, Is.EqualTo(15));
        var global = await Service(store).GetOrphanLinkingMaxIterationsAsync();
        Assert.That(global, Is.EqualTo(15));
        Assert.That(new Tutor.Core.Models.CourseResource().GetEffectiveMaxIterations(global), Is.EqualTo(15), "a resource without an override uses it");
    }

    [Test]
    public async Task Setting_OrphanLinkingMinConfidence_ImportsRoundTripsAndIsTheGlobalLinkingThreshold()
    {
        var store = Import(new() { ["ORPHAN_LINKING_MIN_CONFIDENCE"] = 0.5f.ToString() });
        Assert.That(AssertStoredUnderManifestName(store, "orphanLinkingMinConfidence", 0.5).OrphanLinkingMinConfidence, Is.EqualTo(0.5));
        var global = await Service(store).GetOrphanLinkingMinConfidenceAsync();
        Assert.That(global, Is.EqualTo(0.5f));
        Assert.That(new Tutor.Core.Models.CourseResource().GetEffectiveMinConfidence(global), Is.EqualTo(0.5f), "a resource without an override uses it");
    }

    // ---- Logging ------------------------------------------------------------------------

    private void AssertLogLevel(string legacyKey, string property, string legacyValue, bool expected, Func<TutorSettings, bool> read, Func<LogSettings, bool> applied)
    {
        var store = Import(new() { [legacyKey] = legacyValue });
        Assert.That(read(AssertStoredUnderManifestName(store, property, expected)), Is.EqualTo(expected));
        WithLogSettings(() =>
        {
            Service(store).LoadLogSettingsAsync().GetAwaiter().GetResult();
            Assert.That(applied(Log.Settings), Is.EqualTo(expected), "applied to Log.Settings at startup");
        });
    }

    [Test]
    public void Setting_LogTrace_ImportsRoundTripsAndIsAppliedToTheLog() =>
        AssertLogLevel("LOG_TRACE", "logTrace", "true", true, s => s.LogTrace, l => l.LogTrace);

    [Test]
    public void Setting_LogDebug_ImportsRoundTripsAndIsAppliedToTheLog() =>
        AssertLogLevel("LOG_DEBUG", "logDebug", "false", false, s => s.LogDebug, l => l.LogDebug);

    [Test]
    public void Setting_LogInfo_ImportsRoundTripsAndIsAppliedToTheLog() =>
        AssertLogLevel("LOG_INFO", "logInfo", "false", false, s => s.LogInfo, l => l.LogInfo);

    [Test]
    public void Setting_LogWarning_ImportsRoundTripsAndIsAppliedToTheLog() =>
        AssertLogLevel("LOG_WARNING", "logWarning", "false", false, s => s.LogWarning, l => l.LogWarning);

    [Test]
    public void Setting_LogError_ImportsRoundTripsAndIsAppliedToTheLog() =>
        AssertLogLevel("LOG_ERROR", "logError", "false", false, s => s.LogError, l => l.LogError);

    [Test]
    public void Setting_LogCritical_ImportsRoundTripsAndIsAppliedToTheLog() =>
        AssertLogLevel("LOG_CRITICAL", "logCritical", "false", false, s => s.LogCritical, l => l.LogCritical);

    // ---- repo root ------------------------------------------------------------------------

    internal static string RepoRoot
    {
        get
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "Tutor.slnx"))) dir = dir.Parent;
            return dir?.FullName ?? throw new InvalidOperationException("Tutor.slnx not found above the test output.");
        }
    }
}
