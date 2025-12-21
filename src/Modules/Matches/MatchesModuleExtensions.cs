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

            services.AddDbContext<MatchesDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });
            // Register MediatR Handlers from this assembly
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(MatchesModuleExtensions).Assembly));

            // Register Validators
            services.AddValidatorsFromAssembly(typeof(MatchesModuleExtensions).Assembly);
            return services;
        }
    }
}
