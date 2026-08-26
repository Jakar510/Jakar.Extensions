#Requires -Version 5.1
<#
.SYNOPSIS
    Stage a repository's changes and print a Claude-written commit summary to the console.

.DESCRIPTION
    Each argument names a project or solution; its only job is to say WHICH repository to summarize.
    Staging and the diff always cover that repository in full. With no arguments the project or
    solution in the current directory is used - and if there is none, the current directory itself.
    Several arguments in the same repository collapse to one summary; arguments in different
    repositories each get their own.

    changes.txt (the `git status -v` output handed to Claude) is written to the repository root.
    The summary itself is printed, never saved.

.PARAMETER Target
    Project or solution names, or paths. Positional and variadic. A bare name is resolved against
    <name>.slnx, <name>.sln, then <name>.csproj, searched recursively from the current directory.

.EXAMPLE
    .\commitSummary.ps1

.EXAMPLE
    .\commitSummary.ps1 Jakar.Extensions

.EXAMPLE
    .\commitSummary.ps1 ..\Jakar.Shapes\Jakar.Shapes.slnx ..\Jakar.Database
#>
[CmdletBinding()]
param(
    [Parameter(Position = 0, ValueFromRemainingArguments = $true)]
    [string[]] $Target
)

# PowerShell 7.4+ turns a non-zero native exit code into a terminating error when
# $ErrorActionPreference is 'Stop'. Turn that off so the $LASTEXITCODE checks below stay in charge -
# `git rev-parse` outside a repository is a probe that is expected to fail.
$PSNativeCommandUseErrorActionPreference = $false

function Resolve-TargetDirectory
{
    param([string] $Name)

    if ( Test-Path -LiteralPath $Name -PathType Container ) { return (Resolve-Path -LiteralPath $Name).Path }
    if ( Test-Path -LiteralPath $Name -PathType Leaf ) { return (Split-Path -Parent (Resolve-Path -LiteralPath $Name).Path) }

    foreach ( $extension in @('.slnx', '.sln', '.csproj') )
    {
        $hits = @(Get-ChildItem -Path . -Filter "$Name$extension" -Recurse -File -ErrorAction SilentlyContinue |
                  Where-Object { $_.FullName -notmatch '[\\/](bin|obj|node_modules|\.git)[\\/]' })

        # Any hit is good enough: every match under one repository resolves to the same root, and the
        # scope is the whole repository either way.
        if ( $hits.Count -gt 0 ) { return $hits[0].DirectoryName }
    }

    throw "Could not resolve '$Name'. Looked for a file or directory at that path, then for $Name.slnx / $Name.sln / $Name.csproj under $(Get-Location)."
}

function Get-RepositoryRoot
{
    param([string] $Directory)

    $top = & git -C $Directory rev-parse --show-toplevel 2>$null
    if ( $LASTEXITCODE -ne 0 -or -not $top ) { throw "Not inside a git repository: $Directory" }

    return (Resolve-Path -LiteralPath ($top | Select-Object -First 1)).Path
}

# ── Work out which repositories to summarize ────────────────────────────────────────────────────

$directories = @()

if ( $Target )
{
    foreach ( $name in $Target ) { $directories += Resolve-TargetDirectory $name }
}
else
{
    $here       = (Get-Location).Path
    $candidates = @(Get-ChildItem -LiteralPath $here -File -ErrorAction SilentlyContinue |
               Where-Object { $_.Extension -eq '.slnx' -or $_.Extension -eq '.sln' -or $_.Extension -eq '.csproj' })

    if ( $candidates.Count -gt 0 ) { $directories += $candidates[0].DirectoryName }
    else { $directories += $here }
}

$roots = @($directories | ForEach-Object { Get-RepositoryRoot $_ } | Select-Object -Unique)

if ( -not (Get-Command claude -ErrorAction SilentlyContinue) )
{
    throw 'The Claude Code CLI (claude) was not found on PATH.'
}

# Kept to a single line on purpose: a multi-line argument to a native command has to survive
# PowerShell quoting and then Windows CommandLineToArgvW parsing.
$prompt = 'You are writing a git commit message. The input is the output of `git status -v` - the staged file list followed by the full staged diff. Base the message only on that diff; do not invent changes. Format it exactly like this: a one-line title on the first line, then a blank line, then one or more blocks. Each block is a single line of the form "type: description" followed by "- " bullet lines giving the details of that change. Repeat blocks as needed for unrelated changes, separating them with a blank line. Use conventional types: feat, fix, refactor, perf, test, docs, build, chore. Use the imperative mood. Output only the commit message - no preamble, no commentary, no markdown code fences.'

# ── Summarize each repository ───────────────────────────────────────────────────────────────────

foreach ( $root in $roots )
{
    Push-Location -LiteralPath $root

    try
    {
        git add *

        git status -v > changes.txt

        git diff --cached --quiet
        if ( $LASTEXITCODE -eq 0 )
        {
            Write-Host "Nothing staged in $root - no summary to generate." -ForegroundColor Yellow
            continue
        }

        $summary = Get-Content -LiteralPath changes.txt -Raw | claude -p $prompt

        if ( $LASTEXITCODE -ne 0 )
        {
            Write-Host "claude exited with code $LASTEXITCODE for $root." -ForegroundColor Red
            continue
        }

        Write-Host ''
        if ( $roots.Count -gt 1 ) { Write-Host "=== $root ===" -ForegroundColor Cyan }

        Write-Output $summary
        Write-Host ''
    }
    finally
    {
        Pop-Location
    }
}
