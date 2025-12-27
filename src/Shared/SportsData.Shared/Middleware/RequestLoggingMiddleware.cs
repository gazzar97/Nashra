using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using SportsData.Shared.Logging;
using System.Diagnostics;

namespace SportsData.Shared.Middleware;

/// <summary>
/// Middleware for logging HTTP requests and responses with timing and correlation tracking.
/// </summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILoggingService _loggingService;

    public RequestLoggingMiddleware(RequestDelegate next, ILoggingService loggingService)
    {
        _next = next;
        _loggingService = loggingService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Get or generate correlation ID
        var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault()
                           ?? Guid.NewGuid().ToString();

        // Add correlation ID to response headers
        context.Response.Headers["X-Correlation-ID"] = correlationId;

        using var scope = _loggingService.BeginScope(correlationId);

        var stopwatch = Stopwatch.StartNew();

        try
        {
            _loggingService.LogInformation(
                "HTTP {Method} {Path} started",
                context.Request.Method,
                context.Request.Path);

            await _next(context);

            stopwatch.Stop();

            _loggingService.LogInformation(
                "HTTP {Method} {Path} completed with {StatusCode} in {ElapsedMs}ms",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            _loggingService.LogError(ex,
                "HTTP {Method} {Path} failed after {ElapsedMs}ms",
                context.Request.Method,
                context.Request.Path,
                stopwatch.ElapsedMilliseconds);

            throw;
        }
    }
}
public static class RequestLoggingMiddlewareExtensions
{
    /// <summary>
    /// Extension method to add the RequestLoggingMiddleware to the ASP.NET Core pipeline.
    /// </summary>
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestLoggingMiddleware>();
    }
}
