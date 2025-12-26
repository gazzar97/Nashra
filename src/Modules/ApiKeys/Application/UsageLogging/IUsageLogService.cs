using SportsData.Modules.ApiKeys.Domain;

namespace SportsData.Modules.ApiKeys.Application.UsageLogging
{
    public interface IUsageLogService
    {
        Task LogUsageAsync(
            Guid apiKeyId,
            string endpoint,
            string method,
            int statusCode,
            int responseTimeMs,
            CancellationToken cancellationToken = default);
    }
}
