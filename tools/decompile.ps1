# OpenWood Decompilation Script
# This script uses ILSpy to decompile the game's DLLs for reference

param(
    [string]$GamePath = "..\Littlewood",
    [string]$OutputPath = "..\decompiled"
)

$ErrorActionPreference = "Stop"

# ILSpyCmd version
$ILSpyVersion = "8.2.0.7535"
$ToolPath = Join-Path $env:USERPROFILE ".dotnet\tools"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  OpenWood - Game Decompilation" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Resolve paths
$ScriptDir = $PSScriptRoot
$GamePath = Join-Path $ScriptDir $GamePath | Resolve-Path -ErrorAction SilentlyContinue
if (-not $GamePath) {
    $GamePath = Join-Path $ScriptDir "..\Littlewood"
}

$OutputPath = Join-Path $ScriptDir $OutputPath
if (-not (Test-Path $OutputPath)) {
    New-Item -ItemType Directory -Path $OutputPath | Out-Null
}

$ManagedPath = Join-Path $GamePath "Littlewood_Data\Managed"

Write-Host "Game path: $GamePath"
Write-Host "Output path: $OutputPath"
Write-Host ""

# Check if ILSpyCmd is installed
$ILSpyCmd = Get-Command ilspycmd -ErrorAction SilentlyContinue
if (-not $ILSpyCmd) {
    Write-Host "ILSpyCmd not found. Installing..." -ForegroundColor Yellow
    dotnet tool install ilspycmd -g
    $ILSpyCmd = Get-Command ilspycmd -ErrorAction SilentlyContinue
    
    if (-not $ILSpyCmd) {
        Write-Host "ERROR: Failed to install ILSpyCmd" -ForegroundColor Red
        Write-Host "Try running: dotnet tool install ilspycmd -g" -ForegroundColor Yellow
        exit 1
    }
}

Write-Host "Using ILSpyCmd: $($ILSpyCmd.Source)" -ForegroundColor Green
Write-Host ""

# DLLs to decompile
$DLLs = @(
    "Assembly-CSharp.dll",
    "Assembly-CSharp-firstpass.dll"
)

foreach ($dll in $DLLs) {
    $DllPath = Join-Path $ManagedPath $dll
    
    if (-not (Test-Path $DllPath)) {
        Write-Host "WARNING: $dll not found, skipping..." -ForegroundColor Yellow
        continue
    }
    
    $DllName = [System.IO.Path]::GetFileNameWithoutExtension($dll)
    $DllOutputPath = Join-Path $OutputPath $DllName
    
    Write-Host "Decompiling $dll..." -ForegroundColor Green
    
    try {
        # Create output directory
        if (-not (Test-Path $DllOutputPath)) {
            New-Item -ItemType Directory -Path $DllOutputPath | Out-Null
        }
        
        # Run ILSpy
        & ilspycmd $DllPath -p -o $DllOutputPath
        
        Write-Host "  -> Output: $DllOutputPath" -ForegroundColor Gray
    } catch {
        Write-Host "ERROR: Failed to decompile $dll" -ForegroundColor Red
        Write-Host $_.Exception.Message
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "  Decompilation complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Decompiled source is in: $OutputPath" -ForegroundColor Yellow
Write-Host ""
Write-Host "IMPORTANT: This code is for reference only!" -ForegroundColor Red
Write-Host "Do not include decompiled code in your mods."
Write-Host "Use it to understand game mechanics and find patch targets."
