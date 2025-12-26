using MediatR;
using Microsoft.Extensions.Logging;
using SportsData.Shared.Logging;
using System.Diagnostics;

namespace SportsData.Shared;

/// <summary>
/// MediatR pipeline behavior that automatically logs all query and command executions.
/// Provides complete traceability across CQRS operations.
/// </summary>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    private readonly ILoggingService _loggingService;

    public LoggingBehavior(
        ILogger<LoggingBehavior<TRequest, TResponse>> logger,
        ILoggingService loggingService)
    {
        _logger = logger;
        _loggingService = loggingService;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var correlationId = _loggingService.GetCorrelationId() ?? Guid.NewGuid().ToString();

        using var scope = _loggingService.BeginScope(correlationId);

        _loggingService.LogInformation(
            "Handling {RequestName} with CorrelationId {CorrelationId}",
            requestName,
            correlationId);

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var response = await next();

            stopwatch.Stop();

            _loggingService.LogInformation(
                "Handled {RequestName} successfully in {ElapsedMs}ms",
                requestName,
                stopwatch.ElapsedMilliseconds);

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            _loggingService.LogError(ex,
                "Error handling {RequestName} after {ElapsedMs}ms",
                requestName,
                stopwatch.ElapsedMilliseconds);

            throw;
        }
    }
}
