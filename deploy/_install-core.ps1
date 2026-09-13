param(
    [switch]$Uninstall,
    # Where Gizmo Server is installed. The skin goes into its skins folder.
    [string]$ServerRoot = 'C:\Program Files\NETProjects\Gizmo Server',
    # The skin's folder name = the name the Manager shows for it.
    [string]$SkinName = 'grafit'
)

$ErrorActionPreference = 'Stop'

$Skins     = Join-Path $ServerRoot 'skins'
$Target    = Join-Path $Skins $SkinName
$Reference = Join-Path $Skins 'Next'
$Src       = Join-Path $PSScriptRoot 'skin'
$BackupRt  = Join-Path $PSScriptRoot 'backup'

# Grafit is a skin of its own: a folder next to the stock "Next" with the same layout
# (composition.json, the two assemblies, wwwroot). Nothing of the stock skin is touched
# - a host group is switched to it in the Manager, and switched back to leave it.
#
# The only thing taken from the stock skin is wwwroot\_framework: it belongs to the
# server version, so the server's own copy beats the one in the package.

function Say($t, $c = 'Gray') { Write-Host $t -ForegroundColor $c }

if (-not (Test-Path $Skins)) {
    Say "Not found: $Skins" 'Red'
    Say "Check the Gizmo Server install path (-ServerRoot)." 'Yellow'
    exit 1
}

# ── rollback ────────────────────────────────────────────────────────────────
if ($Uninstall) {
    $last = Get-ChildItem $BackupRt -Directory -ErrorAction SilentlyContinue |
            Sort-Object Name -Descending | Select-Object -First 1
    if (-not $last) {
        Say "No backup found in $BackupRt - nothing to restore." 'Red'
        exit 1
    }

    Say "Restoring from $($last.FullName)" 'DarkGray'

    if (Test-Path $Target) { Remove-Item $Target -Recurse -Force }

    $saved = Join-Path $last.FullName $SkinName
    if (Test-Path $saved) {
        New-Item -ItemType Directory -Path $Target -Force | Out-Null
        Copy-Item (Join-Path $saved '*') $Target -Recurse -Force
        Say "Previous '$SkinName' skin put back." 'Green'
    } else {
        Say "There was no '$SkinName' skin before - the folder is removed." 'Green'
        Say "Host groups still pointing at '$SkinName' must be switched to another skin in the Manager." 'Yellow'
    }

    Say "Restart the client shell on the machines." 'Yellow'
    exit 0
}

if (-not (Test-Path $Src)) {
    Say "Missing 'skin' folder next to the installer." 'Red'
    exit 1
}

# ── backup ──────────────────────────────────────────────────────────────────
$stamp  = Get-Date -Format 'yyyyMMdd-HHmmss'
$backup = Join-Path $BackupRt $stamp
New-Item -ItemType Directory -Path $backup -Force | Out-Null

if (Test-Path $Target) {
    $saved = Join-Path $backup $SkinName
    New-Item -ItemType Directory -Path $saved -Force | Out-Null
    Copy-Item (Join-Path $Target '*') $saved -Recurse -Force
    Say "Backup: $saved" 'DarkGray'
} else {
    # A marker so a rollback knows to remove the folder rather than restore it.
    Set-Content (Join-Path $backup 'absent.txt') "No '$SkinName' skin existed before $stamp."
    Say "No previous '$SkinName' skin - first install." 'DarkGray'
}

# ── install ─────────────────────────────────────────────────────────────────
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

if (Test-Path $Target) { Remove-Item $Target -Recurse -Force }
New-Item -ItemType Directory -Path $Target -Force | Out-Null
Copy-Item (Join-Path $Src '*') $Target -Recurse -Force
Say "Installed skin: $Target" 'DarkGray'

$refFramework = Join-Path $Reference 'wwwroot\_framework'
if (Test-Path $refFramework) {
    $dstFramework = Join-Path $Target 'wwwroot\_framework'
    Remove-Item $dstFramework -Recurse -Force -ErrorAction SilentlyContinue
    New-Item -ItemType Directory -Path $dstFramework -Force | Out-Null
    Copy-Item (Join-Path $refFramework '*') $dstFramework -Recurse -Force
    Say "Took wwwroot\_framework from the server's own Next skin" 'DarkGray'
} elseif (-not (Test-Path (Join-Path $Target 'wwwroot\_framework\blazor.webview.js'))) {
    Say "No _framework found in the stock skin or the package - the shell will not start!" 'Red'
    exit 1
}

# ── sanity ──────────────────────────────────────────────────────────────────
foreach ($must in @('composition.json', 'Gizmo.Client.UI.dll', 'Gizmo.Web.Components.dll',
                    'wwwroot\index.html', 'wwwroot\_framework\blazor.webview.js',
                    'wwwroot\_content\Gizmo.Client.UI\vendor\fonts\fonts.css')) {
    if (-not (Test-Path (Join-Path $Target $must))) {
        Say "Missing after install: $must" 'Red'
        exit 1
    }
}

Say ""
Say "Done. Skin '$SkinName' ($shellVersion) is installed next to the stock one." 'Green'
Say "Next: Manager -> the PC's host group -> Skin = '$SkinName' (a group's own Skin beats the server default)," 'Yellow'
Say "      then restart the Gizmo Client on a PC of that group (the whole client, not a sign-out):" 'Yellow'
Say "      the client receives its skin name and the skin files only when it connects." 'Yellow'
Say "Check on the PC: %PROGRAMDATA%\NETProjects\Gizmo Client\Skins\$SkinName must appear after the restart." 'DarkGray'
Say "Colour: data-accent in $Target\wwwroot\index.html, or ':root { --gg-palette: green; }' in the Manager's custom CSS." 'DarkGray'
Say "Rollback:  install.bat uninstall" 'DarkGray'
