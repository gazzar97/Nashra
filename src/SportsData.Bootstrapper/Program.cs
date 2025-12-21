using SportsData.Modules.Competitions;
using SportsData.Modules.Matches;
using SportsData.Modules.DataProcessing;
using Carter;
using SportsData.Modules.Competitions.Infrastructure.Seeders;
using SportsData.Shared;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register Modules
builder.Services.AddCompetitionsModule(builder.Configuration);
builder.Services.AddMatchesModule(builder.Configuration);
builder.Services.AddDataProcessingModule(builder.Configuration);
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
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapCarter();

app.Run();
