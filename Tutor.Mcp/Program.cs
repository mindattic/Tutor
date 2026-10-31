using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MindAttic.Vault.Configuration;
using MindAttic.Vault.DependencyInjection;
using Tutor.Core.Services;

// ── Tutor MCP server ─────────────────────────────────────────────────────
// Exposes which LLM keys Tutor uses (by name, read-only; keys are managed in
// MindAttic Vault — LAMP, TUT-T31) as Model Context Protocol tools. Stdio
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
builder.Services.AddSingleton(TutorKeys.ForCurrentUser());

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

var app = builder.Build();
await app.RunAsync();
