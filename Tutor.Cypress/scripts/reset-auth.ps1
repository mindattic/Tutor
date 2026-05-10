<#
.SYNOPSIS
  Clears Tutor.Blazor's persisted CURRENT_USER so Cypress runs land on the
  unauthenticated home page.

.DESCRIPTION
  AuthenticationService is a singleton that seeds itself from
  %LOCALAPPDATA%\Tutor\Settings\secure-preferences.json on first AuthGuard
  navigation. Clearing the file alone is not enough — the in-memory currentUser
  outlives the file edit. So this script also stops any running Tutor.Blazor
  process; you restart the app yourself afterwards (a single `dotnet run` from
  the project root), and Cypress will then see a clean logged-out state.

.EXAMPLE
  .\scripts\reset-auth.ps1
  cd ..; dotnet run --project Tutor.Blazor --launch-profile http
  cd Tutor.Cypress; npm run cypress:run
#>

[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'

$prefsPath = Join-Path $env:LOCALAPPDATA 'Tutor\Settings\secure-preferences.json'

if (-not (Test-Path $prefsPath)) {
    Write-Host "No secure-preferences.json at $prefsPath — nothing to clear."
} else {
    $json = Get-Content -LiteralPath $prefsPath -Raw | ConvertFrom-Json
    if ($json.PSObject.Properties.Name -contains 'CURRENT_USER') {
        $json.PSObject.Properties.Remove('CURRENT_USER')
        $json | ConvertTo-Json -Depth 10 | Set-Content -LiteralPath $prefsPath -Encoding utf8
        Write-Host "Cleared CURRENT_USER from $prefsPath."
    } else {
        Write-Host "CURRENT_USER not present in $prefsPath — already clear."
    }
}

# Stop any running Tutor.Blazor host so the next `dotnet run` starts with a
# fresh AuthenticationService singleton.
$processes = Get-CimInstance Win32_Process |
    Where-Object { $_.CommandLine -match 'Tutor\.Blazor' }

if ($processes) {
    foreach ($p in $processes) {
        Write-Host "Stopping Tutor.Blazor process (PID $($p.ProcessId))."
        Stop-Process -Id $p.ProcessId -Force -ErrorAction SilentlyContinue
    }
} else {
    Write-Host "No running Tutor.Blazor process detected."
}

Write-Host ""
Write-Host "Done. Next steps:"
Write-Host "  1. cd .. ; dotnet run --project Tutor.Blazor --launch-profile http"
Write-Host "  2. cd Tutor.Cypress ; npm run cypress:run"
