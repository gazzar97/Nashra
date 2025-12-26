using Microsoft.Extensions.DependencyInjection;
using SportsData.Shared.Caching;
using SportsData.Shared.Logging;
using SportsData.Shared.Logging.Enrichers;

namespace SportsData.Shared
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddSharedServices(this IServiceCollection services)
        {
            // Caching services
            services.AddMemoryCache();
            services.AddSingleton<ICacheService, InMemoryCacheService>();

            // HTTP Context Accessor for enrichers
            services.AddHttpContextAccessor();

            // Logging services
            services.AddSingleton<ILoggingService, SerilogLoggingService>();
            services.AddSingleton<ILogEnricher, CorrelationIdEnricher>();
            services.AddSingleton<ILogEnricher, ModuleContextEnricher>();
            services.AddSingleton<ILogEnricher, RequestContextEnricher>();

            // MediatR Logging Behavior (registered in Program.cs for all pipelines)

            return services;
        }
    }
}
