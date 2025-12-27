# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution file
COPY SportsData.sln ./

# Copy all project files
COPY src/SportsData.Bootstrapper/SportsData.Bootstrapper.csproj src/SportsData.Bootstrapper/
COPY src/Shared/SportsData.Shared/SportsData.Shared.csproj src/Shared/SportsData.Shared/
COPY src/Modules/Competitions/SportsData.Modules.Competitions.csproj src/Modules/Competitions/
COPY src/Modules/Matches/SportsData.Modules.Matches.csproj src/Modules/Matches/
COPY src/Modules/DataProcessing/SportsData.Modules.DataProcessing.csproj src/Modules/DataProcessing/
COPY src/Modules/ApiKeys/SportsData.Modules.ApiKeys.csproj src/Modules/ApiKeys/

# Restore dependencies
RUN dotnet restore

# Copy all source files
COPY . .

# Build the application
WORKDIR /src/src/SportsData.Bootstrapper
RUN dotnet build -c Release -o /app/build --no-restore

# Publish Stage
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish --no-restore /p:UseAppHost=false

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Install curl for healthchecks (optional but recommended)
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy published application
COPY --from=publish /app/publish .

# Create logs directory
RUN mkdir -p /app/logs && chmod 777 /app/logs

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Expose port (Railway will automatically detect this)
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=40s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Run the application
ENTRYPOINT ["dotnet", "SportsData.Bootstrapper.dll"]
