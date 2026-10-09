using Microsoft.Data.Sqlite;
using MindAttic.Log.Schema;
using Tutor.Core.Services.Logging;

namespace Tutor.Tests.Services;

/// <summary>
/// Proves MindAtticLogBridge actually bridges Tutor's existing Log.Info/Warn/Error/... call
/// sites (unchanged) into the shared MindAttic.Log pipeline, and that the existing LogStore stays
/// untouched for the live in-app viewer — not just that the package reference compiles.
/// </summary>
public class MindAtticLogBridgeTests
{
    private string logsDirectory = null!;

    [SetUp]
    public void SetUp()
    {
        logsDirectory = Path.Combine(Path.GetTempPath(), "tutor-mindattic-log-tests-" + Guid.NewGuid());
        Directory.CreateDirectory(logsDirectory);
    }

    [TearDown]
    public void TearDown()
    {
        SqliteConnection.ClearAllPools();
        try { Directory.Delete(logsDirectory, recursive: true); } catch (IOException) { }
    }

    [Test]
    public void Log_Error_Reaches_The_Shared_MindAttic_Log_Table()
    {
        Log.Store.Clear();
        try
        {
            using (new MindAtticLogBridge(logsDirectory))
            {
                Log.Error("Bridge test failure", new InvalidOperationException("boom"));
            } // Dispose unsubscribes and flushes the sink

            var dbPath = Path.Combine(logsDirectory, $"MindAttic.Log.{DateTime.UtcNow:yyyy-MM}.db");
            Assert.That(File.Exists(dbPath), Is.True, "Expected a rolled MindAttic.Log file to be created.");

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
}
