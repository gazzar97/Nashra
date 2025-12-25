using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SportsData.Modules.Matches.Application.Queries;
using SportsData.Shared;
using Asp.Versioning;

namespace SportsData.Modules.Matches.Presentation
{
    public class MatchesEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var apiVersionSet = app.NewApiVersionSet()
                .HasApiVersion(new Asp.Versioning.ApiVersion(1))
                .ReportApiVersions()
                .Build();

            var group = app.MapGroup("api/v{version:apiVersion}/matches")
                .WithTags("Matches")
                .WithApiVersionSet(apiVersionSet)
                .MapToApiVersion(1);

            group.MapGet("/", async ([AsParameters] GetMatchesQuery query, ISender sender) =>
            {
                var result = await sender.Send(query);
                return result.ToHttpResult();
            })
            .WithName("GetMatches")
            .WithSummary("Retrieves a list of matches")
            .WithDescription("Retrieves a paginated list of matches with optional filters for season, team, date range, and status.")
            .Produces<Envelope<PagedList<MatchDto>>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);

            group.MapGet("/{matchId}", async (Guid matchId, ISender sender) =>
            {
                var result = await sender.Send(new GetMatchByIdQuery(matchId));
                return result.ToHttpResult();
            })
            .WithName("GetMatchById")
            .WithSummary("Retrieves a single match by ID")
            .WithDescription("Returns detailed information about a specific match.")
            .Produces<Envelope<MatchDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

            group.MapGet("/{matchId}/statistics", async (Guid matchId, ISender sender) =>
            {
                var result = await sender.Send(new GetMatchStatisticsQuery(matchId));

                if (!result.IsSuccess)
                    return result.ToHttpResult();

                // Return 204 if statistics not available
                if (result.Value == null)
                    return Results.NoContent();

                return result.ToHttpResult();
            })
            .WithName("GetMatchStatistics")
            .WithSummary("Retrieves statistics for a specific match")
            .WithDescription("Returns comprehensive match statistics including possession, shots, cards, corners, and fouls. Returns 204 if statistics are not yet available.")
            .Produces<Envelope<MatchStatisticsDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}
