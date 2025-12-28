using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SportsData.Modules.Competitions.Domain;
using SportsData.Modules.Competitions.Infrastructure;
using SportsData.Shared;

namespace SportsData.Modules.Competitions.Application.Teams.Commands
{
    public record AssignTeamToLeagueSeasonCommand(
        Guid TeamId,
        Guid LeagueId,
        Guid SeasonId) : IRequest<Result<Guid>>;

    public class AssignTeamToLeagueSeasonCommandValidator : AbstractValidator<AssignTeamToLeagueSeasonCommand>
    {
        public AssignTeamToLeagueSeasonCommandValidator()
        {
            RuleFor(x => x.TeamId)
                .NotEmpty()
                .WithMessage("Team ID is required");

            RuleFor(x => x.LeagueId)
                .NotEmpty()
                .WithMessage("League ID is required");

            RuleFor(x => x.SeasonId)
                .NotEmpty()
                .WithMessage("Season ID is required");
        }
    }

    public class AssignTeamToLeagueSeasonCommandHandler : IRequestHandler<AssignTeamToLeagueSeasonCommand, Result<Guid>>
    {
        private readonly CompetitionsDbContext _dbContext;
        private readonly IMemoryCache _cache;

        public AssignTeamToLeagueSeasonCommandHandler(CompetitionsDbContext dbContext, IMemoryCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        public async Task<Result<Guid>> Handle(AssignTeamToLeagueSeasonCommand request, CancellationToken cancellationToken)
        {
            // Verify team exists
            var teamExists = await _dbContext.Teams.AnyAsync(t => t.Id == request.TeamId, cancellationToken);
            if (!teamExists)
            {
                return Result<Guid>.Failure(
                    Error.NotFound("Team.NotFound", $"Team with ID {request.TeamId} was not found"),
                    StatusCodes.Status404NotFound);
            }

            // Verify league exists
            var leagueExists = await _dbContext.Leagues.AnyAsync(l => l.Id == request.LeagueId, cancellationToken);
            if (!leagueExists)
            {
                return Result<Guid>.Failure(
                    Error.NotFound("League.NotFound", $"League with ID {request.LeagueId} was not found"),
                    StatusCodes.Status404NotFound);
            }

            // Verify season exists and belongs to the league
            var season = await _dbContext.Seasons
                .FirstOrDefaultAsync(s => s.Id == request.SeasonId, cancellationToken);

            if (season == null)
            {
                return Result<Guid>.Failure(
                    Error.NotFound("Season.NotFound", $"Season with ID {request.SeasonId} was not found"),
                    StatusCodes.Status404NotFound);
            }

            var seasonLeagueIdProperty = typeof(Season).GetProperty("LeagueId");
            var seasonLeagueId = (Guid)seasonLeagueIdProperty!.GetValue(season)!;

            if (seasonLeagueId != request.LeagueId)
            {
                return Result<Guid>.Failure(
                    Error.Validation("Season.LeagueMismatch", "Season does not belong to the specified league"),
                    StatusCodes.Status400BadRequest);
            }

            // Check if assignment already exists
            var existingAssignment = await _dbContext.LeagueTeamSeasons
                .FirstOrDefaultAsync(lts =>
                    lts.LeagueId == request.LeagueId &&
                    lts.TeamId == request.TeamId &&
                    lts.SeasonId == request.SeasonId,
                    cancellationToken);

            if (existingAssignment != null)
            {
                return Result<Guid>.Failure(
                    Error.Conflict("Team.AlreadyAssigned", "Team is already assigned to this league for this season"),
                    StatusCodes.Status409Conflict);
            }

            var assignment = new LeagueTeamSeason(request.LeagueId, request.TeamId, request.SeasonId);

            _dbContext.LeagueTeamSeasons.Add(assignment);
            await _dbContext.SaveChangesAsync(cancellationToken);

            // Invalidate cache
            _cache.Remove("teams_");

            return Result<Guid>.Success(assignment.Id, StatusCodes.Status201Created);
        }
    }
}
