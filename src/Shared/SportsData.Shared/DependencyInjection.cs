using Microsoft.Extensions.DependencyInjection;
using SportsData.Shared.Caching;

namespace SportsData.Shared
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddSharedServices(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddSingleton<ICacheService, InMemoryCacheService>();
            return services;
        }
    }
}
