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
using SportsData.Modules.Competitions.Application.Matches.Services;
using SportsData.Modules.Competitions.Infrastructure.Matches;

namespace SportsData.Modules.Competitions
{
    public static class CompetitionsModuleExtensions
    {
        public static IServiceCollection AddCompetitionsModule(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<CompetitionsDbContext>(options =>
             {
                 options.UseSqlServer(connectionString);
             });

            services.AddMemoryCache();

            // Register MediatR Handlers from this assembly
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CompetitionsModuleExtensions).Assembly));
            
            // Register Validators
            services.AddValidatorsFromAssembly(typeof(CompetitionsModuleExtensions).Assembly);

            services.AddScoped<LeaguesSeeder>();
            services.AddScoped<TeamsSeeder>();
            services.AddScoped<MatchesSeeder>();
            services.AddScoped<MatchStatsSeeder>();
            services.AddScoped<ILeagueService, LeagueService>();
            services.AddScoped<ITeamService, TeamService>();
            services.AddScoped<IMatchService, MatchService>();
            services.AddScoped<IMatchStatsService, MatchStatsService>();

            return services;
        }
    }
}
