#Requires -Version 5.1
<#
.SYNOPSIS
    Clean, test, bump, pack and publish one or more .NET solutions/projects to a NuGet feed.

.DESCRIPTION
    Runs, in order:
      1. dotnet clean  (plus a deep clean of every bin/ and obj/ under the target, unless -SkipDeepClean)
      2. dotnet restore
      3. dotnet test   - every target must pass before anything is bumped or published
      4. bump <Version> in each packable project
      5. dotnet pack -c Release
      6. dotnet nuget push (--skip-duplicate)
      7. git add / commit / tag  (never git push)

    Nothing is hard-coded: targets are supplied by name or path. A name is resolved against
    <name>.slnx, <name>.sln, then <name>.csproj, searched recursively from the current directory
    with bin/ and obj/ excluded.

.PARAMETER Target
    Solution or project names, or paths. Positional and variadic:
        .\publish-nuget.ps1 Jakar.Extensions
        .\publish-nuget.ps1 Foo.slnx Bar\Bar.csproj

.PARAMETER TestTarget
    What to run tests against. Defaults to -Target. Point this at the solution when -Target is a
    single library, so its test projects still run:
        .\publish-nuget.ps1 -Target Jakar.Extensions -TestTarget Jakar.Extensions.slnx

.PARAMETER Part
    Which version component to increment. 'Last' (the default) increments the final component of
    whatever version is already there - the bug-fix digit for a 3-part version. Components to the
    right of the bumped one are reset to 0. Prerelease and build-metadata suffixes are preserved.

.PARAMETER ApiKey
    NuGet API key. Defaults to $env:NUGET_API_KEY. Required unless -NoPush or -DryRun.

.PARAMETER DryRun
    Print every command and the planned version changes, execute none of them, write no files.

.EXAMPLE
    .\publish-nuget.ps1 Jakar.Extensions -TestTarget Jakar.Extensions.slnx -DryRun

.EXAMPLE
    $env:NUGET_API_KEY = 'oy2...'
    .\publish-nuget.ps1 Jakar.Extensions -TestTarget Jakar.Extensions.slnx
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory = $true, Position = 0, ValueFromRemainingArguments = $true)]
    [string[]] $Target,

    [string[]] $TestTarget,

    [ValidateSet('Major', 'Minor', 'Patch', 'Revision', 'Last')]
    [string] $Part = 'Last',

    [string] $Configuration = 'Release',

    [string] $OutputPath,

    [string] $Source = 'https://api.nuget.org/v3/index.json',

    [string] $ApiKey = $env:NUGET_API_KEY,

    [ValidateSet('quiet', 'minimal', 'normal', 'detailed', 'diagnostic')]
    [string] $Verbosity = 'minimal',

    [switch] $SkipTests,
    [switch] $SkipDeepClean,
    [switch] $NoGit,
    [switch] $NoPush,
    [switch] $DryRun
)

$ErrorActionPreference = 'Stop'

# PowerShell 7.4+ turns a non-zero native exit code into a terminating error when
# $ErrorActionPreference is 'Stop'. Turn that off so the explicit $LASTEXITCODE checks below are the
# single source of truth - several of them are probes that are *expected* to fail (dotnet sln list on
# an .slnx the SDK does not understand, git rev-parse outside a repo, git tag on an existing tag).
$PSNativeCommandUseErrorActionPreference = $false

# ────────────────────────────────────────────────────────────────────────────────────────────────
# Helpers
# ────────────────────────────────────────────────────────────────────────────────────────────────

function Write-Step
{
    param([string] $Message)
    Write-Host ''
    Write-Host "==> $Message" -ForegroundColor Cyan
}

function Write-Detail
{
    param([string] $Message)
    Write-Host "    $Message" -ForegroundColor DarkGray
}

function Write-Good
{
    param([string] $Message)
    Write-Host "    $Message" -ForegroundColor Green
}

function Write-Warn
{
    param([string] $Message)
    Write-Host "    $Message" -ForegroundColor Yellow
}

function Invoke-Dotnet
{
    # One explicit array, never ValueFromRemainingArguments: an advanced function would bind a bare
    # '-v' to the common -Verbose parameter and silently eat it before it reached the dotnet CLI.
    param([Parameter(Mandatory = $true, Position = 0)][string[]] $Arguments)

    Write-Detail "> dotnet $($Arguments -join ' ')"
    if ( $DryRun ) { return }

    & dotnet @Arguments
    if ( $LASTEXITCODE -ne 0 ) { throw "dotnet $($Arguments[0]) failed with exit code $LASTEXITCODE" }
}

function Resolve-TargetFile
{
    param([string] $Name)

    if ( Test-Path -LiteralPath $Name -PathType Leaf ) { return (Resolve-Path -LiteralPath $Name).Path }

    if ( Test-Path -LiteralPath $Name -PathType Container )
    {
        $inDir = @(Get-ChildItem -LiteralPath $Name -File | Where-Object { $_.Extension -eq '.slnx' -or $_.Extension -eq '.sln' -or $_.Extension -eq '.csproj' })
        if ( $inDir.Count -eq 1 ) { return $inDir[0].FullName }
        if ( $inDir.Count -gt 1 ) { throw "'$Name' is a directory holding $($inDir.Count) solution/project files. Pass one explicitly: $(($inDir | ForEach-Object { $_.Name }) -join ', ')" }
    }

    foreach ( $extension in @('.slnx', '.sln', '.csproj') )
    {
        $hits = @(Get-ChildItem -Path . -Filter "$Name$extension" -Recurse -File -ErrorAction SilentlyContinue |
                  Where-Object { $_.FullName -notmatch '[\\/](bin|obj|node_modules|\.git)[\\/]' })

        if ( $hits.Count -eq 1 ) { return $hits[0].FullName }
        if ( $hits.Count -gt 1 ) { throw "'$Name$extension' is ambiguous - $($hits.Count) matches: $(($hits | ForEach-Object { $_.FullName }) -join '; ')" }
    }

    throw "Could not resolve '$Name'. Looked for a file/directory at that path, then for $Name.slnx / $Name.sln / $Name.csproj under $(Get-Location)."
}

function Get-SolutionProject
{
    param([string] $SolutionFile)

    $directory = Split-Path -Parent $SolutionFile
    $projects  = @()

    $listed = & dotnet sln $SolutionFile list 2>&1
    if ( $LASTEXITCODE -eq 0 )
    {
        $projects = @($listed | Where-Object { "$_" -match '\.csproj\s*$' } | ForEach-Object { "$_".Trim() })
    }

    if ( $projects.Count -eq 0 )
    {
        # .slnx: <Project Path="Foo/Foo.csproj" />   .sln: Project("{..}") = "Foo", "Foo\Foo.csproj", "{..}"
        $text     = Get-Content -LiteralPath $SolutionFile -Raw
        $found    = [regex]::Matches($text, '"([^"]+\.csproj)"')
        $projects = @($found | ForEach-Object { $_.Groups[1].Value })
    }

    $resolved = @()
    foreach ( $relative in ($projects | Select-Object -Unique) )
    {
        $full = Join-Path $directory $relative
        if ( Test-Path -LiteralPath $full ) { $resolved += (Resolve-Path -LiteralPath $full).Path }
        else { Write-Warn "Listed in the solution but missing on disk, skipped: $relative" }
    }

    return $resolved
}

function Get-MsBuildProperty
{
    param([xml] $Xml, [string] $Name)

    # SDK-style projects carry no default namespace, so a plain XPath is enough. Last one wins.
    $nodes = @($Xml.SelectNodes("/Project/PropertyGroup/$Name"))
    for ( $i = $nodes.Count - 1; $i -ge 0; $i-- )
    {
        $value = $nodes[$i].InnerText
        if ( $value -and $value.Trim() ) { return $value.Trim() }
    }

    return $null
}

function Step-Version
{
    param([string] $Version, [string] $Part)

    $metadata = ''
    $core     = $Version

    $index = $core.IndexOf('+')
    if ( $index -ge 0 ) { $metadata = $core.Substring($index); $core = $core.Substring(0, $index) }

    $prerelease = ''
    $index      = $core.IndexOf('-')
    if ( $index -ge 0 ) { $prerelease = $core.Substring($index); $core = $core.Substring(0, $index) }

    $parts = @($core -split '\.')
    foreach ( $piece in $parts )
    {
        if ( $piece -notmatch '^\d+$' ) { throw "Version '$Version' is not numeric enough to bump automatically (component '$piece')." }
    }

    switch ( $Part )
    {
        'Major'    { $position = 0 }
        'Minor'    { $position = 1 }
        'Patch'    { $position = 2 }
        'Revision' { $position = 3 }
        'Last'     { $position = $parts.Count - 1 }
    }

    while ( $parts.Count -le $position ) { $parts += '0' }

    $parts[$position] = [string]([int]$parts[$position] + 1)
    for ( $i = $position + 1; $i -lt $parts.Count; $i++ ) { $parts[$i] = '0' }

    return (($parts -join '.') + $prerelease + $metadata)
}

function Set-ProjectVersion
{
    param([string] $ProjectFile, [string] $OldVersion, [string] $NewVersion)

    $bytes  = [System.IO.File]::ReadAllBytes($ProjectFile)
    $hasBom = $bytes.Length -ge 3 -and $bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF
    $text   = [System.Text.Encoding]::UTF8.GetString($bytes)
    if ( $hasBom ) { $text = $text.Substring(1) }

    $regex = [regex]::new('(<Version>\s*)' + [regex]::Escape($OldVersion) + '(\s*</Version>)')
    if ( $regex.Matches($text).Count -ne 1 ) { throw "Expected exactly one <Version>$OldVersion</Version> in $ProjectFile." }

    $text = $regex.Replace($text, "`${1}$NewVersion`${2}", 1)

    # Preserve the file's original encoding and line endings; only the version text changes.
    [System.IO.File]::WriteAllText($ProjectFile, $text, [System.Text.UTF8Encoding]::new($hasBom))
}

function Remove-BuildOutput
{
    param([string] $Root)

    $directories = @(Get-ChildItem -LiteralPath $Root -Directory -Recurse -Force -ErrorAction SilentlyContinue |
                     Where-Object { $_.Name -eq 'bin' -or $_.Name -eq 'obj' })

    if ( $directories.Count -eq 0 ) { return }

    Write-Detail "removing $($directories.Count) bin/obj directories under $Root"
    if ( $DryRun ) { return }

    foreach ( $directory in $directories )
    {
        Remove-Item -LiteralPath $directory.FullName -Recurse -Force -ErrorAction SilentlyContinue
    }
}

# ────────────────────────────────────────────────────────────────────────────────────────────────
# Resolve inputs
# ────────────────────────────────────────────────────────────────────────────────────────────────

Write-Step 'Resolving targets'

$targetFiles = @($Target | ForEach-Object { Resolve-TargetFile $_ } | Select-Object -Unique)
foreach ( $file in $targetFiles ) { Write-Detail "target: $file" }

if ( -not $TestTarget ) { $TestTarget = $Target }
$testFiles = @($TestTarget | ForEach-Object { Resolve-TargetFile $_ } | Select-Object -Unique)
if ( -not $SkipTests ) { foreach ( $file in $testFiles ) { Write-Detail "test:   $file" } }

if ( -not $OutputPath ) { $OutputPath = Join-Path (Split-Path -Parent $targetFiles[0]) 'nupkg' }
$OutputPath = [System.IO.Path]::GetFullPath($OutputPath)
Write-Detail "output: $OutputPath"

if ( -not $NoPush -and -not $DryRun -and -not $ApiKey )
{
    throw 'No API key. Set $env:NUGET_API_KEY or pass -ApiKey, or run with -NoPush / -DryRun.'
}

# Collect the packable projects up front so a missing <Version> fails before the test run, not after.
$plan = @()
foreach ( $file in $targetFiles )
{
    $projectFiles = if ( [System.IO.Path]::GetExtension($file) -eq '.csproj' ) { @($file) } else { Get-SolutionProject $file }

    foreach ( $projectFile in $projectFiles )
    {
        $xml = New-Object System.Xml.XmlDocument
        $xml.Load($projectFile)

        if ( (Get-MsBuildProperty $xml 'IsPackable') -eq 'false' ) { continue }

        $version = Get-MsBuildProperty $xml 'Version'
        if ( -not $version )
        {
            Write-Warn "no <Version> element, not packed: $(Split-Path -Leaf $projectFile)"
            continue
        }

        $packageId = Get-MsBuildProperty $xml 'PackageId'
        if ( -not $packageId ) { $packageId = [System.IO.Path]::GetFileNameWithoutExtension($projectFile) }

        $plan += [pscustomobject]@{
            ProjectFile = $projectFile
            PackageId   = $packageId
            OldVersion  = $version
            NewVersion  = Step-Version $version $Part
            Package     = $null
        }
    }
}

if ( $plan.Count -eq 0 ) { throw 'Nothing to publish: no packable project with a <Version> element was found in the targets.' }

Write-Step 'Planned version bumps'
foreach ( $item in $plan ) { Write-Detail ("{0,-40} {1}  ->  {2}" -f $item.PackageId, $item.OldVersion, $item.NewVersion) }

if ( $DryRun ) { Write-Warn 'DRY RUN - no commands will run and no files will be written.' }

# ────────────────────────────────────────────────────────────────────────────────────────────────
# 1-2. Clean and restore
# ────────────────────────────────────────────────────────────────────────────────────────────────

$allFiles = @(($targetFiles + $testFiles) | Select-Object -Unique)

Write-Step 'Cleaning'
foreach ( $file in $allFiles )
{
    Invoke-Dotnet @('clean', $file, '-c', $Configuration, '-v', $Verbosity)
    if ( -not $SkipDeepClean ) { Remove-BuildOutput (Split-Path -Parent $file) }
}

Write-Step 'Restoring'
foreach ( $file in $allFiles ) { Invoke-Dotnet @('restore', $file, '-v', $Verbosity) }

# ────────────────────────────────────────────────────────────────────────────────────────────────
# 3. Test - everything must pass before a single version is touched
# ────────────────────────────────────────────────────────────────────────────────────────────────

if ( $SkipTests )
{
    Write-Step 'Tests SKIPPED (-SkipTests)'
}
else
{
    Write-Step 'Testing'
    foreach ( $file in $testFiles ) { Invoke-Dotnet @('test', $file, '-c', $Configuration, '-v', $Verbosity, '--no-restore') }
    Write-Good 'all tests passed'
}

# ────────────────────────────────────────────────────────────────────────────────────────────────
# 4-5. Bump and pack. If packing fails nothing has been published yet, so the bumps are rolled back.
# ────────────────────────────────────────────────────────────────────────────────────────────────

Write-Step 'Bumping versions'

$originals = @{}
foreach ( $item in $plan )
{
    Write-Detail "$($item.PackageId) -> $($item.NewVersion)"
    if ( $DryRun ) { continue }

    $originals[$item.ProjectFile] = [System.IO.File]::ReadAllBytes($item.ProjectFile)
    Set-ProjectVersion $item.ProjectFile $item.OldVersion $item.NewVersion
}

try
{
    Write-Step "Packing ($Configuration)"
    foreach ( $item in $plan )
    {
        Invoke-Dotnet @('pack', $item.ProjectFile, '-c', $Configuration, '-o', $OutputPath, '-v', $Verbosity, '--no-restore')

        $expected = Join-Path $OutputPath "$($item.PackageId).$($item.NewVersion).nupkg"
        if ( $DryRun ) { $item.Package = $expected; continue }

        if ( -not (Test-Path -LiteralPath $expected) ) { throw "pack succeeded but $expected is missing - check <PackageId> in $($item.ProjectFile)." }

        $item.Package = $expected
        Write-Good "packed $(Split-Path -Leaf $expected)"
    }
}
catch
{
    Write-Warn 'Pack failed - rolling the version bumps back.'
    foreach ( $file in $originals.Keys ) { [System.IO.File]::WriteAllBytes($file, $originals[$file]) }
    throw
}

# ────────────────────────────────────────────────────────────────────────────────────────────────
# 6. Push. Past this point the bumps are NOT rolled back: some packages may already be live.
# ────────────────────────────────────────────────────────────────────────────────────────────────

if ( $NoPush )
{
    Write-Step 'Push SKIPPED (-NoPush)'
}
else
{
    Write-Step "Pushing to $Source"
    foreach ( $item in $plan )
    {
        # A matching .snupkg beside the .nupkg is pushed by the client automatically.
        Write-Detail "> dotnet nuget push $(Split-Path -Leaf $item.Package) -s $Source -k *** --skip-duplicate"
        if ( $DryRun ) { continue }

        & dotnet nuget push $item.Package -s $Source -k $ApiKey --skip-duplicate
        if ( $LASTEXITCODE -ne 0 ) { throw "push failed for $($item.PackageId) $($item.NewVersion) (exit $LASTEXITCODE). Versions are already bumped on disk - fix the cause and re-run with -SkipTests." }

        Write-Good "pushed $($item.PackageId) $($item.NewVersion)"
    }
}

# ────────────────────────────────────────────────────────────────────────────────────────────────
# 7. Commit and tag. Never pushes to the remote.
# ────────────────────────────────────────────────────────────────────────────────────────────────

if ( $NoGit )
{
    Write-Step 'Git SKIPPED (-NoGit)'
}
else
{
    & git rev-parse --git-dir *> $null
    if ( $LASTEXITCODE -ne 0 )
    {
        Write-Warn 'Not a git repository - skipping commit and tag.'
    }
    else
    {
        Write-Step 'Committing and tagging'

        $summary = ($plan | ForEach-Object { "$($_.PackageId) v$($_.NewVersion)" }) -join ', '
        $files   = @($plan | ForEach-Object { $_.ProjectFile })

        Write-Detail "> git add -- $(($files | ForEach-Object { Split-Path -Leaf $_ }) -join ' ')"
        Write-Detail "> git commit -m `"Release: $summary`""

        if ( -not $DryRun )
        {
            & git add -- @files
            if ( $LASTEXITCODE -ne 0 ) { throw "git add failed with exit code $LASTEXITCODE" }

            & git commit -m "Release: $summary"
            if ( $LASTEXITCODE -ne 0 ) { throw "git commit failed with exit code $LASTEXITCODE" }
        }

        foreach ( $item in $plan )
        {
            $tag = "$($item.PackageId)-v$($item.NewVersion)"
            Write-Detail "> git tag $tag"
            if ( $DryRun ) { continue }

            & git tag $tag
            if ( $LASTEXITCODE -ne 0 ) { Write-Warn "could not create tag $tag (already exists?)" }
        }

        Write-Warn 'Not pushed. Run: git push && git push --tags'
    }
}

# ────────────────────────────────────────────────────────────────────────────────────────────────

Write-Step 'Done'
foreach ( $item in $plan ) { Write-Good ("{0,-40} {1}  ->  {2}" -f $item.PackageId, $item.OldVersion, $item.NewVersion) }
Write-Host ''
