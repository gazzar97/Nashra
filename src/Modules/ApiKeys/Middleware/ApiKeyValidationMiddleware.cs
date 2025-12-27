using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using SportsData.Modules.ApiKeys.Application.ApiKeys.Services;
using SportsData.Modules.ApiKeys.Application.RateLimiting;
using SportsData.Modules.ApiKeys.Application.UsageLogging;
using SportsData.Modules.ApiKeys.Infrastructure.Repositories;
using SportsData.Shared;

namespace SportsData.Modules.ApiKeys.Middleware
{
    public class ApiKeyValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string[] _excludedPaths = new[]
        {
            "/swagger",
            "/health",
            "/metrics"
        };

        public ApiKeyValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(
            HttpContext context,
            IApiKeyService apiKeyService,
            IRateLimitService rateLimitService,
            IUsageLogService usageLogService,
            IApiKeyRepository repository)
        {
            var stopwatch = Stopwatch.StartNew();

            // Skip validation for excluded paths
            if (_excludedPaths.Any(path => context.Request.Path.StartsWithSegments(path)))
            {
                await _next(context);
                return;
            }

            // Extract API key from header
            if (!context.Request.Headers.TryGetValue("X-API-Key", out var apiKeyValue) ||
                string.IsNullOrWhiteSpace(apiKeyValue))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                var envelope = Envelope<object>.Failure(new[] { "API key is required" });
                await context.Response.WriteAsJsonAsync(envelope);
                return;
            }

            var rawKey = apiKeyValue.ToString();

            // Validate API key
            var validationResult = await apiKeyService.ValidateApiKeyAsync(rawKey, context.RequestAborted);
            if (!validationResult.IsSuccess)
            {
                var statusCode = validationResult.Errors.Any(e => e.Contains("revoked") || e.Contains("expired") || e.Contains("not active"))
                    ? StatusCodes.Status403Forbidden
                    : StatusCodes.Status401Unauthorized;

                context.Response.StatusCode = statusCode;
                var envelope = Envelope<object>.Failure(validationResult.Errors);
                await context.Response.WriteAsJsonAsync(envelope);
                return;
            }

            var apiKey = validationResult.Value!;

            // Check rate limits
            var rateLimitResult = await rateLimitService.CheckRateLimitAsync(
                apiKey.Id,
                apiKey.RateLimitPerMinute,
                apiKey.RateLimitPerDay,
                context.RequestAborted);

            if (!rateLimitResult.IsSuccess)
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                var envelope = Envelope<object>.Failure(rateLimitResult.Errors);
                await context.Response.WriteAsJsonAsync(envelope);
                return;
            }

            // Increment usage counters
            await rateLimitService.IncrementUsageAsync(apiKey.Id, context.RequestAborted);

            // Get remaining limits for response headers
            var (remainingMinute, remainingDay) = await rateLimitService.GetRemainingLimitsAsync(
                apiKey.Id,
                apiKey.RateLimitPerMinute,
                apiKey.RateLimitPerDay,
                context.RequestAborted);

            // Add rate limit headers
            context.Response.Headers.Add("X-RateLimit-Limit-Minute", apiKey.RateLimitPerMinute.ToString());
            context.Response.Headers.Add("X-RateLimit-Remaining-Minute", remainingMinute.ToString());
            context.Response.Headers.Add("X-RateLimit-Limit-Day", apiKey.RateLimitPerDay.ToString());
            context.Response.Headers.Add("X-RateLimit-Remaining-Day", remainingDay.ToString());

            // Attach API key context to HttpContext
            var apiKeyContext = new ApiKeyContext
            {
                ApiKeyId = apiKey.Id,
                OwnerId = apiKey.OwnerId,
                Plan = apiKey.Plan,
                RateLimitPerMinute = apiKey.RateLimitPerMinute,
                RateLimitPerDay = apiKey.RateLimitPerDay
            };
            context.Items["ApiKeyContext"] = apiKeyContext;

            // Update last used timestamp (fire-and-forget)
            _ = Task.Run(async () =>
            {
                try
                {
                    apiKey.UpdateLastUsed();
                    await repository.UpdateAsync(apiKey);
                    await repository.SaveChangesAsync();
                }
                catch
                {
                    // Swallow exceptions to prevent breaking the request
                }
            });

            // Continue pipeline
            await _next(context);

            stopwatch.Stop();

            // Log usage (fire-and-forget)
            _ = Task.Run(async () =>
            {
                try
                {
                    await usageLogService.LogUsageAsync(
                        apiKey.Id,
                        context.Request.Path,
                        context.Request.Method,
                        context.Response.StatusCode,
                        (int)stopwatch.ElapsedMilliseconds);
                }
                catch
                {
                    // Swallow exceptions to prevent breaking the request
                }
            });
        }
    }

    public static class ApiKeyValidationMiddlewareExtensions
    {
        public static IApplicationBuilder UseApiKeyValidation(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApiKeyValidationMiddleware>();
        }
    }
}
