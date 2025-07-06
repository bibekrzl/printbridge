# PrintBridge Tray App - Build and Run Script (PowerShell)
# ========================================================

Write-Host "PrintBridge Tray App - Build and Run Script" -ForegroundColor Green
Write-Host "===========================================" -ForegroundColor Green
Write-Host ""

# Check for .NET 9 SDK
Write-Host "Checking for .NET 9 SDK..." -ForegroundColor Yellow
try {
    $dotnetVersion = dotnet --version 2>$null
    if ($LASTEXITCODE -eq 0) {
        if ($dotnetVersion -like "9.*") {
            Write-Host "✓ .NET 9 SDK found: $dotnetVersion" -ForegroundColor Green
        } else {
            Write-Host "WARNING: .NET SDK version $dotnetVersion found, but .NET 9 is recommended." -ForegroundColor Yellow
        }
    } else {
        throw "Command failed"
    }
} catch {
    Write-Host "ERROR: .NET 9 SDK not found!" -ForegroundColor Red
    Write-Host "Please install .NET 9 SDK from: https://dotnet.microsoft.com/download/dotnet/9.0" -ForegroundColor Yellow
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Host ""

# Restore packages
Write-Host "Restoring packages..." -ForegroundColor Yellow
dotnet restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Failed to restore packages" -ForegroundColor Red
    Read-Host "Press Enter to exit"
    exit 1
}
Write-Host "✓ Packages restored" -ForegroundColor Green

Write-Host ""

# Build project
Write-Host "Building project..." -ForegroundColor Yellow
dotnet build --configuration Release
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Build failed" -ForegroundColor Red
    Read-Host "Press Enter to exit"
    exit 1
}
Write-Host "✓ Build successful" -ForegroundColor Green

Write-Host ""

# Start application
Write-Host "Starting PrintBridge Tray App..." -ForegroundColor Yellow
Write-Host "The application will start in the system tray." -ForegroundColor Cyan
Write-Host "Web dashboard will be available at: http://localhost:8080" -ForegroundColor Cyan
Write-Host ""
Write-Host "Press Ctrl+C to stop the application" -ForegroundColor Yellow
Write-Host ""

dotnet run --configuration Release

Read-Host "Press Enter to exit" 