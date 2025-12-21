using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SportsData.Modules.DataProcessing
{
    public static class DataProcessingModuleExtensions
    {
        public static IServiceCollection AddDataProcessingModule(this IServiceCollection services, IConfiguration configuration)
        {
            // Register MediatR Handlers from this assembly
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DataProcessingModuleExtensions).Assembly));

            // Register Validators
            services.AddValidatorsFromAssembly(typeof(DataProcessingModuleExtensions).Assembly);
            // Register services here
            return services;
        }
    }
}
