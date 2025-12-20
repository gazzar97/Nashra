using SportsData.Modules.Competitions;
using SportsData.Modules.Matches;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();

// Register Modules
builder.Services.AddCompetitionsModule(builder.Configuration);
builder.Services.AddMatchesModule(builder.Configuration);

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
