using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using MindAttic.Legion;
using MindAttic.Vault.Credentials;
using MindAttic.Vault.Launch;
using MindAttic.Vault.Paths;
using MindAttic.Vault.Settings;
using MindAttic.Vault.Settings.Manifest;
using Tutor.Core.Models;
using Tutor.Core.Services;
using Tutor.Tests.Fakes;

namespace Tutor.Tests.Lamp;

/// <summary>
/// LAMP (TUT-T31) app-level checks: the manifest, first-run defaults, the one-time import, deployed
/// configuration, LLM key failover through Vault's shared named lists, the app-scoped key import, the
/// Settings cog opening MindAttic Vault, and the old settings screen and key editors being gone.
/// </summary>
[TestFixture, NonParallelizable]
public class TutorLampTests
{
    private static readonly string Prefix = SettingsManifest.PrefixFor(TutorSettings.AppId);
    private static string RepoRoot => TutorSettingTests.RepoRoot;

    private static string NewDir(string kind) =>
        Directory.CreateDirectory(Path.Combine(TestVaultRoots.Root, kind + "-" + Guid.NewGuid().ToString("N"))).FullName;

    // ---- Manifest -------------------------------------------------------------------------

    [Test]
    public void Manifest_builds_with_every_field_labeled_grouped_and_helped()
    {
        var m = SettingsManifestBuilder.Build<TutorSettings>();

        Assert.Multiple(() =>
        {
            Assert.That(m.AppId, Is.EqualTo("Tutor"));
            Assert.That(m.Scope, Is.EqualTo(SettingsManifest.LocalScope), "per-machine, like the %LOCALAPPDATA% files it replaces");
            Assert.That(m.ConfigurationPrefix, Is.EqualTo(Prefix));
            foreach (var f in m.Fields)
            {
                Assert.That(f.Label, Is.Not.Empty, f.Name);
                Assert.That(f.Help, Is.Not.Null.And.Not.Empty, f.Name);
                Assert.That(f.Kind, Is.Not.EqualTo(SettingKind.Unknown), f.Name);
                Assert.That(f.Name, Is.EqualTo($"{Prefix}:{f.Property}"));
            }
            foreach (var f in m.AllFields()) Assert.That(f.Label, Is.Not.Empty, f.Name);
        });
        Assert.That(m.Fields.Select(f => f.Group).Distinct(),
            Is.EquivalentTo(new[] { "General", "Appearance", "Storage", "Learning", "Quiz", "Logging", "Sign-in security" }));
    }

    [Test]
    public void Manifest_has_no_secret_fields_and_declares_every_provider_Tutor_calls()
    {
        var m = SettingsManifestBuilder.Build<TutorSettings>();
        var secretish = new[] { "apikey", "password", "token", "secret", "pepper" };

        Assert.That(m.AllFields().Where(f => f.Kind is not SettingKind.Object && f.Kind is not SettingKind.Integer
                                              && f.Property != "currentPepperKeyId"   // the pepper's id, not the pepper
                                              && secretish.Any(w => f.Property.Contains(w, StringComparison.OrdinalIgnoreCase))),
            Is.Empty);
        Assert.That(m.Credentials.Select(c => c.ProviderId), Is.EquivalentTo(new[] { "openai", "claude", "gemini", "deepseek", "kimi" }));
        Assert.That(m.Credentials.Single(c => c.ProviderId == "openai").Required, Is.True, "embeddings need it");
    }

    // ---- Defaults / storage ----------------------------------------------------------------

    [Test]
    public void LoadOrSeed_on_an_empty_folder_seeds_exactly_todays_defaults_and_the_manifest()
    {
        var store = TestLamp.Store();
        var s = TutorSettings.Load(configuration: null, store: store, legacyDirectory: NewDir("no-legacy"));

        Assert.That(File.Exists(store.FilePath), Is.True);
        Assert.That(File.Exists(Path.Combine(store.Directory, SettingsManifest.FileName)), Is.True);
        var r = JsonNode.Parse(File.ReadAllText(store.FilePath))!;
        // The defaults each old getter returned when nothing was stored (pre-TUT-T31 SettingsService,
        // ThemeService, StorageSettings, the Settings page and LlmServiceRouter).
        Assert.Multiple(() =>
        {
            Assert.That((string)r["theme"]!, Is.EqualTo("Light"));
            Assert.That((string)r["activeModel"]!, Is.EqualTo("ChatGPT"));
            Assert.That((bool)r["enterToSend"]!, Is.True);
            Assert.That((string)r["userDisplayName"]!, Is.EqualTo("User"));
            Assert.That((string)r["userAvatarIcon"]!, Is.EqualTo("bi-person-fill"));
            Assert.That(r["userCustomAvatar"], Is.Null);
            Assert.That((string)r["assistantDisplayName"]!, Is.EqualTo("Assistant"));
            Assert.That((string)r["assistantAvatarIcon"]!, Is.EqualTo("bi-robot"));
            Assert.That(r["assistantCustomAvatar"], Is.Null);
            Assert.That((int)r["borderRadius"]!, Is.EqualTo(6));
            Assert.That((double)r["paragraphSpacing"]!, Is.EqualTo(1.0));
            Assert.That((string)r["dateFormat"]!, Is.EqualTo("yyyy-MM-dd hh:mm:ss tt"));
            Assert.That(r["learningResourcesDirectory"], Is.Null, "empty = the project/AppData Resources folder, as before");
            Assert.That((bool)r["saveFormattedAsMarkdown"]!, Is.True);
            Assert.That((bool)r["preserveOriginalFiles"]!, Is.True);
            Assert.That((int)r["readingTimeSeconds"]!, Is.EqualTo(5));
            Assert.That((bool)r["useCalculatedReadingTime"]!, Is.False);
            Assert.That((int)r["quizQuestionCount"]!, Is.EqualTo(3));
            Assert.That((int)r["gradingScale"]!, Is.EqualTo((int)GradingScale.Standard));
            Assert.That((int)r["orphanLinkingMaxIterations"]!, Is.EqualTo(SettingsService.DefaultOrphanLinkingMaxIterations));
            Assert.That((double)r["orphanLinkingMinConfidence"]!, Is.EqualTo(0.3));
            Assert.That((bool)r["logTrace"]!, Is.False);
            Assert.That((bool)r["logDebug"]! && (bool)r["logInfo"]! && (bool)r["logWarning"]! && (bool)r["logError"]! && (bool)r["logCritical"]!, Is.True);
            Assert.That((string)r["auth"]!["reset"]!["publicBaseUrl"]!, Is.EqualTo("https://localhost:7200"));
            Assert.That((int)r["auth"]!["crypto"]!["memoryKiB"]!, Is.EqualTo(new MindAttic.Authentication.Options.AuthCryptoOptions().MemoryKiB));
        });
        Assert.That(s.Theme, Is.EqualTo("Light"));
    }

    [Test]
    public void Settings_live_in_Tutors_LocalAppData_MindAttic_folder()
    {
        var store = JsonSettingsStore<TutorSettings>.ForSettingsApp();
        Assert.That(store.Directory, Is.EqualTo(VaultPaths.LocalApp("Tutor")));
        Assert.That(store.FileName, Is.EqualTo("settings.json"));
    }

    [Test]
    public void Write_settings_manifest_switch_seeds_and_exits()
    {
        var output = new StringWriter();
        Assert.That(SettingsManifestCommand.TryRun<TutorSettings>(new[] { "--write-settings-manifest" }, output), Is.True);
        Assert.That(output.ToString().Trim(), Is.EqualTo(Path.Combine(VaultPaths.LocalApp("Tutor"), SettingsManifest.FileName)));
        Assert.That(File.Exists(output.ToString().Trim()), Is.True);
        Assert.That(SettingsManifestCommand.TryRun<TutorSettings>(Array.Empty<string>()), Is.False);

        foreach (var program in new[] { "Tutor.Blazor", "Tutor.Cli" })
            Assert.That(File.ReadAllText(Path.Combine(RepoRoot, program, "Program.cs")),
                Does.Contain("SettingsManifestCommand.TryRun<TutorSettings>(args)"), program);
    }

    // ---- Import ----------------------------------------------------------------------------

    [Test]
    public void Import_never_changes_the_old_files_and_never_overwrites_an_existing_Vault_file()
    {
        var legacy = NewDir("legacy");
        var prefsFile = Path.Combine(legacy, TutorSettings.LegacyPreferencesFile);
        var storageFile = Path.Combine(legacy, TutorSettings.LegacyStorageFile);
        File.WriteAllText(prefsFile, """{"APP_THEME":"Dark","CURRENT_USER":"{}","COURSES_DATA":"[]"}""");
        File.WriteAllText(storageFile, """{"LearningResourcesDirectory":"D:\\R","SaveFormattedAsMarkdown":true,"PreserveOriginalFiles":true}""");
        var before = new[] { prefsFile, storageFile }.Select(f => Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(f)))).ToList();

        var store = TestLamp.Store();
        Assert.That(TutorSettings.Load(null, store, legacy).Theme, Is.EqualTo("Dark"));
        store.Update(s => s.Theme = "Winter");                 // edited in Vault afterwards
        File.WriteAllText(prefsFile, """{"APP_THEME":"Spring"}""");
        Assert.That(TutorSettings.Load(null, store, legacy).Theme, Is.EqualTo("Winter"), "the import runs once");

        File.WriteAllText(prefsFile, """{"APP_THEME":"Dark","CURRENT_USER":"{}","COURSES_DATA":"[]"}""");
        var after = new[] { prefsFile, storageFile }.Select(f => Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(f)))).ToList();
        Assert.That(after, Is.EqualTo(before));
        Assert.That(File.ReadAllText(store.FilePath), Does.Not.Contain("CURRENT_USER").And.Not.Contain("COURSES_DATA"),
            "per-user and content data stays in secure-preferences.json");
    }

    [Test]
    public void Unknown_or_ChatGPT_model_imports_as_ChatGPT_which_routes_to_OpenAI_as_before()
    {
        foreach (var old in new[] { "ChatGPT", "OpenAI", "Whatever" })
        {
            var legacy = NewDir("legacy");
            File.WriteAllText(Path.Combine(legacy, TutorSettings.LegacyPreferencesFile), $$"""{"SELECTED_MODEL":"{{old}}"}""");
            Assert.That(TutorSettings.Load(null, TestLamp.Store(), legacy).ActiveModel, Is.EqualTo("ChatGPT"), old);
        }
    }

    [Test]
    public void Deployed_configuration_wins_over_the_local_file()
    {
        var store = TestLamp.Store();
        store.Save(new TutorSettings { Theme = "Dark", QuizQuestionCount = 5 });
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            [$"{Prefix}:theme"] = "Summer",
            [$"{Prefix}:quizQuestionCount"] = "20",
        }).Build();

        var loaded = TutorSettings.Load(config, store);
        var source = new TutorSettingsSource(store, config);
        Assert.That(loaded.Theme, Is.EqualTo("Summer"));
        Assert.That(source.Current.QuizQuestionCount, Is.EqualTo(20));
        source.Update(s => s.Theme = "Light");
        Assert.That(store.Load().Theme, Is.EqualTo("Dark"), "deployed values are read-only (M.VALT-LAW-4)");
    }

    // ---- Keys ------------------------------------------------------------------------------

    /// <summary>Answers Anthropic's endpoint by key: "sk-bad" gets 401, anything else 200.</summary>
    private sealed class KeyedClaudeHandler : HttpMessageHandler
    {
        public List<string> KeysSeen { get; } = new();

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            var key = request.Headers.TryGetValues("x-api-key", out var v) ? v.Single() : "";
            KeysSeen.Add(key);
            return Task.FromResult(key == "sk-bad"
                ? new HttpResponseMessage(HttpStatusCode.Unauthorized) { Content = new StringContent("""{"error":{"type":"authentication_error"}}""", Encoding.UTF8, "application/json") }
                : new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("""{"content":[{"type":"text","text":"hi"}]}""", Encoding.UTF8, "application/json") });
        }
    }

    [Test]
    public async Task Llm_call_fails_over_to_the_second_key_when_the_first_is_rejected_and_flags_the_first()
    {
        var store = TestLamp.KeyStore();
        store.Add("claude", "first", "sk-bad");
        store.Add("claude", "second", "sk-good");
        var health = new KeyHealthStore(NewDir("health"));
        var keys = new TutorKeys(new KeySelector(new FileKeySource(store), health, classifier: TutorKeys.ClassifyLegionFailure));
        var handler = new KeyedClaudeHandler();
        var svc = new ClaudeService(new LegionClient(new HttpClient(handler), null), new FakeSecurePreferences(), keys);

        var reply = await svc.GetReplyAsync(new[] { new ChatMessage("user", "hello", "hello") });

        Assert.That(reply.Text, Is.EqualTo("hi"));
        Assert.That(handler.KeysSeen.Distinct(), Is.EqualTo(new[] { "sk-bad", "sk-good" }));
        Assert.That(health.Get("claude", "first").Failed, Is.True, "the refused key is flagged for Vault.Desktop");
        Assert.That(keys.KeyNames("claude"), Is.EqualTo(new[] { "second" }), "a failed key is skipped next time");
    }

    [Test]
    public async Task Llm_call_uses_Tutors_assigned_key_first()
    {
        var store = TestLamp.KeyStore();
        store.Add("claude", "shared", "sk-shared");
        store.Add("claude", "mine", "sk-mine");
        store.Assignments.Set("Tutor", "claude", new[] { "mine", "shared" });
        var handler = new KeyedClaudeHandler();
        var svc = new ClaudeService(new LegionClient(new HttpClient(handler), null), new FakeSecurePreferences(), TestLamp.Keys(store));

        await svc.GetReplyAsync(new[] { new ChatMessage("user", "hello", "hello") });

        Assert.That(handler.KeysSeen, Is.EqualTo(new[] { "sk-mine" }));
    }

    [Test]
    public void Llm_call_with_no_key_says_to_add_one_in_Vault_without_naming_any_value()
    {
        var svc = new ClaudeService(new LegionClient(new HttpClient(new KeyedClaudeHandler()), null), new FakeSecurePreferences(), TestLamp.Keys());

        var ex = Assert.ThrowsAsync<InvalidOperationException>(() => svc.GetReplyAsync(new[] { new ChatMessage("user", "x", "x") }));
        Assert.That(ex!.Message, Does.Contain("MindAttic Vault"));
        Assert.That(ex.InnerException, Is.InstanceOf<AllKeysFailedException>());
    }

    [Test]
    public void Legion_errors_wrapping_a_401_still_count_as_a_failed_key()
    {
        var wrapped = new InvalidOperationException("breaker", new HttpRequestException("401", null, HttpStatusCode.Unauthorized));
        Assert.That(TutorKeys.ClassifyLegionFailure(wrapped)?.Kind, Is.EqualTo(KeyFailureKind.Auth));
        Assert.That(TutorKeys.ClassifyLegionFailure(new InvalidOperationException("other")), Is.Null);
        Assert.That(TutorKeys.ClassifyLegionFailure(new HttpRequestException("500", null, HttpStatusCode.InternalServerError)), Is.Null);
    }

    [Test]
    public void Startup_import_moves_an_old_tutor_scoped_key_into_the_shared_list_as_Tutors_active_key()
    {
        var root = NewDir("import");
        NamedKeyStore Keys() => new(root, new KeyAssignmentStore(Path.Combine(root, KeyAssignmentStore.Bucket)));
#pragma warning disable CS0618 // the pre-LAMP per-app copy is exactly what this test recreates
        new AppScopedCredentialStore("tutor", new LlmCredentialStore(Path.Combine(root, LlmCredentialStore.Bucket)))
            .SetKey("claude", "sk-tutor-own");
#pragma warning restore CS0618
        Keys().Add("claude", "shared", "sk-shared");

        KeyImporter.ImportAppScopedFor(TutorSettings.AppId, Keys());
        var second = KeyImporter.ImportAppScopedFor(TutorSettings.AppId, Keys());

        Assert.That(TestLamp.Keys(Keys()).Selector.GetActiveKey("claude", "Tutor")?.Value, Is.EqualTo("sk-tutor-own"));
        Assert.That(second.Added, Is.Empty, "running the import again changes nothing");
        foreach (var program in new[] { "Tutor.Blazor", "Tutor.Cli" })
            Assert.That(File.ReadAllText(Path.Combine(RepoRoot, program, "Program.cs")),
                Does.Contain("KeyImporter.ImportAppScopedFor(TutorSettings.AppId)"), program);
    }

    [Test]
    public void No_per_app_key_store_or_key_editor_is_left_in_Tutor()
    {
        var offenders = new[] { "Tutor.Core", "Tutor.Blazor", "Tutor.Cli", "Tutor.Mcp", "Tutor.Shared" }
            .SelectMany(p => Directory.EnumerateFiles(Path.Combine(RepoRoot, p), "*.*", SearchOption.AllDirectories))
            .Where(f => f.EndsWith(".cs") || f.EndsWith(".razor"))
            .Where(f => !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}") && !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}"))
            .Where(f => string.Join('\n', File.ReadAllLines(f).Where(l => !l.TrimStart().StartsWith("//"))) is var t && (t.Contains("AppScopedCredentialStore") || t.Contains("ApiKeyPoolService")
                        || t.Contains("KeyPoolFailover") || t.Contains("GetApiKeysAsync") || t.Contains("SetApiKeysAsync") || t.Contains("_API_KEY\"")))
            .Select(Path.GetFileName);
        Assert.That(offenders, Is.Empty);

        var mcp = File.ReadAllText(Path.Combine(RepoRoot, "Tutor.Mcp", "Tools.ApiKeys.cs"));
        Assert.That(mcp, Does.Not.Contain("SetApiKeys").And.Not.Contain("AddApiKey").And.Not.Contain("RemoveApiKey").And.Not.Contain("ClearApiKeys"));
    }

    // ---- Cog / old UI -----------------------------------------------------------------------

    private sealed class FakeLauncher : IVaultDesktopLauncher
    {
        public bool Running { get; init; } = true;
        public List<VaultOpenRequest> Requests { get; } = new();
        public bool TrySend(VaultOpenRequest request) { Requests.Add(request); return Running; }
        public bool TryStart(VaultOpenRequest request) => false;
    }

    private static T WithLauncher<T>(FakeLauncher fake, Func<T> body)
    {
        var original = VaultSettings.Launcher;
        VaultSettings.Launcher = fake;
        try { return body(); }
        finally { VaultSettings.Launcher = original; }
    }

    [Test]
    public void Settings_cog_opens_Vault_at_the_Tutor_tab()
    {
        var fake = new FakeLauncher();
        var result = WithLauncher(fake, () => SettingsCog.Open());

        Assert.That(result.Opened, Is.True);
        Assert.That(fake.Requests.Single().AppId, Is.EqualTo("Tutor"));
        Assert.That(fake.Requests.Single().Setting, Is.Null);
    }

    [Test]
    public void Settings_cog_can_deep_link_to_one_setting()
    {
        var fake = new FakeLauncher();
        WithLauncher(fake, () => SettingsCog.Open(SettingsCog.FullName(nameof(TutorSettings.QuizQuestionCount))));

        Assert.That(fake.Requests.Single().Setting, Is.EqualTo($"{Prefix}:quizQuestionCount"));
        Assert.That(SettingsManifestBuilder.Build<TutorSettings>().Find(fake.Requests.Single().Setting!), Is.Not.Null);
    }

    [Test]
    public void Settings_cog_explains_when_Vault_is_not_installed_and_offers_the_link()
    {
        var result = WithLauncher(new FakeLauncher { Running = false }, () => SettingsCog.Open());

        Assert.That(result.Opened, Is.False);
        Assert.That(result.Message, Does.Contain("isn't installed"));
        Assert.That(SettingsCog.Link(), Is.EqualTo("mindattic-vault://open?app=Tutor"));
    }

    [Test]
    public void Top_menu_and_nav_menu_cogs_call_VaultSettings_and_no_longer_link_to_a_settings_page()
    {
        foreach (var menu in new[] { "TopMenu.razor", "NavMenu.razor" })
        {
            var markup = File.ReadAllText(Path.Combine(RepoRoot, "Tutor.Shared", "Components", "Layout", menu));
            Assert.That(markup, Does.Contain("SettingsCog.Open()"), menu);
            Assert.That(markup, Does.Contain("SettingsCog.Link()"), menu + " shows the mindattic-vault:// fallback");
            Assert.That(markup, Does.Not.Contain("href=\"settings\""), menu);
        }
    }

    [Test]
    public void Old_settings_screen_is_gone()
    {
        var shared = Assembly.Load("Tutor.Shared");
        var routes = shared.GetTypes().SelectMany(t => t.GetCustomAttributes<RouteAttribute>()).Select(r => r.Template).ToList();
        Assert.Multiple(() =>
        {
            Assert.That(routes, Does.Not.Contain("/settings"));
            Assert.That(shared.GetType("Tutor.Shared.Components.Pages.Settings"), Is.Null);
            Assert.That(File.Exists(Path.Combine(RepoRoot, "Tutor.Shared", "Components", "Pages", "Settings.razor")), Is.False);
            Assert.That(typeof(SettingsService).GetMethods().Where(m => m.Name.StartsWith("Set") && m.Name != "SaveLessonStateAsync").Select(m => m.Name), Is.Empty,
                "no settings setters left");
            Assert.That(typeof(ThemeService).GetMethod("SetThemeAsync"), Is.Null);
            Assert.That(typeof(SideNavService).GetMethod("PopulateSettingsNav"), Is.Null);
            Assert.That(routes, Does.Contain("/maintenance"), "the non-settings admin tools (logs, data folders) moved here");
        });
    }

    [Test]
    public async Task Cli_keys_command_refuses_to_edit_keys_and_lists_names_only()
    {
        var store = TestLamp.KeyStore();
        store.Add("openai", "work key", "sk-secret-value");
        var cmd = new Tutor.Cli.Commands.KeysCommand(TestLamp.Keys(store));
        var outWriter = new StringWriter();
        var errWriter = new StringWriter();
        var (stdout, stderr) = (Console.Out, Console.Error);
        Console.SetOut(outWriter);
        Console.SetError(errWriter);
        try
        {
            Assert.That(await cmd.RunAsync(new[] { "--provider", "openai", "--set-key", "sk-x" }), Is.EqualTo(64));
            Assert.That(await cmd.RunAsync(new[] { "--provider", "openai" }), Is.EqualTo(0));
        }
        finally
        {
            Console.SetOut(stdout);
            Console.SetError(stderr);
        }
        Assert.That(errWriter.ToString(), Does.Contain("MindAttic Vault"));
        Assert.That(outWriter.ToString(), Does.Contain("work key").And.Not.Contain("sk-secret-value"));
        Assert.That(store.GetKeys("openai").Select(k => k.Value), Is.EqualTo(new[] { "sk-secret-value" }), "nothing changed");
    }
}
