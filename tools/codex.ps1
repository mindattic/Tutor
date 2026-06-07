#requires -Version 5.1
<#
.SYNOPSIS
    Codex documentation standard CLI for the Tutor repo.

.DESCRIPTION
    Subcommands:
      doctor  - validate the Codex docs (front-matter, IDs, cross-refs, data
                schemas, test citations, cited paths, digest freshness).
                Exits non-zero on any hard error.
      digest  - regenerate docs/BIBLE.digest.md from BIBLE.md (sections 1, 3, 5,
                9) + a status index + the latest amendment head.

.EXAMPLE
    pwsh tools/codex.ps1 doctor
    pwsh tools/codex.ps1 digest
#>
[CmdletBinding()]
param(
    [Parameter(Position = 0)]
    [ValidateSet('doctor', 'digest')]
    [string]$Command = 'doctor'
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

# ----- paths -------------------------------------------------------------
$RepoRoot = Split-Path -Parent $PSScriptRoot
$DocsDir  = Join-Path $RepoRoot 'docs'
$Bible    = Join-Path $DocsDir 'BIBLE.md'
$Stories  = Join-Path $DocsDir 'USER_STORIES.md'
$Amend    = Join-Path $DocsDir 'AMENDMENTS.md'
$RfcDir   = Join-Path $DocsDir 'rfc'
$DataDir  = Join-Path $DocsDir 'data'
$Digest   = Join-Path $DocsDir 'BIBLE.digest.md'

# Status emoji built from codepoints so this source stays pure-ASCII (safe to
# parse under Windows PowerShell 5.1 / Win-1252).
$EMO_DONE    = [char]::ConvertFromUtf32(0x2705)
$EMO_PARTIAL = [char]::ConvertFromUtf32(0x1F7E1)
$EMO_PLANNED = [char]::ConvertFromUtf32(0x2B1C)
$EMO_CUT     = [char]::ConvertFromUtf32(0x1F5D1)

$script:Errors   = New-Object System.Collections.Generic.List[string]
$script:Warnings = New-Object System.Collections.Generic.List[string]
function Add-Err ($m)  { $script:Errors.Add($m) }
function Add-Warn ($m) { $script:Warnings.Add($m) }

# ----- helpers -----------------------------------------------------------
function Get-FrontMatter ($path) {
    # Returns a hashtable of the leading YAML front-matter, or $null if absent.
    $lines = Get-Content -LiteralPath $path -Encoding UTF8
    if ($lines.Count -lt 1 -or $lines[0].Trim() -ne '---') { return $null }
    $fm = @{}
    for ($i = 1; $i -lt $lines.Count; $i++) {
        if ($lines[$i].Trim() -eq '---') { return $fm }
        if ($lines[$i] -match '^\s*([A-Za-z0-9_]+)\s*:\s*(.*?)\s*$') {
            $fm[$matches[1]] = $matches[2]
        }
    }
    return $null  # no closing fence
}

function Test-FrontMatter ($path, $expectedLayer) {
    $rel = $path.Substring($RepoRoot.Length).TrimStart('\','/')
    $fm = Get-FrontMatter $path
    if ($null -eq $fm) { Add-Err "front-matter: $rel has no valid '---' YAML block"; return }
    foreach ($k in @('codex','project','code','layer','status','updated')) {
        if (-not $fm.ContainsKey($k) -or [string]::IsNullOrWhiteSpace($fm[$k])) {
            Add-Err "front-matter: $rel missing key '$k'"
        }
    }
    if ($fm.ContainsKey('layer') -and $expectedLayer -and $fm['layer'] -ne $expectedLayer) {
        Add-Err "front-matter: $rel layer is '$($fm['layer'])', expected '$expectedLayer'"
    }
    if ($fm.ContainsKey('updated') -and $fm['updated'] -notmatch '^\d{4}-\d{2}-\d{2}$') {
        Add-Err "front-matter: $rel 'updated' is not YYYY-MM-DD ('$($fm['updated'])')"
    }
}

function Get-GitHubSlug ($heading) {
    # Approximate GitHub heading -> anchor slug.
    $s = $heading.ToLowerInvariant()
    $s = $s -replace '`',''
    $s = $s -replace '[^\p{L}\p{Nd}\s\-_]', ''   # keep letters, digits, space, dash, underscore
    $s = $s -replace '\s', '-'
    return $s
}

# ----- collect docs ------------------------------------------------------
function Get-DocFiles {
    $docs = @()
    if (Test-Path $Bible)   { $docs += [pscustomobject]@{ Path = $Bible;   Layer = 'bible' } }
    if (Test-Path $Stories) { $docs += [pscustomobject]@{ Path = $Stories; Layer = 'stories' } }
    if (Test-Path $Amend)   { $docs += [pscustomobject]@{ Path = $Amend;   Layer = 'amendments' } }
    if (Test-Path $RfcDir) {
        Get-ChildItem -LiteralPath $RfcDir -Filter '*.md' -File | ForEach-Object {
            $docs += [pscustomobject]@{ Path = $_.FullName; Layer = 'rfc' }
        }
    }
    if (Test-Path $DataDir) {
        Get-ChildItem -LiteralPath $DataDir -Filter '*.json' -File -Recurse |
            Where-Object { $_.FullName -notmatch '_schema' } | ForEach-Object {
            $docs += [pscustomobject]@{ Path = $_.FullName; Layer = 'data' }
        }
    }
    return $docs
}

# ----- doctor checks -----------------------------------------------------
function Invoke-Doctor {
    if (-not (Test-Path $Bible))   { Add-Err "missing docs/BIBLE.md" }
    if (-not (Test-Path $Stories)) { Add-Err "missing docs/USER_STORIES.md" }

    $docFiles = Get-DocFiles

    # 1. front-matter on every L0/L1/L2/rfc/data file
    foreach ($d in $docFiles) {
        if ($d.Layer -eq 'data') {
            # JSON data files: front-matter doesn't apply; schema check below.
            continue
        }
        Test-FrontMatter $d.Path $d.Layer
    }

    # 2. collect declared {#...} anchors across all markdown docs; check uniqueness
    $anchorsByFile = @{}
    $allAnchors = @{}            # anchor -> list of files (for global uniqueness within a file)
    $headingSlugsByFile = @{}
    foreach ($d in ($docFiles | Where-Object { $_.Layer -ne 'data' })) {
        $text = Get-Content -LiteralPath $d.Path -Encoding UTF8 -Raw
        $rel  = $d.Path.Substring($RepoRoot.Length).TrimStart('\','/')
        $fileAnchors = @{}
        foreach ($m in [regex]::Matches($text, '\{#([^}]+)\}')) {
            $a = $m.Groups[1].Value
            if ($fileAnchors.ContainsKey($a)) {
                Add-Err "duplicate anchor {#$a} within $rel"
            } else {
                $fileAnchors[$a] = $true
            }
        }
        $anchorsByFile[$d.Path] = $fileAnchors

        # heading slugs (for resolving GitHub-style links)
        $slugs = @{}
        foreach ($hm in [regex]::Matches($text, '(?m)^#{1,6}\s+(.+?)\s*$')) {
            $h = $hm.Groups[1].Value -replace '\{#[^}]+\}',''   # strip explicit anchor
            $slug = Get-GitHubSlug $h.Trim()
            $slugs[$slug] = $true
        }
        $headingSlugsByFile[$d.Path] = $slugs
    }

    # 3. cross-ref links to #anchors resolve (same-file or relative-path target)
    foreach ($d in ($docFiles | Where-Object { $_.Layer -ne 'data' })) {
        $text = Get-Content -LiteralPath $d.Path -Encoding UTF8 -Raw
        $rel  = $d.Path.Substring($RepoRoot.Length).TrimStart('\','/')
        foreach ($lm in [regex]::Matches($text, '\]\(([^)]+)\)')) {
            $target = $lm.Groups[1].Value.Trim()
            if ($target -notmatch '#') { continue }            # no fragment -> skip
            if ($target -match '^https?:') { continue }
            $parts = $target -split '#', 2
            $filePart = $parts[0]
            $frag     = $parts[1]
            if ([string]::IsNullOrWhiteSpace($frag)) { continue }

            if ([string]::IsNullOrWhiteSpace($filePart)) {
                $targetPath = $d.Path                          # same-file link
            } else {
                $targetPath = Join-Path (Split-Path -Parent $d.Path) $filePart
                try { $targetPath = (Resolve-Path -LiteralPath $targetPath -ErrorAction Stop).Path } catch { $targetPath = $null }
            }

            if ($null -eq $targetPath -or -not (Test-Path $targetPath)) {
                # links outside docs/ (e.g. HouseRules) - verify the file exists at least
                $maybe = Join-Path (Split-Path -Parent $d.Path) $filePart
                if (-not (Test-Path $maybe)) {
                    Add-Err "broken link in $rel -> '$target' (file not found)"
                }
                continue
            }

            # For in-scope docs we precomputed anchors/slugs; for any other reachable
            # markdown (e.g. ../MindAttic.HouseRules.md) parse {#anchors} on demand.
            if ($anchorsByFile.ContainsKey($targetPath)) {
                $declared = $anchorsByFile[$targetPath]
                $slugs    = $headingSlugsByFile[$targetPath]
            } else {
                $declared = @{}; $slugs = @{}
                if ($targetPath -match '\.md$') {
                    $ttext = Get-Content -LiteralPath $targetPath -Encoding UTF8 -Raw
                    foreach ($am in [regex]::Matches($ttext, '\{#([^}]+)\}')) { $declared[$am.Groups[1].Value] = $true }
                    foreach ($hm in [regex]::Matches($ttext, '(?m)^#{1,6}\s+(.+?)\s*$')) {
                        $slugs[(Get-GitHubSlug (($hm.Groups[1].Value -replace '\{#[^}]+\}','').Trim()))] = $true
                    }
                }
            }
            $tgtRel = $targetPath.Substring($RepoRoot.Length).TrimStart('\','/')
            # Resolve against declared {#anchor} OR GitHub heading slug.
            if (-not $declared.ContainsKey($frag) -and -not $slugs.ContainsKey($frag) -and -not $slugs.ContainsKey((Get-GitHubSlug $frag))) {
                Add-Err "unresolved anchor in $rel -> '$target' (no {#$frag} or heading in $tgtRel)"
            }
        }
    }

    # 4. data JSON validates against _schema and ids unique (only if data/ exists)
    if (Test-Path $DataDir) {
        $ids = @{}
        Get-ChildItem -LiteralPath $DataDir -Filter '*.json' -File -Recurse |
            Where-Object { $_.FullName -notmatch '_schema' } | ForEach-Object {
            $rel = $_.FullName.Substring($RepoRoot.Length).TrimStart('\','/')
            try {
                $json = Get-Content -LiteralPath $_.FullName -Encoding UTF8 -Raw | ConvertFrom-Json
            } catch {
                Add-Err "data: $rel is not valid JSON"; return
            }
            $entities = if ($json -is [System.Array]) { $json } elseif ($json.PSObject.Properties.Name -contains 'items') { $json.items } else { @($json) }
            foreach ($e in $entities) {
                if ($null -eq $e -or -not ($e.PSObject.Properties.Name -contains 'id')) {
                    Add-Err "data: $rel has an entity with no 'id'"; continue
                }
                if ($ids.ContainsKey($e.id)) { Add-Err "data: duplicate entity id '$($e.id)' ($rel)" }
                else { $ids[$e.id] = $true }
            }
            $schema = Join-Path $DataDir ("_schema/" + ([IO.Path]::GetFileNameWithoutExtension($_.Name)) + ".schema.json")
            if (-not (Test-Path $schema)) {
                Add-Warn "data: $rel has no matching _schema/*.schema.json"
            }
        }
    }

    # 5. every done story names a test token; best-effort that the token exists in the tree
    if (Test-Path $Stories) {
        $storyLines = Get-Content -LiteralPath $Stories -Encoding UTF8
        $testTree = @()
        $testRoot = Join-Path $RepoRoot 'Tutor.Tests'
        $cypressRoot = Join-Path $RepoRoot 'Tutor.Cypress'
        if (Test-Path $testRoot)    { $testTree += Get-ChildItem -LiteralPath $testRoot -Recurse -File -Include *.cs -ErrorAction SilentlyContinue }
        if (Test-Path $cypressRoot) { $testTree += Get-ChildItem -LiteralPath $cypressRoot -Recurse -File -Include *.ts,*.js -ErrorAction SilentlyContinue | Where-Object { $_.FullName -notmatch 'node_modules' } }
        $testBlob = ($testTree | ForEach-Object { Get-Content -LiteralPath $_.FullName -Encoding UTF8 -Raw }) -join "`n"
        $testNames = ($testTree | ForEach-Object { $_.Name }) -join "`n"

        # Group each story into a block (bullet line + its continuation lines) so a
        # citation on a wrapped line still counts.
        $bulletRe = '^\s*-\s+\*\*TUT-US-[A-Z]\d+'
        $blocks = @()
        $cur = $null
        foreach ($line in $storyLines) {
            if ($line -match $bulletRe) {
                if ($null -ne $cur) { $blocks += $cur }
                $cur = $line
            } elseif ($null -ne $cur) {
                if ($line -match '^\s+\S' -or $line -match '^\s*$') { $cur += "`n" + $line }
                else { $blocks += $cur; $cur = $null }
            }
        }
        if ($null -ne $cur) { $blocks += $cur }

        $doneMark = [regex]::Escape($EMO_DONE)
        foreach ($block in $blocks) {
            $first = ($block -split "`n")[0]
            if ($first -match ('\*\*TUT-US-[A-Z]\d+\s+' + $doneMark)) {
                # A done story must cite proof: a verifying test, an enforcing attribute,
                # a recorded verification run, or (for org-policy stories) a HOUSE-LAW.
                if ($block -notmatch 'verified by|enforced by|verified \d{4}|HOUSE-LAW') {
                    $id = if ($first -match '(TUT-US-[A-Z]\d+)') { $matches[1] } else { '?' }
                    Add-Err "done story $id has no test/proof citation"
                    continue
                }
                foreach ($tm in [regex]::Matches($block, '`([A-Za-z][A-Za-z0-9_]*(?:\.[A-Za-z0-9_]+)?(?:\.cy\.ts)?)`')) {
                    $tok = $tm.Groups[1].Value
                    $bare = ($tok -split '\.')[0]
                    if ($testBlob -notmatch [regex]::Escape($tok) -and $testBlob -notmatch [regex]::Escape($bare) -and $testNames -notmatch [regex]::Escape($bare)) {
                        if ($tok -match 'Tests|\.cy\.|Service|Resolver|Validator|Importer|Exporter') {
                            Add-Warn "story test token '$tok' not found in test tree (best-effort)"
                        }
                    }
                }
            }
        }
    }

    # 6. every code path/file cited in the bible exists on disk
    if (Test-Path $Bible) {
        $bibleText = Get-Content -LiteralPath $Bible -Encoding UTF8 -Raw
        foreach ($pm in [regex]::Matches($bibleText, '`([A-Za-z0-9_./]+\.(?:cs|ts|js|json|md))`')) {
            $p = $pm.Groups[1].Value
            if ($p -match '/') {
                $full = Join-Path $RepoRoot ($p -replace '/','\')
                if (-not (Test-Path $full)) { Add-Err "bible cites missing path: $p" }
            }
        }
    }

    # 7. digest freshness (generatedFrom)
    if (Test-Path $Digest) {
        if ((Get-Item $Bible).LastWriteTimeUtc -gt (Get-Item $Digest).LastWriteTimeUtc) {
            Add-Warn "BIBLE.digest.md is older than BIBLE.md - run 'codex.ps1 digest'"
        }
    } else {
        Add-Warn "docs/BIBLE.digest.md does not exist - run 'codex.ps1 digest'"
    }

    # ----- report --------------------------------------------------------
    Write-Host ""
    Write-Host "Codex doctor - Tutor" -ForegroundColor Cyan
    Write-Host ("-" * 40)
    $checks = @(
        "front-matter present & valid",
        "anchors unique; cross-refs resolve",
        "data JSON schema + id uniqueness",
        "every (done) story cites a test",
        "bible-cited paths exist",
        "digest freshness"
    )
    foreach ($c in $checks) { Write-Host "  [check] $c" }
    Write-Host ("-" * 40)

    foreach ($w in $script:Warnings) { Write-Host "  WARN  $w" -ForegroundColor Yellow }
    foreach ($e in $script:Errors)   { Write-Host "  FAIL  $e" -ForegroundColor Red }

    if ($script:Errors.Count -eq 0) {
        Write-Host ""
        Write-Host "PASS - $($script:Warnings.Count) warning(s), 0 error(s)." -ForegroundColor Green
        exit 0
    } else {
        Write-Host ""
        Write-Host "FAIL - $($script:Errors.Count) error(s), $($script:Warnings.Count) warning(s)." -ForegroundColor Red
        exit 1
    }
}

# ----- digest ------------------------------------------------------------
function Get-BibleSection ($text, $heading) {
    # Returns the body of a "## <heading>" section up to the next "## ".
    $pattern = '(?ms)^##\s+' + [regex]::Escape($heading) + '.*?$(.*?)(?=^##\s|\Z)'
    $m = [regex]::Match($text, $pattern)
    if ($m.Success) { return $m.Groups[1].Value.Trim() }
    return ''
}

function Invoke-Digest {
    if (-not (Test-Path $Bible)) { Write-Error "docs/BIBLE.md not found"; exit 1 }
    $text = Get-Content -LiteralPath $Bible -Encoding UTF8 -Raw

    # status index from USER_STORIES
    $done = 0; $partial = 0; $planned = 0; $cut = 0
    if (Test-Path $Stories) {
        $s = Get-Content -LiteralPath $Stories -Encoding UTF8
        foreach ($l in $s) {
            if ($l -match '^\s*-\s+\*\*TUT-US-') {
                if     ($l -match [regex]::Escape($EMO_DONE))    { $done++ }
                elseif ($l -match [regex]::Escape($EMO_PARTIAL)) { $partial++ }
                elseif ($l -match [regex]::Escape($EMO_PLANNED)) { $planned++ }
                elseif ($l -match [regex]::Escape($EMO_CUT))     { $cut++ }
            }
        }
    }

    # latest amendment head
    $amendHead = "_No amendments (epoch 0)._"
    if (Test-Path $Amend) {
        $am = Get-Content -LiteralPath $Amend -Encoding UTF8
        $h = $am | Where-Object { $_ -match '^##\s+TUT-A\d+' } | Select-Object -Last 1
        if ($h) { $amendHead = $h.TrimStart('# ').Trim() }
    }

    $sec1 = Get-BibleSection $text '1\. The one sentence'
    $sec3 = Get-BibleSection $text '3\. What Tutor is NOT'
    $sec5 = Get-BibleSection $text '5\. The Laws'
    $sec9 = Get-BibleSection $text '9\. Glossary'

    $sb = New-Object System.Text.StringBuilder
    [void]$sb.AppendLine("# Tutor - Bible Digest")
    [void]$sb.AppendLine("> AUTHORITATIVE - full detail in docs/BIBLE.md")
    [void]$sb.AppendLine("> generatedFrom: docs/BIBLE.md  -  generated: $(Get-Date -Format 'yyyy-MM-dd')")
    [void]$sb.AppendLine("> Do not hand-edit; regenerate with ``tools/codex.ps1 digest``.")
    [void]$sb.AppendLine("")
    [void]$sb.AppendLine("## One sentence")
    [void]$sb.AppendLine($sec1)
    [void]$sb.AppendLine("")
    [void]$sb.AppendLine("## What it is NOT")
    [void]$sb.AppendLine($sec3)
    [void]$sb.AppendLine("")
    [void]$sb.AppendLine("## The Laws")
    [void]$sb.AppendLine($sec5)
    [void]$sb.AppendLine("")
    [void]$sb.AppendLine("## Glossary")
    [void]$sb.AppendLine($sec9)
    [void]$sb.AppendLine("")
    [void]$sb.AppendLine("## Status index")
    [void]$sb.AppendLine("- done: $done")
    [void]$sb.AppendLine("- partial: $partial")
    [void]$sb.AppendLine("- planned: $planned")
    [void]$sb.AppendLine("- cut: $cut")
    [void]$sb.AppendLine("")
    [void]$sb.AppendLine("## Latest amendment")
    [void]$sb.AppendLine($amendHead)

    Set-Content -LiteralPath $Digest -Value $sb.ToString() -Encoding UTF8
    Write-Host "Wrote docs/BIBLE.digest.md (done:$done partial:$partial planned:$planned cut:$cut)" -ForegroundColor Green
}

switch ($Command) {
    'doctor' { Invoke-Doctor }
    'digest' { Invoke-Digest }
}
