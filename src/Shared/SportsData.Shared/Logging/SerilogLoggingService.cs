using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Context;
using ILogger = Serilog.ILogger;

namespace SportsData.Shared.Logging;

/// <summary>
/// Serilog-based implementation of ILoggingService.
/// Provides structured logging with correlation tracking and enrichment.
/// </summary>
public class SerilogLoggingService : ILoggingService
{
    private readonly ILogger _logger;
    private static readonly AsyncLocal<string?> _correlationId = new();

    public SerilogLoggingService()
    {
        _logger = Log.Logger;
    }

    public void LogDebug(string message, params object[] args)
    {
        _logger.Debug(message, args);
    }

    public void LogInformation(string message, params object[] args)
    {
        _logger.Information(message, args);
    }

    public void LogWarning(string message, params object[] args)
    {
        _logger.Warning(message, args);
    }

    public void LogError(Exception? exception, string message, params object[] args)
    {
        if (exception != null)
        {
            _logger.Error(exception, message, args);
        }
        else
        {
            _logger.Error(message, args);
        }
    }

    public void LogFatal(Exception exception, string message, params object[] args)
    {
        _logger.Fatal(exception, message, args);
    }

    public void LogWithContext(LogLevel level, string message, object context)
    {
        var logLevel = MapLogLevel(level);
        _logger.Write(logLevel, "{Message} {@Context}", message, context);
    }

    public IDisposable BeginScope(string correlationId)
    {
        _correlationId.Value = correlationId;
        return Serilog.Context.LogContext.PushProperty("CorrelationId", correlationId);
    }

    public string? GetCorrelationId()
    {
        return _correlationId.Value;
    }

    public void EnrichWith(string propertyName, object value)
    {
        Serilog.Context.LogContext.PushProperty(propertyName, value);
    }

    private static Serilog.Events.LogEventLevel MapLogLevel(LogLevel level)
    {
        return level switch
        {
            LogLevel.Trace => Serilog.Events.LogEventLevel.Verbose,
            LogLevel.Debug => Serilog.Events.LogEventLevel.Debug,
            LogLevel.Information => Serilog.Events.LogEventLevel.Information,
            LogLevel.Warning => Serilog.Events.LogEventLevel.Warning,
            LogLevel.Error => Serilog.Events.LogEventLevel.Error,
            LogLevel.Critical => Serilog.Events.LogEventLevel.Fatal,
            _ => Serilog.Events.LogEventLevel.Information
        };
    }
}
