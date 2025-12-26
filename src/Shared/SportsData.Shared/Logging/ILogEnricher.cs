using Serilog.Core;
using Serilog.Events;

namespace SportsData.Shared.Logging;

/// <summary>
/// Interface for custom log enrichers that add contextual information to log events.
/// </summary>
public interface ILogEnricher : ILogEventEnricher
{
    // Inherits: void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory);
}
