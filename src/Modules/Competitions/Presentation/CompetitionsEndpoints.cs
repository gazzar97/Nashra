using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SportsData.Modules.Competitions.Application.Leagues.GetLeagues;
using SportsData.Shared;
using Asp.Versioning;

namespace SportsData.Modules.Competitions.Presentation
{
    public class CompetitionsEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var apiVersionSet = app.NewApiVersionSet()
                .HasApiVersion(new Asp.Versioning.ApiVersion(1))
                .ReportApiVersions()
                .Build();

            var group = app.MapGroup("api/v{version:apiVersion}/competitions")
                .WithTags("Competitions")
                .WithApiVersionSet(apiVersionSet)
                .MapToApiVersion(1);

            group.MapGet("leagues", async ([AsParameters] GetLeaguesQuery query, ISender sender) =>
            {
                var result = await sender.Send(query);
                return result.ToHttpResult();
            })
            .WithName("GetLeagues")
            .WithSummary("Retrieves a list of leagues")
            .WithDescription("Retrieves a paginated list of leagues with optional country filtering.")
            .Produces<Envelope<PagedList<LeagueDto>>>(StatusCodes.Status200OK);

            group.MapGet("leagues/{id}", (Guid id) => Results.Ok($"League {id}"));
            group.MapGet("leagues/{id}/seasons", (Guid id) => Results.Ok($"Seasons for League {id}"));
            group.MapGet("seasons/{id}", (Guid id) => Results.Ok($"Season {id}"));
            group.MapGet("teams", () => Results.Ok("Teams"));
            group.MapGet("teams/{id}", (Guid id) => Results.Ok($"Team {id}"));
            group.MapGet("players/{id}", (Guid id) => Results.Ok($"Player {id}"));
        }
    }
}
