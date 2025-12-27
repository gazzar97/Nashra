#!/bin/bash
# Database Migration Script for Railway
# This script can be used as an alternative to runtime migrations

set -e  # Exit on error

echo "🚀 Starting database migration process..."

# Check if connection string is set
if [ -z "$ConnectionStrings__DefaultConnection" ]; then
    echo "❌ Error: ConnectionStrings__DefaultConnection environment variable is not set"
    exit 1
fi

echo "📦 Installing EF Core tools..."
dotnet tool install --global dotnet-ef --version 9.0.1 || true

echo "🔄 Applying migrations for all modules..."

# Competitions Module
echo "📊 Migrating Competitions module..."
dotnet ef database update --project src/SportsData.Bootstrapper --context CompetitionsDbContext --no-build

# Matches Module
echo "⚽ Migrating Matches module..."
dotnet ef database update --project src/SportsData.Bootstrapper --context MatchesDbContext --no-build

# ApiKeys Module
echo "🔑 Migrating ApiKeys module..."
dotnet ef database update --project src/SportsData.Bootstrapper --context ApiKeysDbContext --no-build

echo "✅ All migrations completed successfully!"
