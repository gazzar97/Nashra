using Microsoft.EntityFrameworkCore;
using SportsData.Modules.Competitions.Infrastructure;
using SportsData.Modules.Matches.Infrastructure;
using SportsData.Modules.ApiKeys.Infrastructure;

namespace SportsData.Bootstrapper.Extensions;

public static class DatabaseExtensions
{
    /// <summary>
    /// Applies all pending migrations for all modules on application startup
    /// </summary>
    public static async Task ApplyMigrationsAsync(this IServiceProvider serviceProvider, ILogger logger)
    {
        try
        {
            logger.LogInformation("Starting database migration process...");

            using var scope = serviceProvider.CreateScope();

            // Migrate Competitions module
            await MigrateDbContextAsync<CompetitionsDbContext>(scope, logger, "Competitions");

            // Migrate Matches module
            await MigrateDbContextAsync<MatchesDbContext>(scope, logger, "Matches");

            // Migrate ApiKeys module
            await MigrateDbContextAsync<ApiKeysDbContext>(scope, logger, "ApiKeys");

            logger.LogInformation("All database migrations completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the database");
            throw;
        }
    }

    private static async Task MigrateDbContextAsync<TContext>(
        IServiceScope scope,
        ILogger logger,
        string moduleName) where TContext : DbContext
    {
        try
        {
            var context = scope.ServiceProvider.GetRequiredService<TContext>();
            
            // Check if any migrations exist
            var allMigrations = context.Database.GetMigrations();
            
            if (!allMigrations.Any())
            {
                // No migrations exist, use EnsureCreated to create the database schema
                logger.LogInformation("No migrations found for {Module} module. Creating database schema...", moduleName);
                await context.Database.EnsureCreatedAsync();
                logger.LogInformation("Successfully created database schema for {Module} module", moduleName);
            }
            else
            {
                // Migrations exist, apply them
                var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
                
                if (pendingMigrations.Any())
                {
                    logger.LogInformation("Applying {Count} pending migrations for {Module} module", 
                        pendingMigrations.Count(), moduleName);
                    
                    await context.Database.MigrateAsync();
                    
                    logger.LogInformation("Successfully migrated {Module} module", moduleName);
                }
                else
                {
                    logger.LogInformation("No pending migrations for {Module} module", moduleName);
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to migrate {Module} module", moduleName);
            throw;
        }
    }
}
