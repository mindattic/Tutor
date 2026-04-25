namespace Tutor.Core.Models;

/// <summary>
/// A single LLM (or LLM-shaped) provider's credentials, persisted at
/// %APPDATA%/MindAttic/LLM/{providerId}.json. Shared across every MindAttic
/// application so a credential rotation in one app propagates to all of them.
///
/// File layout per provider, e.g. claude.json:
///   { "apiKey": "...", "model": "claude-opus-4-7", "extra": { } }
///
/// Extra carries provider-specific fields that don't fit the apiKey/model
/// shape — voice IDs for ElevenLabs, app IDs for HERE Maps, regions, etc.
/// </summary>
public class LlmCredential
{
    public string ApiKey { get; set; } = "";
    public string Model { get; set; } = "";
    public Dictionary<string, string> Extra { get; set; } = new();
}
