using Microsoft.Extensions.DependencyInjection;
using MindAttic.Vault.Credentials;
using MindAttic.Vault.Paths;
using MindAttic.Vault.Settings;
using Tutor.Core.Services;

/// <summary>
/// Assembly-wide (global namespace, so it wraps every fixture): every test sees temp Vault roots, so no
/// test can read or write the real %APPDATA% / %LOCALAPPDATA% MindAttic folders (M.VALT-LAW-6), and the
/// learning-resources folder points at a temp folder instead of the real one.
/// </summary>
[SetUpFixture]
public sealed class TestVaultRoots
{
    public static string Root { get; private set; } = "";

    [OneTimeSetUp]
    public void SetUp()
    {
        Root = Path.Combine(Path.GetTempPath(), "TutorVault-" + Guid.NewGuid().ToString("N"));
        Environment.SetEnvironmentVariable(VaultPaths.RoamingRootEnvVar, Path.Combine(Root, "roaming"));
        Environment.SetEnvironmentVariable(VaultPaths.LocalRootEnvVar, Path.Combine(Root, "local"));
        StorageSettings.Source = Tutor.Tests.TestLamp.Settings(s => s.LearningResourcesDirectory = Path.Combine(Root, "Resources"));
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        Environment.SetEnvironmentVariable(VaultPaths.RoamingRootEnvVar, null);
        Environment.SetEnvironmentVariable(VaultPaths.LocalRootEnvVar, null);
        try { Directory.Delete(Root, recursive: true); } catch { }
    }
}

namespace Tutor.Tests
{
    /// <summary>Tutor settings and keys over throwaway folders under <see cref="TestVaultRoots.Root"/>.</summary>
    public static class TestLamp
    {
        private static string NewDir(string kind) =>
            Directory.CreateDirectory(Path.Combine(TestVaultRoots.Root, kind + "-" + Guid.NewGuid().ToString("N"))).FullName;

        /// <summary>A settings store in its own temp folder.</summary>
        public static JsonSettingsStore<TutorSettings> Store() => new(NewDir("settings"));

        /// <summary>A settings source over its own temp store, optionally pre-saved with changes.</summary>
        public static TutorSettingsSource Settings(Action<TutorSettings>? change = null)
        {
            var store = Store();
            if (change is not null) store.Update(change);
            return new TutorSettingsSource(store);
        }

        /// <summary>A named-key store (keys + assignments) in its own temp roaming root.</summary>
        public static NamedKeyStore KeyStore()
        {
            var root = NewDir("keys");
            return new NamedKeyStore(root, new KeyAssignmentStore(Path.Combine(root, KeyAssignmentStore.Bucket)));
        }

        /// <summary>Tutor's key access over <paramref name="store"/> (a fresh empty store when null).</summary>
        public static TutorKeys Keys(NamedKeyStore? store = null) =>
            new(new KeySelector(new FileKeySource(store ?? KeyStore()), classifier: TutorKeys.ClassifyLegionFailure));

        /// <summary>Tutor's key access holding one key per (provider, value) pair.</summary>
        public static TutorKeys KeysWith(params (string Provider, string Value)[] keys)
        {
            var store = KeyStore();
            var i = 0;
            foreach (var (provider, value) in keys) store.Add(provider, $"key{++i}", value);
            return Keys(store);
        }

        /// <summary>Registers temp-folder settings and empty keys, the way the hosts register the real ones.</summary>
        public static IServiceCollection AddTestLamp(this IServiceCollection services, TutorSettingsSource? settings = null, TutorKeys? keys = null)
        {
            services.AddSingleton(settings ?? Settings());
            services.AddSingleton(keys ?? Keys());
            return services;
        }
    }
}
