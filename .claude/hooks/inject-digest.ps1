#requires -Version 5.1
<#
  SessionStart hook: injects docs/BIBLE.digest.md as authoritative context.
  Emits Claude Code SessionStart hook JSON. Non-ASCII is escaped to \uXXXX so
  the output is safe on Windows PowerShell 5.1 / Win-1252 consoles.
  If the digest is missing or empty, emits {}.
#>
[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'

$repoRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$digestPath = Join-Path $repoRoot 'docs/BIBLE.digest.md'

function Write-Empty { Write-Output '{}'; exit 0 }

if (-not (Test-Path -LiteralPath $digestPath)) { Write-Empty }
$digest = Get-Content -LiteralPath $digestPath -Encoding UTF8 -Raw
if ([string]::IsNullOrWhiteSpace($digest)) { Write-Empty }

$preamble = @"
The following is the AUTHORITATIVE Tutor project digest, generated from docs/BIBLE.md.
Treat it as the source of truth for what Tutor IS, is NOT, and its Laws. When this
digest and your assumptions disagree, the digest wins. Full detail lives in
docs/BIBLE.md; user stories in docs/USER_STORIES.md; design notes in docs/rfc/.

"@

$context = $preamble + $digest

# Build JSON, then escape every non-ASCII char to \uXXXX.
$payload = [ordered]@{
    hookSpecificOutput = [ordered]@{
        hookEventName    = 'SessionStart'
        additionalContext = $context
    }
}
$json = $payload | ConvertTo-Json -Depth 6 -Compress

$sb = New-Object System.Text.StringBuilder
foreach ($ch in $json.ToCharArray()) {
    $code = [int][char]$ch
    if ($code -gt 127) {
        [void]$sb.AppendFormat('\u{0:x4}', $code)
    } else {
        [void]$sb.Append($ch)
    }
}
Write-Output $sb.ToString()
