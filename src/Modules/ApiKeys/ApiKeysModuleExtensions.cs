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
            var connectionString = configuration.GetConnectionString("DefaultConnection");
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
