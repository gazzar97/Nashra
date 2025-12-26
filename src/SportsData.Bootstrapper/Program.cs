using SportsData.Modules.Competitions;
using SportsData.Modules.Matches;
using SportsData.Modules.DataProcessing;
using SportsData.Modules.ApiKeys;
using Carter;
using SportsData.Modules.Competitions.Infrastructure.Seeders;
using SportsData.Shared;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

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

// Seed Data
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<LeaguesSeeder>();
    await seeder.SeedAsync();

    var teamsSeeder = scope.ServiceProvider.GetRequiredService<TeamsSeeder>();
    await teamsSeeder.SeedAsync();

    var apiKeysSeeder = scope.ServiceProvider.GetRequiredService<SportsData.Modules.ApiKeys.Infrastructure.Seeders.ApiKeysSeeder>();
    await apiKeysSeeder.SeedAsync();

    // TODO: Add match seeders to Matches module
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Enable API Key Validation Middleware
app.UseMiddleware<SportsData.Modules.ApiKeys.Middleware.ApiKeyValidationMiddleware>();

app.MapCarter();

app.Run();
