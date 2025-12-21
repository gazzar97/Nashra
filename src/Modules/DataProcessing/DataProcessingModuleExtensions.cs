using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SportsData.Modules.DataProcessing
{
    public static class DataProcessingModuleExtensions
    {
        public static IServiceCollection AddDataProcessingModule(this IServiceCollection services, IConfiguration configuration)
        {
            // Register services here
            return services;
        }
    }
}
