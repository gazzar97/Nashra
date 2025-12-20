using Carter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace SportsData.Modules.Matches.Presentation
{
    public class MatchesEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/matches").WithTags("Matches");

            group.MapGet("/", () =>
            {
                return Results.Ok(new[]
                {
                    new { Id = Guid.NewGuid(), Match = "Al Ahly vs Zamalek", Status = "Scheduled" }
                });
            });
        }
    }
}
