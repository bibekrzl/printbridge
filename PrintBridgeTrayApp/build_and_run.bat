@echo off
echo PrintBridge Tray App - Build and Run Script
echo ===========================================

echo.
echo Checking for .NET 9 SDK...
dotnet --version >tmpver.txt 2>&1
set /p DOTNETVER=<tmpver.txt
if not "%DOTNETVER:~0,1%"=="9" (
    echo WARNING: .NET SDK version %DOTNETVER% found, but .NET 9 is recommended.
)
del tmpver.txt
if "%DOTNETVER%"=="dotnet: command not found" (
    echo ERROR: .NET 9 SDK not found!
    echo Please install .NET 9 SDK from: https://dotnet.microsoft.com/download/dotnet/9.0
    pause
    exit /b 1
)

echo ✓ .NET SDK found: %DOTNETVER%
echo.

echo Restoring packages...
dotnet restore
if %errorlevel% neq 0 (
    echo ERROR: Failed to restore packages
    pause
    exit /b 1
)

echo ✓ Packages restored
echo.

echo Building project...
dotnet build --configuration Release
if %errorlevel% neq 0 (
    echo ERROR: Build failed
    pause
    exit /b 1
)

echo ✓ Build successful
echo.

echo Starting PrintBridge Tray App...
echo The application will start in the system tray.
echo Web dashboard will be available at: http://localhost:8080
echo.
echo Press Ctrl+C to stop the application
echo.

dotnet run --configuration Release

pause 