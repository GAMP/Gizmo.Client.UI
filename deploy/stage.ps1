param(
    # The Gizmo.Client.UI project folder; by default the one next to this deploy folder.
    [string]$Project = (Join-Path (Split-Path $PSScriptRoot -Parent) 'Gizmo.Client.UI'),
    # Where skin\ and dist\ are written; by default next to this script.
    [string]$OutRoot = $PSScriptRoot,
    # Build first (dotnet build -c Release, which runs the webpack bundle as well).
    [switch]$Build
)

# Stages the skin folder from the project's Release build and packs it as
# dist\grafit-shell-<grafit>-gizmo-<gizmo>.zip together with the installer. The two
# version numbers come from the project file - the only place they are written - and
# are also left in the skin as grafit.version.txt, where a person can read them without
# opening the DLL's properties (the shell shows its own version only on the Profile tab).

$ErrorActionPreference = 'Stop'

function Say($t, $c = 'Gray') { Write-Host $t -ForegroundColor $c }

$csproj = Join-Path $Project 'Gizmo.Client.UI.csproj'
if (-not (Test-Path $csproj)) { Say "Not a project folder: $Project" 'Red'; exit 1 }

[xml]$proj = Get-Content $csproj
$grafit = ($proj.Project.PropertyGroup | ForEach-Object { $_.GrafitVersion } | Where-Object { $_ } | Select-Object -First 1)
$gizmo  = ($proj.Project.PropertyGroup | ForEach-Object { $_.GizmoVersion }  | Where-Object { $_ } | Select-Object -First 1)
if (-not $grafit -or -not $gizmo) { Say "GrafitVersion / GizmoVersion not found in the project file." 'Red'; exit 1 }

if ($Build) {
    Say "Building $csproj (Release)..." 'DarkGray'
    & dotnet build $csproj -c Release --nologo -v q
    if ($LASTEXITCODE -ne 0) { Say "Build failed." 'Red'; exit 1 }
}

$bin     = Join-Path $Project 'bin\Release\net10.0'
$wwwroot = Join-Path $Project 'wwwroot'
$skin    = Join-Path $OutRoot 'skin'
$dist    = Join-Path $OutRoot 'dist'

foreach ($must in @((Join-Path $bin 'Gizmo.Client.UI.dll'), (Join-Path $bin 'Gizmo.Web.Components.dll'),
                    (Join-Path $wwwroot 'index.html'), (Join-Path $wwwroot 'client_internal_style.js'))) {
    if (-not (Test-Path $must)) { Say "Missing build output: $must (build first: stage.ps1 -Build)" 'Red'; exit 1 }
}

# The DLL must carry the same version the project file says, or the build is stale.
$built = (Get-Item (Join-Path $bin 'Gizmo.Client.UI.dll')).VersionInfo.ProductVersion
if ($built -ne "$grafit (Gizmo $gizmo)") {
    Say "The built DLL says '$built' but the project file says '$grafit (Gizmo $gizmo)' - rebuild first." 'Red'
    exit 1
}

# ── stage ───────────────────────────────────────────────────────────────────
$content = Join-Path $skin 'wwwroot\_content\Gizmo.Client.UI'
New-Item -ItemType Directory -Path $skin -Force | Out-Null
Copy-Item (Join-Path $bin 'Gizmo.Client.UI.dll')       (Join-Path $skin 'Gizmo.Client.UI.dll')       -Force
Copy-Item (Join-Path $bin 'Gizmo.Web.Components.dll')  (Join-Path $skin 'Gizmo.Web.Components.dll')  -Force
New-Item -ItemType Directory -Path (Join-Path $skin 'wwwroot') -Force | Out-Null
Copy-Item (Join-Path $wwwroot 'index.html') (Join-Path $skin 'wwwroot\index.html') -Force

if (Test-Path $content) { Remove-Item $content -Recurse -Force }
New-Item -ItemType Directory -Path $content -Force | Out-Null
Get-ChildItem $wwwroot -Force | Where-Object { $_.Name -ne 'index.html' } |
    ForEach-Object { Copy-Item $_.FullName (Join-Path $content $_.Name) -Recurse -Force }

# composition.json is the skin's own (same content as the stock skin's); keep it if
# staged before, write it otherwise.
$composition = Join-Path $skin 'composition.json'
if (-not (Test-Path $composition)) {
    Set-Content $composition @'
{
  "UIComposition": {
    "AppAssembly": "Gizmo.Client.UI.dll",
    "AdditionalAssemblies": [ "Gizmo.Web.Components.dll" ],
    "RootComponentType": "Gizmo.Client.UI.App,Gizmo.Client.UI",
    "NotificationsComponentType": "Gizmo.Client.UI.Components.NotificationsHost,Gizmo.Client.UI"
  }
}
'@
}

Set-Content (Join-Path $skin 'grafit.version.txt') @(
    "Grafit $grafit",
    "Gizmo  $gizmo",
    "Built  $(Get-Date -Format 'yyyy-MM-dd HH:mm')"
)

# ── pack ────────────────────────────────────────────────────────────────────
New-Item -ItemType Directory -Path $dist -Force | Out-Null
$zip = Join-Path $dist "grafit-shell-$grafit-gizmo-$gizmo.zip"
if (Test-Path $zip) { Remove-Item $zip -Force }
# The installer and its notes travel with the skin; they live next to this script.
$parts = @('README.md', 'install.bat', '_install-core.ps1' | ForEach-Object { Join-Path $PSScriptRoot $_ }) + $skin
Compress-Archive -Path $parts -DestinationPath $zip -CompressionLevel Optimal

Say "Staged skin\ and packed $zip" 'Green'
Say "Grafit $grafit for Gizmo $gizmo" 'DarkGray'
