using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MindAttic.Vault.Configuration;
using MindAttic.Vault.DependencyInjection;
using Tutor.Cli.Services;
using Tutor.Core.Services;
using Tutor.Core.Services.Abstractions;

// ── Tutor MCP server ─────────────────────────────────────────────────────
// Exposes the BYO API-key-pool management (list/set/add/remove/clear, per LLM
// provider) as Model Context Protocol tools, so an assistant can manage Tutor's
// own key rotation without a human running `tutor keys` by hand. Stdio
// transport — launched per-session by the MCP client.
// ──────────────────────────────────────────────────────────────────────────

var builder = Host.CreateApplicationBuilder(args);

// Console output is reserved for the MCP wire protocol — route framework logs
// to stderr instead of the default console provider.
builder.Logging.ClearProviders();
builder.Logging.AddConsole(o => o.LogToStandardErrorThreshold = Microsoft.Extensions.Logging.LogLevel.Trace);

// Cloud-native credential resolution via MindAttic.Vault, same as Tutor.Cli.
var configuration = new ConfigurationBuilder()
    .AddMindAtticVaultFiles()
    .AddEnvironmentVariables()
    .Build();
builder.Services.AddMindAtticVault(configuration);
builder.Services.AddSingleton<ISecurePreferences, CliSecurePreferences>();
builder.Services.AddSingleton<ApiKeyPoolService>();

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

var app = builder.Build();
await app.RunAsync();
