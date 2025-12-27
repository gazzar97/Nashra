using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using SportsData.Modules.Competitions.Infrastructure;
using FluentValidation;
using SportsData.Modules.Competitions.Infrastructure.Seeders;
using SportsData.Modules.Competitions.Application.Leagues.Services;
using SportsData.Modules.Competitions.Infrastructure.Leagues;
using SportsData.Modules.Competitions.Application.Teams.Services;
using SportsData.Modules.Competitions.Infrastructure.Teams;

namespace SportsData.Modules.Competitions
{
    public static class CompetitionsModuleExtensions
    {
        public static IServiceCollection AddCompetitionsModule(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var databaseProvider = configuration.GetValue<string>("DatabaseProvider") ?? "SqlServer";

            services.AddDbContext<CompetitionsDbContext>(options =>
             {
                if (databaseProvider.Equals("MySQL", StringComparison.OrdinalIgnoreCase))
                {
                    var serverVersion = new MySqlServerVersion(new Version(8, 0, 21));
                    options.UseMySql(connectionString, serverVersion, mySqlOptions =>
                        mySqlOptions.MigrationsAssembly("SportsData.Modules.Competitions"));
                }
                else
                {
                    options.UseSqlServer(connectionString, sqlOptions =>
                        sqlOptions.MigrationsAssembly("SportsData.Modules.Competitions"));
                }
                 
                // Suppress pending model changes warning
                options.ConfigureWarnings(warnings => 
                    warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
             });

            services.AddMemoryCache();

            // Register MediatR Handlers from this assembly
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CompetitionsModuleExtensions).Assembly));
            
            // Register Validators
            services.AddValidatorsFromAssembly(typeof(CompetitionsModuleExtensions).Assembly);

            services.AddScoped<LeaguesSeeder>();
            services.AddScoped<TeamsSeeder>();
            services.AddScoped<ILeagueService, LeagueService>();
            services.AddScoped<ITeamService, TeamService>();

            return services;
        }
    }
}
