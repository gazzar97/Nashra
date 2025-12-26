using Microsoft.AspNetCore.Http;
using Serilog.Core;
using Serilog.Events;

namespace SportsData.Shared.Logging.Enrichers;

/// <summary>
/// Enriches log events with correlation ID from HTTP context or generates a new one.
/// </summary>
public class CorrelationIdEnricher : ILogEnricher
{
    private const string CorrelationIdPropertyName = "CorrelationId";
    private const string CorrelationIdHeaderName = "X-Correlation-ID";
    private static readonly AsyncLocal<string?> _correlationId = new();
    private readonly IHttpContextAccessor? _httpContextAccessor;

    public CorrelationIdEnricher(IHttpContextAccessor? httpContextAccessor = null)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        // Skip if already enriched
        if (logEvent.Properties.ContainsKey(CorrelationIdPropertyName))
        {
            return;
        }

        var correlationId = GetOrCreateCorrelationId();
        var property = propertyFactory.CreateProperty(CorrelationIdPropertyName, correlationId);
        logEvent.AddPropertyIfAbsent(property);
    }

    private string GetOrCreateCorrelationId()
    {
        // Try to get from HTTP context header
        if (_httpContextAccessor?.HttpContext != null)
        {
            if (_httpContextAccessor.HttpContext.Request.Headers.TryGetValue(
                CorrelationIdHeaderName, out var headerValue) && !string.IsNullOrEmpty(headerValue))
            {
                var correlationId = headerValue.ToString();
                _correlationId.Value = correlationId;
                return correlationId;
            }
        }

        // Try to get from AsyncLocal
        if (!string.IsNullOrEmpty(_correlationId.Value))
        {
            return _correlationId.Value;
        }

        // Generate new correlation ID
        var newCorrelationId = Guid.NewGuid().ToString();
        _correlationId.Value = newCorrelationId;
        return newCorrelationId;
    }
}
