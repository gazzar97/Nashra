using Microsoft.Extensions.Caching.Memory;
using SportsData.Shared;

namespace SportsData.Modules.ApiKeys.Application.RateLimiting
{
    public class RateLimitService : IRateLimitService
    {
        private readonly IMemoryCache _cache;

        public RateLimitService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public async Task<Result<bool>> CheckRateLimitAsync(
            Guid apiKeyId,
            int limitPerMinute,
            int limitPerDay,
            CancellationToken cancellationToken = default)
        {
            var minuteKey = $"rate:{apiKeyId}:minute";
            var dayKey = $"rate:{apiKeyId}:day";

            var minuteCount = _cache.Get<int>(minuteKey);
            var dayCount = _cache.Get<int>(dayKey);

            if (minuteCount >= limitPerMinute)
            {
                return await Task.FromResult(Result<bool>.Failure("Rate limit exceeded: too many requests per minute"));
            }

            if (dayCount >= limitPerDay)
            {
                return await Task.FromResult(Result<bool>.Failure("Rate limit exceeded: too many requests per day"));
            }

            return await Task.FromResult(Result<bool>.Success(true));
        }

        public async Task IncrementUsageAsync(Guid apiKeyId, CancellationToken cancellationToken = default)
        {
            var minuteKey = $"rate:{apiKeyId}:minute";
            var dayKey = $"rate:{apiKeyId}:day";

            // Increment minute counter with 1-minute sliding expiration
            var minuteCount = _cache.Get<int>(minuteKey);
            _cache.Set(minuteKey, minuteCount + 1, TimeSpan.FromMinutes(1));

            // Increment day counter with 24-hour sliding expiration
            var dayCount = _cache.Get<int>(dayKey);
            _cache.Set(dayKey, dayCount + 1, TimeSpan.FromHours(24));

            await Task.CompletedTask;
        }

        public async Task<(int remainingMinute, int remainingDay)> GetRemainingLimitsAsync(
            Guid apiKeyId,
            int limitPerMinute,
            int limitPerDay,
            CancellationToken cancellationToken = default)
        {
            var minuteKey = $"rate:{apiKeyId}:minute";
            var dayKey = $"rate:{apiKeyId}:day";

            var minuteCount = _cache.Get<int>(minuteKey);
            var dayCount = _cache.Get<int>(dayKey);

            var remainingMinute = Math.Max(0, limitPerMinute - minuteCount);
            var remainingDay = Math.Max(0, limitPerDay - dayCount);

            return await Task.FromResult((remainingMinute, remainingDay));
        }
    }
}
