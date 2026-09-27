@echo off
setlocal

rem ============================================================
rem  GRAFIT skin for Gizmo V3 - installer
rem
rem    install.bat              - install / update the "Grafit" skin
rem    install.bat rollback     - back to the previously installed version
rem    install.bat uninstall    - back to what was there before Grafit
rem
rem  Requests admin rights automatically (Program Files).
rem  The skin is a folder of its own in the server's skins directory,
rem  next to the stock "Next". Nothing of the stock skin is touched:
rem  a host group is switched to "Grafit" in the Manager.
rem ============================================================

set "MODE=%~1"
set "SELF=%~f0"
set "SELF=%SELF:'=''%"

net session >nul 2>&1
if not "%errorlevel%"=="0" (
    echo Requesting administrator rights...
    if defined MODE (
        powershell -NoProfile -Command "Start-Process -FilePath '%SELF%' -ArgumentList '%MODE%' -Verb RunAs"
    ) else (
        powershell -NoProfile -Command "Start-Process -FilePath '%SELF%' -Verb RunAs"
    )
    exit /b
)

cd /d "%~dp0"

if /i "%MODE%"=="uninstall" (
    powershell -NoProfile -ExecutionPolicy Bypass -File ".\_install-core.ps1" -Uninstall
) else if /i "%MODE%"=="rollback" (
    powershell -NoProfile -ExecutionPolicy Bypass -File ".\_install-core.ps1" -Rollback
) else (
    powershell -NoProfile -ExecutionPolicy Bypass -File ".\_install-core.ps1"
)

echo.
pause
