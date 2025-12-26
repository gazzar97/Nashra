using SportsData.Modules.ApiKeys.Domain;
using SportsData.Modules.ApiKeys.Infrastructure;

namespace SportsData.Modules.ApiKeys.Application.UsageLogging
{
    public class UsageLogService : IUsageLogService
    {
        private readonly ApiKeysDbContext _context;

        public UsageLogService(ApiKeysDbContext context)
        {
            _context = context;
        }

        public async Task LogUsageAsync(
            Guid apiKeyId,
            string endpoint,
            string method,
            int statusCode,
            int responseTimeMs,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var log = ApiUsageLog.Create(apiKeyId, endpoint, method, statusCode, responseTimeMs);
                await _context.ApiUsageLogs.AddAsync(log, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch
            {
                // Swallow exceptions to prevent logging failures from breaking API requests
                // In production, consider using a background queue or logging service
            }
        }
    }
}
