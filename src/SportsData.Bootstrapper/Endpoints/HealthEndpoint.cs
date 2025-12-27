using Carter;

namespace SportsData.Bootstrapper.Endpoints;

public class HealthEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/health", () => Results.Ok(new
        {
            status = "healthy",
            timestamp = DateTime.UtcNow,
            service = "SportsData API"
        }))
        .WithName("HealthCheck")
        .WithTags("Health")
        .Produces(200)
        .WithOpenApi();
    }
}
