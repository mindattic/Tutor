@echo off
setlocal enabledelayedexpansion

:: Define source and destination folders
set "SOURCE=D:\Projects\Tutor"
set "BACKUP_BASE=R:\Backup\SnowCrash"

:: Get current date in YYYY-MM-DD format
for /f "tokens=2-4 delims=/ " %%a in ('echo %date%') do (
    set "YEAR=%date:~-4,4%"
    set "MONTH=%%a"
    set "DAY=%%b"
)
set "BASE_FOLDER=%BACKUP_BASE%\%YEAR%-%MONTH%-%DAY%"

:: Check if the base folder already exists, and append _a, _b, _c, etc.
set "BACKUP_FOLDER=%BASE_FOLDER%"
set "SUFFIX=a"

:CHECK_FOLDER
if exist "%BACKUP_FOLDER%" (
    set "BACKUP_FOLDER=%BASE_FOLDER%_%SUFFIX%"
    for /f "delims=" %%x in ('cmd /c "echo !SUFFIX! | findstr /r [a-z]"') do set /a "SUFFIX=1!SUFFIX!+1"
    set "SUFFIX=!SUFFIX:~1!"
    goto CHECK_FOLDER
)

:: Confirmation prompt
echo.
echo You are about to back up:
echo   From: %SOURCE%
echo   To:   %BACKUP_FOLDER%\Tutor
echo.
set /p "CHOICE=Proceed with backup? (y/n): "
if /i not "%CHOICE%"=="Y" (
    echo Backup aborted by user.
    pause
    exit /b
)

:: Create the new dated folder
mkdir "%BACKUP_FOLDER%"

:: Copy the project folder
xcopy "%SOURCE%" "%BACKUP_FOLDER%\Tutor\" /E /I /Y

