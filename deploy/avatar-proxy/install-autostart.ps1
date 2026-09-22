<#
.SYNOPSIS
    Registers the Grafit avatar proxy as a Scheduled Task that starts at
    system boot, restarts itself on failure, and runs invisibly (no console
    window) - so it behaves like a lightweight background service without
    needing a real Windows Service install.

.PARAMETER Uninstall
    Removes the scheduled task instead of installing it.

.EXAMPLE
    Right-click PowerShell -> Run as Administrator, then:
    .\install-autostart.ps1

.EXAMPLE
    .\install-autostart.ps1 -Uninstall
#>
param(
    [switch]$Uninstall
)

$ErrorActionPreference = "Stop"
$TaskName = "Grafit Avatar Proxy"
$ScriptDir = $PSScriptRoot

if (-not ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
    Write-Host "This needs to run as Administrator (registers a Scheduled Task)." -ForegroundColor Yellow
    exit 1
}

if ($Uninstall) {
    Unregister-ScheduledTask -TaskName $TaskName -Confirm:$false -ErrorAction SilentlyContinue
    Write-Host "Removed scheduled task '$TaskName'."
    exit 0
}

if (-not (Test-Path (Join-Path $ScriptDir "config.json"))) {
    Write-Host "config.json not found next to this script." -ForegroundColor Yellow
    Write-Host "Copy config.example.json to config.json and fill in an operator username/password first."
    exit 1
}

$ExePath = Join-Path $ScriptDir "avatar_proxy.exe"
if (Test-Path $ExePath) {
    # Prefer the PyInstaller-built standalone exe if present - no Python
    # install required on the server at all.
    $Action = New-ScheduledTaskAction -Execute $ExePath -WorkingDirectory $ScriptDir
} else {
    $PythonW = (Get-Command pythonw.exe -ErrorAction SilentlyContinue).Source
    if (-not $PythonW) {
        $PythonW = (Get-Command python.exe -ErrorAction SilentlyContinue).Source
    }
    if (-not $PythonW) {
        Write-Host "Neither avatar_proxy.exe nor a Python install were found." -ForegroundColor Yellow
        Write-Host "Install Python 3.8+ (python.org, check 'Add to PATH'), or build the exe - see README.md."
        exit 1
    }
    $ScriptPath = Join-Path $ScriptDir "avatar_proxy.py"
    $Action = New-ScheduledTaskAction -Execute $PythonW -Argument "`"$ScriptPath`"" -WorkingDirectory $ScriptDir
}

# A short delay so GizmoService.exe (and the SQL/web server it depends on)
# is already up before the proxy starts trying to authenticate against it.
$Trigger = New-ScheduledTaskTrigger -AtStartup
$Trigger.Delay = "PT30S"

$Settings = New-ScheduledTaskSettingsSet `
    -AllowStartIfOnBatteries -DontStopIfGoingOnBatteries `
    -StartWhenAvailable -RestartCount 999 -RestartInterval (New-TimeSpan -Minutes 1) `
    -ExecutionTimeLimit (New-TimeSpan -Days 0)

$Principal = New-ScheduledTaskPrincipal -UserId "SYSTEM" -LogonType ServiceAccount -RunLevel Highest

Register-ScheduledTask -TaskName $TaskName -Action $Action -Trigger $Trigger `
    -Settings $Settings -Principal $Principal -Force | Out-Null

Write-Host "Installed scheduled task '$TaskName' (starts at boot, restarts on crash)." -ForegroundColor Green
Write-Host "Starting it now..."
Start-ScheduledTask -TaskName $TaskName
Start-Sleep -Seconds 2
Write-Host "Check avatar_proxy.log in this folder, or open http://localhost:8765/health"
