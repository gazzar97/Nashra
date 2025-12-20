using Carter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace SportsData.Modules.Competitions.Presentation
{
    public class CompetitionsEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/competitions").WithTags("Competitions");

            group.MapGet("leagues", () =>
            {
                return Results.Ok(new[]
                {
                    new { Id = Guid.NewGuid(), Name = "Egyptian Premier League", Country = "Egypt" }
                });
            });
        }
    }
}
