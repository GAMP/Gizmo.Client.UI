param(
    # Back to what was in skins\<SkinName> before Grafit was first installed.
    [switch]$Uninstall,
    # Back to the version installed before the current one.
    [switch]$Rollback,
    # Where Gizmo Server is installed. The skin goes into its skins folder.
    [string]$ServerRoot = 'C:\Program Files\NETProjects\Gizmo Server',
    # The skin's folder name = the name the Manager shows for it.
    [string]$SkinName = 'Grafit',
    # Backups and the staging folder: outside the package, so every release shares them.
    [string]$DataRoot = (Join-Path $env:ProgramData 'Grafit'),
    # Installs kept for rollback, besides the state before Grafit.
    [int]$Keep = 5
)

$ErrorActionPreference = 'Stop'

$Skins     = Join-Path $ServerRoot 'skins'
$Target    = Join-Path $Skins $SkinName
$Reference = Join-Path $Skins 'Next'
$Src       = Join-Path $PSScriptRoot 'skin'
$BackupRt  = Join-Path $DataRoot 'backup'
$Stage     = Join-Path $DataRoot 'staging'
$OldRt     = Join-Path $PSScriptRoot 'backup'

# Grafit is a skin of its own: a folder next to the stock "Next" with the same layout
# (composition.json, the two assemblies, wwwroot). Nothing of the stock skin is touched
# - a host group is switched to it in the Manager, and switched back to leave it.
#
# wwwroot\_framework (the server version's Blazor runtime) and static\ (the club's files
# served as https://static/, among them the sign-in rotator's fallback pictures) belong to
# the server: they are taken from the stock skin every time a skin is put in place -
# install, rollback or uninstall - and never kept in backups.
#
# A skin is put together in the staging folder, checked, and swapped in by renaming, so a
# locked file stops the swap before anything is destroyed. Every install keeps what it
# replaced in backup\<timestamp>\ (the old skin, or absent.txt when there was none).
# "rollback" restores the newest backup and drops it; "uninstall" restores the oldest -
# the state before Grafit - and clears the backups.

$Required = @('composition.json', 'Gizmo.Client.UI.dll', 'Gizmo.Web.Components.dll',
              'wwwroot\index.html', 'wwwroot\_framework\blazor.webview.js',
              'wwwroot\_content\Gizmo.Client.UI\vendor\fonts\fonts.css', 'static')

function Say($t, $c = 'Gray') { Write-Host $t -ForegroundColor $c }

function Copy-Tree($from, $to, [string[]]$skip = @()) {
    New-Item -ItemType Directory -Path $to -Force | Out-Null
    Get-ChildItem $from -Force -ErrorAction SilentlyContinue | ForEach-Object {
        $relative = $_.Name
        if ($skip -notcontains $relative) {
            Copy-Item $_.FullName (Join-Path $to $relative) -Recurse -Force
        }
    }
}

function Stamp {
    $s = Get-Date -Format 'yyyyMMdd-HHmmss-fff'
    while (Test-Path (Join-Path $BackupRt $s)) { Start-Sleep -Milliseconds 5; $s = Get-Date -Format 'yyyyMMdd-HHmmss-fff' }
    return $s
}

function Backups {
    Get-ChildItem $BackupRt -Directory -ErrorAction SilentlyContinue | Sort-Object Name
}

# The server's part of a skin, fresh from the stock one.
function Add-ServerParts($dir) {
    $framework = Join-Path $Reference 'wwwroot\_framework'
    $dstFramework = Join-Path $dir 'wwwroot\_framework'
    if (Test-Path $framework) {
        if (Test-Path $dstFramework) { Remove-Item $dstFramework -Recurse -Force }
        Copy-Tree $framework $dstFramework
        Say "Took wwwroot\_framework from the server's own Next skin" 'DarkGray'
    }

    $static = Join-Path $Reference 'static'
    $dstStatic = Join-Path $dir 'static'
    if (Test-Path $dstStatic) { Remove-Item $dstStatic -Recurse -Force }
    if (Test-Path $static) {
        Copy-Tree $static $dstStatic
        Say "Took static\ from the server's own Next skin" 'DarkGray'
    } else {
        New-Item -ItemType Directory -Path $dstStatic -Force | Out-Null
        Say "The stock skin has no static\ folder - an empty one is created" 'DarkGray'
    }
}

function Test-Skin($dir) {
    $missing = $Required | Where-Object { -not (Test-Path (Join-Path $dir $_)) }
    $missing | ForEach-Object { Say "Missing: $_" 'Red' }
    return -not $missing
}

# Replaces skins\<SkinName> with $Stage (or removes it when $Stage is $null). The current
# folder is renamed out of the skins folder first: if a file is locked, that rename fails
# and nothing has changed. Returns $true on success.
function Swap-In($staged) {
    $aside = Join-Path $DataRoot ("replaced-" + (Get-Date -Format 'yyyyMMdd-HHmmss-fff'))
    $movedAside = $false
    try {
        if (Test-Path $Target) {
            Move-Item $Target $aside
            $movedAside = $true
        }
        if ($staged) { Move-Item $staged $Target }
    }
    catch {
        Say "Could not replace $Target : $($_.Exception.Message)" 'Red'
        if ($movedAside -and -not (Test-Path $Target)) {
            try { Move-Item $aside $Target; Say "The previous '$SkinName' skin is back in place." 'Yellow' }
            catch { Say "The previous skin is in $aside - move it back to $Target by hand." 'Red' }
        } elseif (-not $movedAside) {
            # A failed move may have left a partial copy behind; the skin itself is untouched.
            Remove-Item $aside -Recurse -Force -ErrorAction SilentlyContinue
            Say "Nothing was changed. Close what holds the file (or stop the Gizmo service) and run it again." 'Yellow'
        }
        if ($staged) { Remove-Item $staged -Recurse -Force -ErrorAction SilentlyContinue }
        return $false
    }
    if ($movedAside) { Remove-Item $aside -Recurse -Force -ErrorAction SilentlyContinue }
    return $true
}

if (-not (Test-Path $Skins)) {
    Say "Not found: $Skins" 'Red'
    Say "Check the Gizmo Server install path (-ServerRoot)." 'Yellow'
    exit 1
}

New-Item -ItemType Directory -Path $BackupRt -Force | Out-Null

# Backups of installers before 1.2 lived next to the package; they join the shared folder.
if ((Test-Path $OldRt) -and ((Resolve-Path $OldRt).Path -ne (Resolve-Path $BackupRt).Path)) {
    Get-ChildItem $OldRt -Directory | ForEach-Object {
        $dest = Join-Path $BackupRt $_.Name
        if (-not (Test-Path $dest)) { Move-Item $_.FullName $dest }
    }
}

if (Test-Path $Stage) { Remove-Item $Stage -Recurse -Force }

# ── rollback / uninstall ────────────────────────────────────────────────────
if ($Uninstall -or $Rollback) {
    $all = @(Backups)
    if ($all.Count -eq 0) {
        Say "No backup found in $BackupRt - nothing to restore." 'Red'
        exit 1
    }

    $backup = if ($Uninstall) { $all[0] } else { $all[-1] }
    Say "Restoring from $($backup.FullName)" 'DarkGray'

    $saved = Join-Path $backup.FullName $SkinName
    if (Test-Path $saved) {
        Copy-Tree $saved $Stage @('static')
        Add-ServerParts $Stage
        if (-not (Test-Skin $Stage)) {
            Say "The backup does not make a complete skin; nothing was changed." 'Yellow'
            Remove-Item $Stage -Recurse -Force
            exit 1
        }
        if (-not (Swap-In $Stage)) { exit 1 }
        Say "The '$SkinName' skin from $($backup.Name) is back." 'Green'
    } else {
        if (-not (Swap-In $null)) { exit 1 }
        Say "There was no '$SkinName' skin then - the folder is removed." 'Green'
        Say "Host groups still pointing at '$SkinName' must be switched to another skin in the Manager." 'Yellow'
    }

    if ($Uninstall) { $all | ForEach-Object { Remove-Item $_.FullName -Recurse -Force } }
    else { Remove-Item $backup.FullName -Recurse -Force }

    Say "Restart the Gizmo Client on the machines." 'Yellow'
    exit 0
}

# ── stage ───────────────────────────────────────────────────────────────────
if (-not (Test-Path $Src)) {
    Say "Missing 'skin' folder next to the installer." 'Red'
    exit 1
}

# The shell's version travels in the assembly: "1.0.0 (Gizmo 3.0.92)". Say which one is
# going in, and warn when the server is not the release it was built for.
$dll = Join-Path $Src 'Gizmo.Client.UI.dll'
$shellVersion = (Get-Item $dll).VersionInfo.ProductVersion
$serverDll = Join-Path $ServerRoot 'GizmoService.dll'
if (Test-Path $serverDll) {
    $serverVersion = ((Get-Item $serverDll).VersionInfo.ProductVersion -split '\+')[0]
    Say "Shell $shellVersion, server Gizmo $serverVersion" 'DarkGray'
    if ($shellVersion -notmatch ('Gizmo ' + [regex]::Escape($serverVersion) + '\)')) {
        Say "This build of the shell was made for another Gizmo release than the server runs - it may not load. Rebuild against $serverVersion." 'Yellow'
    }
} else {
    Say "Shell $shellVersion" 'DarkGray'
}

Copy-Tree $Src $Stage
Add-ServerParts $Stage
if (-not (Test-Skin $Stage)) {
    Say "Nothing was installed; the server's skins folder is unchanged." 'Yellow'
    Remove-Item $Stage -Recurse -Force
    exit 1
}

# ── backup ──────────────────────────────────────────────────────────────────
$stamp  = Stamp
$backup = Join-Path $BackupRt $stamp
New-Item -ItemType Directory -Path $backup -Force | Out-Null

if (Test-Path $Target) {
    $saved = Join-Path $backup $SkinName
    try {
        Copy-Tree $Target $saved @('static')
    }
    catch {
        Say "Could not back up $Target : $($_.Exception.Message)" 'Red'
        Say "Nothing was installed; the server's skins folder is unchanged." 'Yellow'
        Remove-Item $backup, $Stage -Recurse -Force -ErrorAction SilentlyContinue
        exit 1
    }
    $savedFramework = Join-Path $saved 'wwwroot\_framework'
    if (Test-Path $savedFramework) { Remove-Item $savedFramework -Recurse -Force }
    Say "Backup: $saved" 'DarkGray'
} else {
    # A marker so a rollback knows to remove the folder rather than restore it.
    Set-Content (Join-Path $backup 'absent.txt') "No '$SkinName' skin existed before $stamp."
    Say "No previous '$SkinName' skin - first install." 'DarkGray'
}

# ── install ─────────────────────────────────────────────────────────────────
if (-not (Swap-In $Stage)) {
    Remove-Item $backup -Recurse -Force -ErrorAction SilentlyContinue
    exit 1
}

# The state before Grafit (the oldest backup) and the last $Keep installs are kept.
$all = @(Backups)
if ($all.Count -gt $Keep + 1) {
    $all[1..($all.Count - $Keep - 1)] | ForEach-Object { Remove-Item $_.FullName -Recurse -Force }
}

Say "Installed skin: $Target" 'DarkGray'
Say ""
Say "Done. Skin '$SkinName' ($shellVersion) is installed next to the stock one." 'Green'
Say "Next: Manager -> the PC's host group -> Skin = '$SkinName' (a group's own Skin beats the server default)," 'Yellow'
Say "      then restart the Gizmo Client on a PC of that group (the whole client, not a sign-out):" 'Yellow'
Say "      the client receives its skin name and the skin files only when it connects." 'Yellow'
Say "Check on the PC: %PROGRAMDATA%\NETProjects\Gizmo Client\Skins\$SkinName must appear after the restart." 'DarkGray'
Say "Colour: ':root { --giz-palette: green; }' in the Manager's Skin profile custom CSS (it survives reinstalls)." 'DarkGray'
$minClient = Get-Content (Join-Path $Src 'grafit.version.txt') -ErrorAction SilentlyContinue | Where-Object { $_ -like 'Client *' }
if ($minClient) { Say "Clients: $($minClient -replace '^Client\s+', '') - an older Gizmo Client will not start the shell." 'DarkGray' }
Say "After every Gizmo Server update, run install.bat again: _framework and static\ are copies of the stock skin's." 'Yellow'
Say "Backups: $BackupRt" 'DarkGray'
Say "Back to the previous version:  install.bat rollback" 'DarkGray'
Say "Remove Grafit completely:      install.bat uninstall" 'DarkGray'
