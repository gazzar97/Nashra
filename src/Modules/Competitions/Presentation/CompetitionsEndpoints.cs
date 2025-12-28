using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SportsData.Modules.Competitions.Application.Leagues.GetLeagues;
using SportsData.Modules.Competitions.Application.Leagues.Queries;
using SportsData.Shared;
using Asp.Versioning;
using SportsData.Modules.Competitions.Application.Teams.Queries;
using SportsData.Modules.Competitions.Application.Teams.Services;
using SportsData.Modules.Competitions.Application.Players.Queries;
using SportsData.Modules.Competitions.Application.Players.Commands;
using SportsData.Modules.Competitions.Application.Players.Services;

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

            // ===== LEAGUE ENDPOINTS =====
            group.MapGet("leagues", async ([AsParameters] GetLeaguesQuery query, ISender sender) =>
            {
                var result = await sender.Send(query);
                return result.ToHttpResult();
            })
            .WithName("GetLeagues")
            .WithSummary("Retrieves a list of leagues")
            .WithDescription("Retrieves a paginated list of leagues with optional country filtering.")
            .Produces<Envelope<PagedList<LeagueDto>>>(StatusCodes.Status200OK);

            group.MapGet("leagues/{id}", async (Guid id, ISender sender) =>
            {
                var result = await sender.Send(new GetLeagueByIdQuery(id));
                return result.ToHttpResult();
            })
            .WithName("GetLeagueById")
            .WithSummary("Retrieves a league by ID")
            .WithDescription("Retrieves detailed information about a specific league.")
            .Produces<Envelope<LeagueDetailDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

            group.MapGet("leagues/{id}/seasons", async ([AsParameters] GetSeasonsQuery query, ISender sender) =>
            {
                var result = await sender.Send(query);
                return result.ToHttpResult();
            })
            .WithName("GetSeasons")
            .WithSummary("Retrieves a list of seasons for a league")
            .WithDescription("Retrieves all seasons for a specific league, ordered by year descending.")
            .Produces<Envelope<PagedList<SeasonDto>>>(StatusCodes.Status200OK);

            // ===== SEASON ENDPOINTS =====
            group.MapGet("seasons/{id}", async (Guid id, ISender sender) =>
            {
                var result = await sender.Send(new GetSeasonByIdQuery(id));
                return result.ToHttpResult();
            })
            .WithName("GetSeasonById")
            .WithSummary("Retrieves a season by ID")
            .WithDescription("Retrieves detailed information about a specific season including league information.")
            .Produces<Envelope<SeasonDetailDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

            // ===== TEAM ENDPOINTS =====
            group.MapGet("teams", async ([AsParameters] GetTeamsQuery query, ISender sender) =>
            {
                var result = await sender.Send(query);
                return result.ToHttpResult();
            })
            .WithName("GetTeams")
            .WithSummary("Retrieves a list of teams")
            .WithDescription("Retrieves a paginated list of teams with optional league and season filtering.")
            .Produces<Envelope<PagedList<TeamDto>>>(StatusCodes.Status200OK);

            group.MapGet("teams/{id}", async (Guid id, ISender sender) =>
            {
                var result = await sender.Send(new GetTeamByIdQuery(id));
                return result.ToHttpResult();
            })
            .WithName("GetTeamById")
            .WithSummary("Retrieves a team by ID")
            .WithDescription("Retrieves detailed information about a specific team.")
            .Produces<Envelope<TeamDetailDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

            // ===== PLAYER ENDPOINTS =====
            group.MapGet("players", async ([AsParameters] GetPlayersQuery query, ISender sender) =>
            {
                var result = await sender.Send(query);
                return result.ToHttpResult();
            })
            .WithName("GetPlayers")
            .WithSummary("Retrieves a list of players")
            .WithDescription("Retrieves a paginated list of players with optional filters for team, season, position, and nationality.")
            .Produces<Envelope<PagedList<Application.Players.Services.PlayerDto>>>(StatusCodes.Status200OK);

            group.MapGet("players/{id}", async (Guid id, ISender sender) =>
            {
                var result = await sender.Send(new GetPlayerByIdQuery(id));
                return result.ToHttpResult();
            })
            .WithName("GetPlayerById")
            .WithSummary("Retrieves a player by ID")
            .WithDescription("Retrieves detailed information about a specific player including current team and career history.")
            .Produces<Envelope<Application.Players.Services.PlayerDetailDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

            group.MapPost("players", async (CreatePlayerCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return result.ToHttpResult();
            })
            .WithName("CreatePlayer")
            .WithSummary("Creates a new player")
            .WithDescription("Creates a new player with the provided information.")
            .Produces<Envelope<Guid>>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

            group.MapPut("players/{id}", async (Guid id, UpdatePlayerCommand command, ISender sender) =>
            {
                if (id != command.Id)
                {
                    return Results.BadRequest(new { error = "ID mismatch between route and body" });
                }

                var result = await sender.Send(command);
                return result.ToHttpResult();
            })
            .WithName("UpdatePlayer")
            .WithSummary("Updates a player")
            .WithDescription("Updates an existing player's information.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

            group.MapPost("players/{playerId}/assign", async (Guid playerId, AssignPlayerToTeamRequest request, ISender sender) =>
            {
                var command = new AssignPlayerToTeamCommand(
                    playerId,
                    request.TeamId,
                    request.SeasonId,
                    request.ShirtNumber,
                    request.StartDate,
                    request.EndDate);

                var result = await sender.Send(command);
                return result.ToHttpResult();
            })
            .WithName("AssignPlayerToTeam")
            .WithSummary("Assigns a player to a team")
            .WithDescription("Creates a team-player-season assignment (transfer).")
            .Produces<Envelope<Guid>>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict);

            // ===== ADVANCED FEATURES =====
            
            // League Standings
            group.MapGet("leagues/{leagueId}/seasons/{seasonId}/standings", async (Guid leagueId, Guid seasonId, ISender sender) =>
            {
                var result = await sender.Send(new GetLeagueStandingsQuery(leagueId, seasonId));
                return result.ToHttpResult();
            })
            .WithName("GetLeagueStandings")
            .WithSummary("Retrieves league standings/table")
            .WithDescription("Retrieves the league table/standings for a specific league and season.")
            .Produces<Envelope<List<StandingDto>>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

            // Team Squad
            group.MapGet("teams/{teamId}/squad", async (Guid teamId, Guid seasonId, int page, int pageSize, ISender sender) =>
            {
                var result = await sender.Send(new GetTeamSquadQuery(teamId, seasonId, page, pageSize));
                return result.ToHttpResult();
            })
            .WithName("GetTeamSquad")
            .WithSummary("Retrieves team squad")
            .WithDescription("Retrieves the complete squad/roster for a team in a specific season.")
            .Produces<Envelope<PagedList<SquadPlayerDto>>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

            // Player Career History
            group.MapGet("players/{playerId}/career", async (Guid playerId, ISender sender) =>
            {
                var result = await sender.Send(new GetPlayerCareerQuery(playerId));
                return result.ToHttpResult();
            })
            .WithName("GetPlayerCareer")
            .WithSummary("Retrieves player career history")
            .WithDescription("Retrieves the complete career history of a player across all teams and seasons.")
            .Produces<Envelope<PlayerCareerDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
        }
    }

    // Request DTOs
    public record AssignPlayerToTeamRequest(
        Guid TeamId,
        Guid SeasonId,
        int? ShirtNumber,
        DateTime StartDate,
        DateTime? EndDate);
}
