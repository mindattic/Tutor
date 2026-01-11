@echo off
setlocal

set "PS_SCRIPT=%~dp0Export.ps1"

if not exist "%PS_SCRIPT%" (
  echo Error: Export.ps1 not found next to zzz_Export.bat
  exit /b 1
)

powershell -NoProfile -ExecutionPolicy Bypass -File "%PS_SCRIPT%"
if errorlevel 1 (
  echo Export failed. See errors above.
  exit /b 1
)

echo Export complete: ExportedScripts.txt
endlocal
