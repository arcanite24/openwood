# OpenWood Asset Extraction Script
# This script uses AssetRipper to extract game assets for modding API development

param(
    [string]$GamePath = "..\Littlewood",
    [string]$OutputPath = "..\extracted-assets",
    [switch]$Force
)

$ErrorActionPreference = "Stop"

# AssetRipper configuration
$AssetRipperVersion = "1.3.9"
$AssetRipperUrl = "https://github.com/AssetRipper/AssetRipper/releases/download/$AssetRipperVersion/AssetRipper_win_x64.zip"
$ToolsDir = Join-Path $PSScriptRoot ".assetripper"
$AssetRipperZip = Join-Path $ToolsDir "AssetRipper.zip"
$AssetRipperDir = Join-Path $ToolsDir "AssetRipper"
$AssetRipperExe = Join-Path $AssetRipperDir "AssetRipper.GUI.Free.exe"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  OpenWood - Asset Extraction" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Resolve paths
$ScriptDir = $PSScriptRoot
$GamePath = Join-Path $ScriptDir $GamePath | Resolve-Path -ErrorAction SilentlyContinue
if (-not $GamePath) {
    $GamePath = Join-Path $ScriptDir "..\Littlewood"
}

$OutputPath = Join-Path $ScriptDir $OutputPath

$GameDataPath = Join-Path $GamePath "Littlewood_Data"

Write-Host "Game path: $GamePath" -ForegroundColor White
Write-Host "Game data path: $GameDataPath" -ForegroundColor White
Write-Host "Output path: $OutputPath" -ForegroundColor White
Write-Host ""

# Verify game data exists
if (-not (Test-Path $GameDataPath)) {
    Write-Host "ERROR: Game data not found at $GameDataPath" -ForegroundColor Red
    Write-Host "Make sure Littlewood is installed in the correct location." -ForegroundColor Yellow
    exit 1
}

# Check if output already exists
if ((Test-Path $OutputPath) -and -not $Force) {
    Write-Host "Output directory already exists: $OutputPath" -ForegroundColor Yellow
    Write-Host "Use -Force to re-extract assets." -ForegroundColor Yellow
    $response = Read-Host "Do you want to continue and overwrite? (y/N)"
    if ($response -ne 'y' -and $response -ne 'Y') {
        Write-Host "Aborted." -ForegroundColor Red
        exit 0
    }
}

# Create tools directory
if (-not (Test-Path $ToolsDir)) {
    New-Item -ItemType Directory -Path $ToolsDir -Force | Out-Null
}

# Download AssetRipper if not present
if (-not (Test-Path $AssetRipperExe)) {
    Write-Host "Downloading AssetRipper v$AssetRipperVersion..." -ForegroundColor Yellow
    
    # Download the zip file
    try {
        [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
        $ProgressPreference = 'SilentlyContinue'  # Faster download
        Invoke-WebRequest -Uri $AssetRipperUrl -OutFile $AssetRipperZip -UseBasicParsing
        $ProgressPreference = 'Continue'
    }
    catch {
        Write-Host "ERROR: Failed to download AssetRipper" -ForegroundColor Red
        Write-Host $_.Exception.Message -ForegroundColor Red
        Write-Host ""
        Write-Host "You can manually download from:" -ForegroundColor Yellow
        Write-Host "  $AssetRipperUrl" -ForegroundColor White
        Write-Host "Extract to: $AssetRipperDir" -ForegroundColor White
        exit 1
    }
    
    Write-Host "Extracting AssetRipper..." -ForegroundColor Yellow
    
    # Extract the zip file
    if (Test-Path $AssetRipperDir) {
        Remove-Item -Path $AssetRipperDir -Recurse -Force
    }
    
    Expand-Archive -Path $AssetRipperZip -DestinationPath $AssetRipperDir -Force
    
    # Clean up zip file
    Remove-Item -Path $AssetRipperZip -Force
    
    Write-Host "AssetRipper installed successfully!" -ForegroundColor Green
}
else {
    Write-Host "AssetRipper already installed." -ForegroundColor Green
}

Write-Host ""

# Create output directory
if (Test-Path $OutputPath) {
    Write-Host "Cleaning existing output directory..." -ForegroundColor Yellow
    Remove-Item -Path $OutputPath -Recurse -Force
}
New-Item -ItemType Directory -Path $OutputPath -Force | Out-Null

# Run AssetRipper
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Extracting Assets..." -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "This may take a few minutes..." -ForegroundColor Yellow
Write-Host ""

# AssetRipper GUI mode - we'll open it and let user export
# For automated extraction, we can use the library API, but GUI is easier for now
Write-Host "Starting AssetRipper GUI..." -ForegroundColor Yellow
Write-Host ""
Write-Host "Instructions:" -ForegroundColor Cyan
Write-Host "  1. In AssetRipper, click 'File' -> 'Open Folder'" -ForegroundColor White
Write-Host "  2. Navigate to and select: $GameDataPath" -ForegroundColor White
Write-Host "  3. Wait for the assets to load" -ForegroundColor White
Write-Host "  4. Click 'Export' -> 'Export All Files'" -ForegroundColor White
Write-Host "  5. Select the output folder: $OutputPath" -ForegroundColor White
Write-Host "  6. Wait for export to complete" -ForegroundColor White
Write-Host ""

# Start AssetRipper
Start-Process -FilePath $AssetRipperExe

Write-Host "AssetRipper has been launched." -ForegroundColor Green
Write-Host ""
Write-Host "After exporting, the assets will be available in:" -ForegroundColor Cyan
Write-Host "  $OutputPath" -ForegroundColor White
Write-Host ""
Write-Host "Expected asset types:" -ForegroundColor Cyan
Write-Host "  - Textures (sprites, UI, tiles)" -ForegroundColor White
Write-Host "  - Audio (music, sound effects)" -ForegroundColor White
Write-Host "  - Prefabs (game objects)" -ForegroundColor White
Write-Host "  - Scenes (game levels)" -ForegroundColor White
Write-Host "  - ScriptableObjects (game data)" -ForegroundColor White
Write-Host "  - Shaders" -ForegroundColor White
Write-Host "  - Fonts" -ForegroundColor White
Write-Host "  - Animations" -ForegroundColor White
Write-Host ""
