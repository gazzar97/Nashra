using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SportsData.Modules.Competitions.Application.Leagues.GetLeagues;
using SportsData.Modules.Competitions.Application.Leagues.Queries;
using SportsData.Modules.Competitions.Application.Teams.Services;
using SportsData.Shared;
using Asp.Versioning;
using SportsData.Modules.Competitions.Application.Teams.Queries;
using SportsData.Modules.Competitions.Application.Matches.Queries;
using SportsData.Modules.Competitions.Application.Matches.Services;

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

            group.MapGet("leagues/{id}/seasons", async ([AsParameters] GetSeasonsQuery query, ISender sender) =>
            {
                var result = await sender.Send(query);
                return result.ToHttpResult();
            })
            .WithName("GetSeasons")
            .WithSummary("Retrieves a list of seasons for a league")
            .WithDescription("Retrieves all seasons for a specific league, ordered by year descending.")
            .Produces<Envelope<PagedList<SeasonDto>>>(StatusCodes.Status200OK);

            group.MapGet("seasons/{id}", (Guid id) => Results.Ok($"Season {id}"));
            
            group.MapGet("matches", async ([AsParameters] GetMatchesQuery query, ISender sender) =>
            {
                var result = await sender.Send(query);
                return result.ToHttpResult();
            })
            .WithName("GetMatches")
            .WithSummary("Retrieves match results for a season")
            .WithDescription("Retrieves a paginated list of matches with optional team and date filtering. Supports fixtures, live scores, and results.")
            .Produces<Envelope<PagedList<MatchDto>>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);

            group.MapGet("matches/{matchId}/stats", async (Guid matchId, ISender sender) =>
            {
                var query = new GetMatchStatsQuery(matchId);
                var result = await sender.Send(query);

                if (!result.IsSuccess)
                    return result.ToHttpResult();

                // Return 204 if stats not available
                if (result.Value == null)
                    return Results.NoContent();

                return result.ToHttpResult();
            })
            .WithName("GetMatchStats")
            .WithSummary("Retrieves detailed statistics for a specific match")
            .WithDescription("Returns possession, shots, cards, corners, and fouls for both teams. Returns 204 if stats are not yet available.")
            .Produces<Envelope<MatchStatsDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest);
            
            group.MapGet("teams", async ([AsParameters] GetTeamsQuery query, ISender sender) =>
            {
                var result = await sender.Send(query);
                return result.ToHttpResult();
            })
            .WithName("GetTeams")
            .WithSummary("Retrieves a list of teams")
            .WithDescription("Retrieves a paginated list of teams with optional league and season filtering.")
            .Produces<Envelope<PagedList<TeamDto>>>(StatusCodes.Status200OK);
            group.MapGet("teams/{id}", (Guid id) => Results.Ok($"Team {id}"));
            group.MapGet("players/{id}", (Guid id) => Results.Ok($"Player {id}"));
        }
    }
}
