using Microsoft.Extensions.Logging;

namespace SportsData.Shared.Logging;

/// <summary>
/// Generic logging service interface for application-wide logging with advanced features.
/// Provides abstraction over ILogger to enable future migration to different observability tools.
/// </summary>
public interface ILoggingService
{
    /// <summary>
    /// Logs a debug message.
    /// </summary>
    void LogDebug(string message, params object[] args);

    /// <summary>
    /// Logs an informational message.
    /// </summary>
    void LogInformation(string message, params object[] args);

    /// <summary>
    /// Logs a warning message.
    /// </summary>
    void LogWarning(string message, params object[] args);

    /// <summary>
    /// Logs an error message with exception details.
    /// </summary>
    void LogError(Exception? exception, string message, params object[] args);

    /// <summary>
    /// Logs a fatal/critical error with exception details.
    /// </summary>
    void LogFatal(Exception exception, string message, params object[] args);

    /// <summary>
    /// Logs a message with structured context data.
    /// </summary>
    void LogWithContext(LogLevel level, string message, object context);

    /// <summary>
    /// Begins a logical operation scope with a correlation ID.
    /// </summary>
    /// <param name="correlationId">Correlation ID to track related operations</param>
    /// <returns>Disposable scope that should be disposed when the operation completes</returns>
    IDisposable BeginScope(string correlationId);

    /// <summary>
    /// Gets the current correlation ID from the logging context.
    /// </summary>
    string? GetCorrelationId();

    /// <summary>
    /// Enriches subsequent log entries with a custom property.
    /// </summary>
    void EnrichWith(string propertyName, object value);
}
