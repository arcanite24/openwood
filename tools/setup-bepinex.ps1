# OpenWood Setup Script
# This script downloads and installs BepInEx for Littlewood

param(
    [string]$GamePath = "..\Littlewood",
    [switch]$Force
)

$ErrorActionPreference = "Stop"

# BepInEx version for Unity 2019.4 (Mono)
$BepInExVersion = "5.4.23.2"
$BepInExUrl = "https://github.com/BepInEx/BepInEx/releases/download/v$BepInExVersion/BepInEx_win_x64_$BepInExVersion.zip"
$TempZip = "$env:TEMP\BepInEx.zip"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  OpenWood - BepInEx Setup" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Resolve game path
$GamePath = Resolve-Path $GamePath -ErrorAction SilentlyContinue
if (-not $GamePath) {
    $GamePath = Join-Path $PSScriptRoot "..\Littlewood"
}

Write-Host "Game path: $GamePath"

# Check if game exists
$GameExe = Join-Path $GamePath "Littlewood.exe"
if (-not (Test-Path $GameExe)) {
    Write-Host "ERROR: Littlewood.exe not found at $GamePath" -ForegroundColor Red
    Write-Host "Please ensure the game files are in the Littlewood folder." -ForegroundColor Yellow
    exit 1
}

# Check if BepInEx already installed
$BepInExFolder = Join-Path $GamePath "BepInEx"
if ((Test-Path $BepInExFolder) -and -not $Force) {
    Write-Host "BepInEx is already installed." -ForegroundColor Yellow
    Write-Host "Use -Force to reinstall." -ForegroundColor Yellow
    exit 0
}

Write-Host ""
Write-Host "Downloading BepInEx v$BepInExVersion..." -ForegroundColor Green

try {
    [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
    Invoke-WebRequest -Uri $BepInExUrl -OutFile $TempZip -UseBasicParsing
} catch {
    Write-Host "ERROR: Failed to download BepInEx" -ForegroundColor Red
    Write-Host $_.Exception.Message
    exit 1
}

Write-Host "Extracting BepInEx to game folder..." -ForegroundColor Green

try {
    Expand-Archive -Path $TempZip -DestinationPath $GamePath -Force
    Remove-Item $TempZip -Force
} catch {
    Write-Host "ERROR: Failed to extract BepInEx" -ForegroundColor Red
    Write-Host $_.Exception.Message
    exit 1
}

# Create plugins folder
$PluginsFolder = Join-Path $BepInExFolder "plugins"
if (-not (Test-Path $PluginsFolder)) {
    New-Item -ItemType Directory -Path $PluginsFolder | Out-Null
}

# Create OpenWood plugins subfolder
$OpenWoodFolder = Join-Path $PluginsFolder "OpenWood"
if (-not (Test-Path $OpenWoodFolder)) {
    New-Item -ItemType Directory -Path $OpenWoodFolder | Out-Null
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "  BepInEx installed successfully!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "1. Run Littlewood.exe once to generate BepInEx config"
Write-Host "2. Build the OpenWood solution: dotnet build"
Write-Host "3. The plugin will be automatically copied to BepInEx/plugins"
Write-Host ""
Write-Host "BepInEx folder: $BepInExFolder"
Write-Host "Plugins folder: $PluginsFolder"
