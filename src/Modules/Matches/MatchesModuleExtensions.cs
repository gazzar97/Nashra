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
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var databaseProvider = configuration.GetValue<string>("DatabaseProvider") ?? "SqlServer";

            services.AddDbContext<MatchesDbContext>(options =>
            {
                if (databaseProvider.Equals("PostgreSQL", StringComparison.OrdinalIgnoreCase))
                {
                    options.UseNpgsql(connectionString, npgsqlOptions =>
                        npgsqlOptions.MigrationsAssembly("SportsData.Modules.Matches"));
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
