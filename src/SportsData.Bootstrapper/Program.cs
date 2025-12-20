using SportsData.Modules.Competitions;
using SportsData.Modules.Matches;
using Carter;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register Modules
builder.Services.AddCompetitionsModule(builder.Configuration);
builder.Services.AddMatchesModule(builder.Configuration);

// Shared Kernel Behaviors
builder.Services.AddTransient(typeof(MediatR.IPipelineBehavior<,>), typeof(SportsData.Shared.ValidationBehavior<,>));

// Add Carter
builder.Services.AddCarter();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapCarter();

app.Run();
