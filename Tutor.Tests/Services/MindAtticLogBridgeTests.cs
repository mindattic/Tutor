using Microsoft.Data.Sqlite;
using MindAttic.Log.Schema;
using MindAttic.Vault.Paths;
using Tutor.Core.Services.Logging;

namespace Tutor.Tests.Services;

/// <summary>
/// Proves MindAtticLogBridge actually bridges Tutor's existing Log.Info/Warn/Error/... call
/// sites (unchanged) into the shared MindAttic.Log pipeline's standalone database, and that the
/// existing LogStore stays untouched for the live in-app viewer — not just that the package
/// reference compiles. Redirects Vault's roaming root via its env-var override (M.VALT-LAW-6) so this
/// never touches the real %APPDATA%.
/// </summary>
public class MindAtticLogBridgeTests
{
    private string logsDirectory = null!;
    private string roamingRoot = null!;
    private string? originalRoamingRoot;

    [SetUp]
    public void SetUp()
    {
        logsDirectory = Path.Combine(Path.GetTempPath(), "tutor-mindattic-log-tests-" + Guid.NewGuid());
        Directory.CreateDirectory(logsDirectory);

        roamingRoot = Path.Combine(Path.GetTempPath(), "tutor-mindattic-vault-tests-" + Guid.NewGuid());
        originalRoamingRoot = Environment.GetEnvironmentVariable(VaultPaths.RoamingRootEnvVar);
        Environment.SetEnvironmentVariable(VaultPaths.RoamingRootEnvVar, roamingRoot);
    }

    [TearDown]
    public void TearDown()
    {
        SqliteConnection.ClearAllPools();
        Environment.SetEnvironmentVariable(VaultPaths.RoamingRootEnvVar, originalRoamingRoot);
        try { Directory.Delete(logsDirectory, recursive: true); } catch (IOException) { }
        try { Directory.Delete(roamingRoot, recursive: true); } catch (IOException) { }
    }

    [Test]
    public void Log_Error_Reaches_The_Standalone_MindAttic_Log_Table()
    {
        Log.Store.Clear();
        try
        {
            using (new MindAtticLogBridge(logsDirectory))
            {
                Log.Error("Bridge test failure", new InvalidOperationException("boom"));
            } // Dispose unsubscribes and flushes the sink

            var dbPath = VaultPaths.LoggingDbPath("Tutor");
            Assert.That(File.Exists(dbPath), Is.True, "Expected the standalone MindAttic.Log.db to be created.");

            using var connection = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = dbPath }.ToString());
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = $"SELECT Application, Level, Message, Exception, Category FROM {LogSchema.TableName};";
            using var reader = command.ExecuteReader();

            Assert.That(reader.Read(), Is.True, "Expected the logged error to have reached the table.");
            Assert.Multiple(() =>
            {
                Assert.That(reader.GetString(0), Is.EqualTo("Tutor"));
                Assert.That(reader.GetInt32(1), Is.EqualTo((int)MindAttic.Log.Models.LogSeverity.Error));
                Assert.That(reader.GetString(2), Does.Contain("Bridge test failure"));
                Assert.That(reader.GetString(3), Does.Contain("InvalidOperationException"));
                Assert.That(reader.IsDBNull(4), Is.False, "Expected CallingMember/FilePath to populate Category.");
            });
        }
        finally
        {
            Log.Store.Clear();
        }
    }

    [Test]
    public void Existing_LogStore_Still_Receives_Entries_Alongside_The_Bridge()
    {
        Log.Store.Clear();
        try
        {
            using (new MindAtticLogBridge(logsDirectory))
            {
                Log.Info("Still visible in the live in-app viewer");
            }

            Assert.That(Log.Store.Entries.Any(e => e.Message == "Still visible in the live in-app viewer"), Is.True);
        }
        finally
        {
            Log.Store.Clear();
        }
    }

    [Test]
    public void Constructor_Migrates_Phase1_Rolled_Files_And_Removes_Them()
    {
        var rolledFile = Path.Combine(logsDirectory, $"MindAttic.Log.{DateTime.UtcNow:yyyy-MM}.db");
        using (var connection = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = rolledFile }.ToString()))
        {
            connection.Open();
            using var create = connection.CreateCommand();
            create.CommandText = LogSchema.CreateTableSqlite;
            create.ExecuteNonQuery();
            using var insert = connection.CreateCommand();
            insert.CommandText = $"""
                INSERT INTO {LogSchema.TableName} (TimestampUtc, Level, Application, Message)
                VALUES ('2026-01-01T00:00:00.000Z', 2, 'Tutor', 'Phase 1 rolled-file row');
                """;
            insert.ExecuteNonQuery();
        }
        SqliteConnection.ClearAllPools();

        using (new MindAtticLogBridge(logsDirectory)) { }

        Assert.That(File.Exists(rolledFile), Is.False, "Expected the Phase 1 rolled file to be removed after migrating.");

        var dbPath = VaultPaths.LoggingDbPath("Tutor");
        using var verify = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = dbPath }.ToString());
        verify.Open();
        using var select = verify.CreateCommand();
        select.CommandText = $"SELECT Message FROM {LogSchema.TableName} WHERE Message = 'Phase 1 rolled-file row';";
        Assert.That(select.ExecuteScalar(), Is.EqualTo("Phase 1 rolled-file row"));
    }
}
