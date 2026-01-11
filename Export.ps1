# =====================================================================
# Export Unity project sources into one structured text bundle
# Default: ONLY .cs files
# Optional: Uncomment the extra extensions below to include prefabs/meta/scenes
# Output: ExportedScripts.txt in the current directory
# =====================================================================

$projectName = 'Tutor'
$ErrorActionPreference = 'Stop'
$startTime = Get-Date
Write-Host "Starting .cs export..." -ForegroundColor Cyan

function Get-RelativePath {
    param([string]$Base, [string]$Child)
    $baseClean = $Base.TrimEnd('\')
    $childFull = (Resolve-Path -LiteralPath $Child).Path
    if ($childFull.StartsWith($baseClean)) {
        return $childFull.Substring($baseClean.Length + 1).Replace('\','/')
    } else {
        return $childFull.Replace('\','/')
    }
}

function Get-GuidFromMetaText {
    param([string]$MetaText)
    if ([string]::IsNullOrWhiteSpace($MetaText)) { return $null }
    $m = [regex]::Match($MetaText, '(^|\r?\n)guid:\s*([a-fA-F0-9]+)')
    if ($m.Success) { return $m.Groups[2].Value }
    return $null
}

function Get-GuidForPath {
    param([string]$FullPath)
    if ([System.IO.Path]::GetExtension($FullPath).ToLower() -eq '.meta') {
        try { return Get-GuidFromMetaText -MetaText (Get-Content -LiteralPath $FullPath -Raw) } catch { return $null }
    }
    $metaPath = $FullPath + '.meta'
    if (Test-Path -LiteralPath $metaPath) {
        try { return Get-GuidFromMetaText -MetaText (Get-Content -LiteralPath $metaPath -Raw) } catch { return $null }
    }
    return $null
}

function Classify-Type {
    param([string]$Ext)
    switch ($Ext.ToLower()) {
        '.cs'     { return 'script' }
        '.prefab' { return 'prefab' }
        '.unity'  { return 'scene' }
        '.scene'  { return 'scene' }
        '.meta'   { return 'meta' }
        default   { return 'other' }
    }
}

# ---------------------------------------------------------------------
# Configuration
# ---------------------------------------------------------------------

# Only .cs by default. Uncomment extra lines to include more.
$includeExtensions = @(
    '.cs'
    # '.prefab'
    # '.meta'
    # '.unity'
    # '.scene'
)

# Common noisy folders to skip, including Packages
$excludeDirs = @(
    '\Library\',
    '\Temp\',
    '\Logs\',
    '\Obj\',
    '\obj\',
    '\.git\',
    '\.vs\',
    '\Build\',
    '\Builds\',
    '\Packages\'   # Skip Unity Packages folder
)

# Optional: limit search to Assets only to keep things small.
# Set to $true to restrict to Assets; $false scans the whole repo.
$assetsOnly = $false

# ---------------------------------------------------------------------
# Output
# ---------------------------------------------------------------------
$out  = Join-Path -Path (Get-Location).Path -ChildPath 'ExportedScripts.txt'
$root = (Get-Location).Path

if (Test-Path -LiteralPath $out) { Remove-Item -LiteralPath $out -Force }

Write-Host "Writing header..." -ForegroundColor Yellow
'== ' + $projectName  + ' EXPORT =='               | Set-Content -LiteralPath $out -Encoding UTF8
'VERSION: 1'                          | Add-Content -LiteralPath $out -Encoding UTF8
('CREATED_UTC: ' + [DateTime]::UtcNow.ToString('yyyy-MM-ddTHH:mm:ssZ')) | Add-Content -LiteralPath $out -Encoding UTF8
('ROOT: ' + $root.Replace('\','/'))   | Add-Content -LiteralPath $out -Encoding UTF8
''                                    | Add-Content -LiteralPath $out -Encoding UTF8

# ---------------------------------------------------------------------
# Discover files
# ---------------------------------------------------------------------
Write-Host "Scanning for files..." -ForegroundColor Yellow

$searchPath = if ($assetsOnly) { Join-Path $root 'Assets' } else { $root }
if ($assetsOnly -and -not (Test-Path -LiteralPath $searchPath)) {
    Write-Host "Assets folder not found; falling back to project root." -ForegroundColor DarkYellow
    $searchPath = $root
}

$all = Get-ChildItem -Path $searchPath -Recurse -File | Where-Object {
    $extOk = $includeExtensions -contains $_.Extension.ToLower()
    if (-not $extOk) { return $false }

    $full = $_.FullName

    # Hard guard to exclude any path segment named Packages regardless of casing or slashes
    if ($full -match '([\\/])Packages\1') { return $false }

    foreach ($ex in $excludeDirs) {
        if ($full -like ('*' + $ex + '*')) { return $false }
    }
    return $true
} | Sort-Object FullName

if (-not $all -or $all.Count -eq 0) {
    Write-Host "No matching files found." -ForegroundColor Red
    'No matching files found.' | Add-Content -LiteralPath $out -Encoding UTF8
    $elapsed = (Get-Date) - $startTime
    Write-Host ("Elapsed time: {0:hh\:mm\:ss}" -f $elapsed) -ForegroundColor Cyan
    exit 0
}

Write-Host ("Found {0} file(s)." -f $all.Count) -ForegroundColor Green

# ---------------------------------------------------------------------
# Build manifest
# ---------------------------------------------------------------------
Write-Host "Building manifest..." -ForegroundColor Yellow
$manifest = @()
foreach ($f in $all) {
    $full   = $f.FullName
    $rel    = Get-RelativePath -Base $root -Child $full
    $hash   = (Get-FileHash -Algorithm SHA256 -LiteralPath $full).Hash
    $bytes  = $f.Length
    $ext    = $f.Extension
    $type   = Classify-Type -Ext $ext
    $guid   = if ($ext -ieq '.cs') { $null } else { Get-GuidForPath -FullPath $full }
    $lines  = 0
    try { $lines = (Get-Content -LiteralPath $full | Measure-Object -Line).Lines } catch { $lines = 0 }
    $manifest += [pscustomobject]@{
        path   = $rel
        sha256 = $hash
        bytes  = $bytes
        lines  = $lines
        ext    = $ext
        type   = $type
        guid   = $guid
    }
}

'<<<MANIFEST JSON START>>>' | Add-Content -LiteralPath $out -Encoding UTF8
($manifest | ConvertTo-Json -Depth 6) | Add-Content -LiteralPath $out -Encoding UTF8
'<<<MANIFEST JSON END>>>'   | Add-Content -LiteralPath $out -Encoding UTF8
''                          | Add-Content -LiteralPath $out -Encoding UTF8

# ---------------------------------------------------------------------
# Write file blocks
# ---------------------------------------------------------------------
Write-Host "Writing file contents..." -ForegroundColor Yellow
$counter = 0
foreach ($f in $all) {
    $counter++
    Write-Host ("[{0}/{1}] {2}" -f $counter, $all.Count, $f.FullName) -ForegroundColor DarkGray

    $full   = $f.FullName
    $rel    = Get-RelativePath -Base $root -Child $full
    $hash   = (Get-FileHash -Algorithm SHA256 -LiteralPath $full).Hash
    $bytes  = $f.Length
    $ext    = $f.Extension
    $type   = Classify-Type -Ext $ext
    $guid   = if ($ext -ieq '.cs') { $null } else { Get-GuidForPath -FullPath $full }
    $raw    = ''
    try { $raw = Get-Content -LiteralPath $full -Raw } catch { $raw = '' }
    $lines  = 0
    try { $lines = ($raw -split '\r?\n').Count } catch { $lines = 0 }

    '<<<FILE START>>>'                 | Add-Content -LiteralPath $out -Encoding UTF8
    ('PATH: '   + $rel)               | Add-Content -LiteralPath $out -Encoding UTF8
    ('TYPE: '   + $type)              | Add-Content -LiteralPath $out -Encoding UTF8
    ('EXT: '    + $ext)               | Add-Content -LiteralPath $out -Encoding UTF8
    if ($guid) { ('GUID: ' + $guid)   | Add-Content -LiteralPath $out -Encoding UTF8 }
    ('SHA256: ' + $hash)              | Add-Content -LiteralPath $out -Encoding UTF8
    ('BYTES: '  + $bytes)             | Add-Content -LiteralPath $out -Encoding UTF8
    ('LINES: '  + $lines)             | Add-Content -LiteralPath $out -Encoding UTF8
    '<<<CONTENT START>>>'             | Add-Content -LiteralPath $out -Encoding UTF8
    $raw                               | Add-Content -LiteralPath $out -Encoding UTF8
    '<<<CONTENT END>>>'               | Add-Content -LiteralPath $out -Encoding UTF8
    '<<<FILE END>>>'                  | Add-Content -LiteralPath $out -Encoding UTF8
    ''                                | Add-Content -LiteralPath $out -Encoding UTF8
}

# ---------------------------------------------------------------------
# Timing
# ---------------------------------------------------------------------
$elapsed = (Get-Date) - $startTime
Write-Host ("Export complete. Output saved to {0}" -f $out) -ForegroundColor Green
Write-Host ("Elapsed time: {0:hh\:mm\:ss}" -f $elapsed) -ForegroundColor Cyan
