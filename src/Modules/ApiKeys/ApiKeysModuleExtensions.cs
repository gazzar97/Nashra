using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using SportsData.Modules.ApiKeys.Infrastructure;
using FluentValidation;
using SportsData.Modules.ApiKeys.Infrastructure.Repositories;
using SportsData.Modules.ApiKeys.Application.ApiKeys.Services;
using SportsData.Modules.ApiKeys.Application.RateLimiting;
using SportsData.Modules.ApiKeys.Application.UsageLogging;
using SportsData.Modules.ApiKeys.Infrastructure.Seeders;

namespace SportsData.Modules.ApiKeys
{
    public static class ApiKeysModuleExtensions
    {
        public static IServiceCollection AddApiKeysModule(this IServiceCollection services, IConfiguration configuration)
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

            services.AddDbContext<ApiKeysDbContext>(options =>
            {
                if (databaseProvider.Equals("MySQL", StringComparison.OrdinalIgnoreCase))
                {
                    var serverVersion = new MySqlServerVersion(new Version(8, 0, 21));
                    options.UseMySql(connectionString, serverVersion, mySqlOptions =>
                        mySqlOptions.MigrationsAssembly("SportsData.Modules.ApiKeys"));
                }
                else
                {
                    options.UseSqlServer(connectionString, sqlOptions =>
                        sqlOptions.MigrationsAssembly("SportsData.Modules.ApiKeys"));
                }
                
                // Suppress pending model changes warning
                options.ConfigureWarnings(warnings => 
                    warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
            });

            services.AddMemoryCache();

            // Register MediatR Handlers from this assembly
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ApiKeysModuleExtensions).Assembly));

            // Register Validators
            services.AddValidatorsFromAssembly(typeof(ApiKeysModuleExtensions).Assembly);

            // Register Services
            services.AddScoped<IApiKeyRepository, ApiKeyRepository>();
            services.AddScoped<IApiKeyService, ApiKeyService>();
            services.AddScoped<IRateLimitService, RateLimitService>();
            services.AddScoped<IUsageLogService, UsageLogService>();
            services.AddScoped<ApiKeysSeeder>();

            return services;
        }
    }
}
