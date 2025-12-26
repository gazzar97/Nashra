using Microsoft.AspNetCore.Http;
using Serilog.Core;
using Serilog.Events;

namespace SportsData.Shared.Logging.Enrichers;

/// <summary>
/// Enriches log events with HTTP request context information.
/// </summary>
public class RequestContextEnricher : ILogEnricher
{
    private readonly IHttpContextAccessor? _httpContextAccessor;

    public RequestContextEnricher(IHttpContextAccessor? httpContextAccessor = null)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var httpContext = _httpContextAccessor?.HttpContext;
        if (httpContext == null)
        {
            return;
        }

        // Add request path
        if (httpContext.Request?.Path.HasValue == true)
        {
            logEvent.AddPropertyIfAbsent(
                propertyFactory.CreateProperty("RequestPath", httpContext.Request.Path.Value));
        }

        // Add HTTP method
        if (!string.IsNullOrEmpty(httpContext.Request?.Method))
        {
            logEvent.AddPropertyIfAbsent(
                propertyFactory.CreateProperty("HttpMethod", httpContext.Request.Method));
        }

        // Add user agent
        if (httpContext.Request?.Headers.TryGetValue("User-Agent", out var userAgent) == true)
        {
            logEvent.AddPropertyIfAbsent(
                propertyFactory.CreateProperty("UserAgent", userAgent.ToString()));
        }

        // Add client IP
        var clientIp = httpContext.Connection?.RemoteIpAddress?.ToString();
        if (!string.IsNullOrEmpty(clientIp))
        {
            logEvent.AddPropertyIfAbsent(
                propertyFactory.CreateProperty("ClientIp", clientIp));
        }

        // Add API Key ID if available (from custom context)
        if (httpContext.Items.TryGetValue("ApiKeyId", out var apiKeyId) && apiKeyId != null)
        {
            logEvent.AddPropertyIfAbsent(
                propertyFactory.CreateProperty("ApiKeyId", apiKeyId.ToString()!));
        }
    }
}
