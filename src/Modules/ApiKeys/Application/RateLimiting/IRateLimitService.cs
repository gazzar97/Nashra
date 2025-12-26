using SportsData.Shared;

namespace SportsData.Modules.ApiKeys.Application.RateLimiting
{
    public interface IRateLimitService
    {
        Task<Result<bool>> CheckRateLimitAsync(
            Guid apiKeyId,
            int limitPerMinute,
            int limitPerDay,
            CancellationToken cancellationToken = default);
        
        Task IncrementUsageAsync(Guid apiKeyId, CancellationToken cancellationToken = default);
        
        Task<(int remainingMinute, int remainingDay)> GetRemainingLimitsAsync(
            Guid apiKeyId,
            int limitPerMinute,
            int limitPerDay,
            CancellationToken cancellationToken = default);
    }
}
