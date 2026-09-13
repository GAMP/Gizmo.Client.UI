@echo off
setlocal

rem ============================================================
rem  GRAFIT skin for Gizmo V3 - installer
rem
rem    install.bat              - install / update the "Grafit" skin
rem    install.bat uninstall    - put back what was there before
rem
rem  Requests admin rights automatically (Program Files).
rem  The skin is a folder of its own in the server's skins directory,
rem  next to the stock "Next". Nothing of the stock skin is touched:
rem  a host group is switched to "Grafit" in the Manager.
rem ============================================================

set "MODE=%~1"

net session >nul 2>&1
if not "%errorlevel%"=="0" (
    echo Requesting administrator rights...
    powershell -NoProfile -Command "Start-Process -FilePath '%~f0' -ArgumentList '%MODE%' -Verb RunAs"
    exit /b
)

cd /d "%~dp0"

if /i "%MODE%"=="uninstall" (
    powershell -NoProfile -ExecutionPolicy Bypass -File ".\_install-core.ps1" -Uninstall
) else (
    powershell -NoProfile -ExecutionPolicy Bypass -File ".\_install-core.ps1"
)

echo.
pause
