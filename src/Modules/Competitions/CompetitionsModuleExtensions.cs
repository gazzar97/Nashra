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
            services.AddScoped<Application.Players.Services.IPlayerService, Infrastructure.Players.PlayerService>();

            return services;
        }
    }
}
