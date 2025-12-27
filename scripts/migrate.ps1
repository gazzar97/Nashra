# Database Migration Script for Railway (PowerShell)
# This script can be used as an alternative to runtime migrations

$ErrorActionPreference = "Stop"

Write-Host "🚀 Starting database migration process..." -ForegroundColor Green

# Check if connection string is set
if (-not $env:ConnectionStrings__DefaultConnection) {
    Write-Host "❌ Error: ConnectionStrings__DefaultConnection environment variable is not set" -ForegroundColor Red
    exit 1
}

Write-Host "📦 Installing EF Core tools..." -ForegroundColor Cyan
dotnet tool install --global dotnet-ef --version 9.0.1 2>$null

Write-Host "🔄 Applying migrations for all modules..." -ForegroundColor Cyan

# Competitions Module
Write-Host "📊 Migrating Competitions module..." -ForegroundColor Yellow
dotnet ef database update --project src/SportsData.Bootstrapper --context CompetitionsDbContext --no-build

# Matches Module
Write-Host "⚽ Migrating Matches module..." -ForegroundColor Yellow
dotnet ef database update --project src/SportsData.Bootstrapper --context MatchesDbContext --no-build

# ApiKeys Module
Write-Host "🔑 Migrating ApiKeys module..." -ForegroundColor Yellow
dotnet ef database update --project src/SportsData.Bootstrapper --context ApiKeysDbContext --no-build

Write-Host "✅ All migrations completed successfully!" -ForegroundColor Green
