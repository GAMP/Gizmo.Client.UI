@echo off
setlocal
cd /d "%~dp0"

if not exist config.json (
    echo config.json not found. Copy config.example.json to config.json and fill in
    echo an operator username/password ^(the same kind Gizmo Manager logs in with^).
    pause
    exit /b 1
)

where python >nul 2>nul
if errorlevel 1 (
    echo Python was not found on PATH. Install Python 3.8+ from python.org
    echo ^(check "Add python.exe to PATH" during install^), or build avatar_proxy.exe
    echo with PyInstaller - see README.md.
    pause
    exit /b 1
)

echo Starting Grafit avatar proxy...
python avatar_proxy.py
pause
