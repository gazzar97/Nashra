namespace SportsData.Shared.Logging;

/// <summary>
/// Represents structured logging context with metadata.
/// </summary>
public class LogContext
{
    /// <summary>
    /// Name of the module generating the log entry.
    /// </summary>
    public string? ModuleName { get; set; }

    /// <summary>
    /// Name of the operation being performed.
    /// </summary>
    public string? OperationName { get; set; }

    /// <summary>
    /// User identifier if available.
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Request identifier for tracking.
    /// </summary>
    public string? RequestId { get; set; }

    /// <summary>
    /// Additional custom properties.
    /// </summary>
    public Dictionary<string, object> Properties { get; set; } = new();

    /// <summary>
    /// Adds a custom property to the context.
    /// </summary>
    public void AddProperty(string key, object value)
    {
        Properties[key] = value;
    }
}
