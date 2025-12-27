using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SportsData.Modules.Matches.Infrastructure;

namespace SportsData.Modules.Matches
{
    public static class MatchesModuleExtensions
    {
        public static IServiceCollection AddMatchesModule(this IServiceCollection services, IConfiguration configuration)
        {
            // Build connection string from Railway environment variables
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            
            // If connection string is empty, build it from environment variables (Railway deployment)
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                var host = Environment.GetEnvironmentVariable("MYSQLHOST");
                var port = Environment.GetEnvironmentVariable("MYSQLPORT");
                var database = Environment.GetEnvironmentVariable("MYSQLDATABASE");
                var user = Environment.GetEnvironmentVariable("MYSQLUSER");
                var password = Environment.GetEnvironmentVariable("MYSQL_ROOT_PASSWORD") ?? Environment.GetEnvironmentVariable("MYSQLPASSWORD");
                
                if (!string.IsNullOrWhiteSpace(host) && !string.IsNullOrWhiteSpace(database))
                {
                    connectionString = $"Server={host};Port={port ?? "3306"};Database={database};User={user ?? "root"};Password={password};";
                }
            }
            
            var databaseProvider = configuration.GetValue<string>("DatabaseProvider") ?? "SqlServer";

            services.AddDbContext<MatchesDbContext>(options =>
            {
                if (databaseProvider.Equals("MySQL", StringComparison.OrdinalIgnoreCase))
                {
                    var serverVersion = new MySqlServerVersion(new Version(8, 0, 21));
                    options.UseMySql(connectionString, serverVersion, mySqlOptions =>
                        mySqlOptions.MigrationsAssembly("SportsData.Modules.Matches"));
                }
                else
                {
                    options.UseSqlServer(connectionString, sqlOptions =>
                        sqlOptions.MigrationsAssembly("SportsData.Modules.Matches"));
                }
                
                // Suppress pending model changes warning
                options.ConfigureWarnings(warnings => 
                    warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
            });
            // Register MediatR Handlers from this assembly
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(MatchesModuleExtensions).Assembly));

            // Register Validators
            services.AddValidatorsFromAssembly(typeof(MatchesModuleExtensions).Assembly);
            return services;
        }
    }
}
