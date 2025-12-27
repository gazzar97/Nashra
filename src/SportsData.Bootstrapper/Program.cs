using SportsData.Modules.Competitions;
using SportsData.Modules.Matches;
using SportsData.Modules.DataProcessing;
using SportsData.Modules.ApiKeys;
using Carter;
using SportsData.Modules.Competitions.Infrastructure.Seeders;
using SportsData.Shared;
using SportsData.Shared.Middleware;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using SportsData.Modules.ApiKeys.Middleware;
using SportsData.Bootstrapper.Extensions;
using System.Text.RegularExpressions;

// Configure Serilog BEFORE building the host
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .WriteTo.Console()
    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);


// Replace default logging with Serilog
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId());

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Add API Key security definition
    options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "API Key needed to access the endpoints. Format: X-API-Key: {your key}",
        In = ParameterLocation.Header,
        Name = "X-API-Key",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "ApiKeyScheme"
    });

    // Make API Key required globally
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                },
                Scheme = "ApiKeyScheme",
                Name = "X-API-Key",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

// Register Modules
builder.Services.AddCompetitionsModule(builder.Configuration);
builder.Services.AddMatchesModule(builder.Configuration);
builder.Services.AddDataProcessingModule(builder.Configuration);
builder.Services.AddApiKeysModule(builder.Configuration);
builder.Services.AddSharedServices();

// Shared Kernel Behaviors
builder.Services.AddTransient(typeof(MediatR.IPipelineBehavior<,>), typeof(SportsData.Shared.ValidationBehavior<,>));
builder.Services.AddTransient(typeof(MediatR.IPipelineBehavior<,>), typeof(SportsData.Shared.LoggingBehavior<,>));

// Add Carter
builder.Services.AddCarter();

// API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1);
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = Asp.Versioning.ApiVersionReader.Combine(
        new Asp.Versioning.UrlSegmentApiVersionReader(),
        new Asp.Versioning.HeaderApiVersionReader("X-Api-Version"));
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'V";
    options.SubstituteApiVersionInUrl = true;
});


var app = builder.Build();

// Apply Database Migrations
await app.Services.ApplyMigrationsAsync(app.Logger);

// Seed Data
/*using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<LeaguesSeeder>();
    await seeder.SeedAsync();

    var teamsSeeder = scope.ServiceProvider.GetRequiredService<TeamsSeeder>();
    await teamsSeeder.SeedAsync();

    var apiKeysSeeder = scope.ServiceProvider.GetRequiredService<SportsData.Modules.ApiKeys.Infrastructure.Seeders.ApiKeysSeeder>();
    await apiKeysSeeder.SeedAsync();

    // TODO: Add match seeders to Matches module
}*/

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Enable Request Logging Middleware (before API Key validation)
app.UseRequestLogging();
// Enable API Key Validation Middleware
app.UseApiKeyValidation();

app.MapCarter();

app.Run();

// Helper method to parse Railway's DATABASE_URL format
//static string ParseDatabaseUrl(string databaseUrl)
//{
//    // Railway format: mysql://user:password@host:port/database
//    var regex = new Regex(@"mysql://(?<user>[^:]+):(?<password>[^@]+)@(?<host>[^:]+):(?<port>\d+)/(?<database>.+)");
//    var match = regex.Match(databaseUrl);
    
//    if (!match.Success)
//    {
//        throw new InvalidOperationException($"Invalid DATABASE_URL format: {databaseUrl}");
//    }
    
//    var user = match.Groups["user"].Value;
//    var password = match.Groups["password"].Value;
//    var host = match.Groups["host"].Value;
//    var port = match.Groups["port"].Value;
//    var database = match.Groups["database"].Value;
    
//    return $"Server={host};Port={port};Database={database};User={user};Password={password};SslMode=Required;";
//}
