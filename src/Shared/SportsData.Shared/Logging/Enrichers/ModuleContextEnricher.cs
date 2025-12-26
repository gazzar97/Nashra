using Serilog.Core;
using Serilog.Events;
using System.Reflection;

namespace SportsData.Shared.Logging.Enrichers;

/// <summary>
/// Enriches log events with module context information.
/// </summary>
public class ModuleContextEnricher : ILogEnricher
{
    private readonly string _moduleName;
    private readonly string _assemblyVersion;
    private readonly string _environment;

    public ModuleContextEnricher(string? moduleName = null, string? environment = null)
    {
        _moduleName = moduleName ?? "SportsData";
        _environment = environment ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
        
        var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
        _assemblyVersion = assembly.GetName().Version?.ToString() ?? "1.0.0";
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ModuleName", _moduleName));
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("AssemblyVersion", _assemblyVersion));
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("Environment", _environment));
    }
}
