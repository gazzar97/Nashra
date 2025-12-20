using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using SportsData.Modules.Competitions.Infrastructure;
using FluentValidation;

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

            // Register MediatR Handlers from this assembly
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CompetitionsModuleExtensions).Assembly));
            
            // Register Validators
            services.AddValidatorsFromAssembly(typeof(CompetitionsModuleExtensions).Assembly);

            return services;
        }
    }
}
