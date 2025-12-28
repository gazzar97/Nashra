using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SportsData.Modules.Competitions.Domain;
using SportsData.Modules.Competitions.Infrastructure;
using SportsData.Shared;

namespace SportsData.Modules.Competitions.Application.Players.Commands
{
    public record AssignPlayerToTeamCommand(
        Guid PlayerId,
        Guid TeamId,
        Guid SeasonId,
        int? ShirtNumber,
        DateTime StartDate,
        DateTime? EndDate = null) : IRequest<Result<Guid>>;

    public class AssignPlayerToTeamCommandValidator : AbstractValidator<AssignPlayerToTeamCommand>
    {
        public AssignPlayerToTeamCommandValidator()
        {
            RuleFor(x => x.PlayerId)
                .NotEmpty()
                .WithMessage("Player ID is required");

            RuleFor(x => x.TeamId)
                .NotEmpty()
                .WithMessage("Team ID is required");

            RuleFor(x => x.SeasonId)
                .NotEmpty()
                .WithMessage("Season ID is required");

            RuleFor(x => x.ShirtNumber)
                .GreaterThan(0)
                .LessThanOrEqualTo(99)
                .When(x => x.ShirtNumber.HasValue)
                .WithMessage("Shirt number must be between 1 and 99");

            RuleFor(x => x.StartDate)
                .NotEmpty()
                .WithMessage("Start date is required");

            RuleFor(x => x.EndDate)
                .GreaterThan(x => x.StartDate)
                .When(x => x.EndDate.HasValue)
                .WithMessage("End date must be after start date");
        }
    }

    public class AssignPlayerToTeamCommandHandler : IRequestHandler<AssignPlayerToTeamCommand, Result<Guid>>
    {
        private readonly CompetitionsDbContext _dbContext;
        private readonly IMemoryCache _cache;

        public AssignPlayerToTeamCommandHandler(CompetitionsDbContext dbContext, IMemoryCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        public async Task<Result<Guid>> Handle(AssignPlayerToTeamCommand request, CancellationToken cancellationToken)
        {
            // Verify player exists
            var playerExists = await _dbContext.Players.AnyAsync(p => p.Id == request.PlayerId, cancellationToken);
            if (!playerExists)
            {
                return Result<Guid>.Failure(
                    Error.NotFound("Player.NotFound", $"Player with ID {request.PlayerId} was not found"),
                    StatusCodes.Status404NotFound);
            }

            // Verify team exists
            var teamExists = await _dbContext.Teams.AnyAsync(t => t.Id == request.TeamId, cancellationToken);
            if (!teamExists)
            {
                return Result<Guid>.Failure(
                    Error.NotFound("Team.NotFound", $"Team with ID {request.TeamId} was not found"),
                    StatusCodes.Status404NotFound);
            }

            // Verify season exists
            var seasonExists = await _dbContext.Seasons.AnyAsync(s => s.Id == request.SeasonId, cancellationToken);
            if (!seasonExists)
            {
                return Result<Guid>.Failure(
                    Error.NotFound("Season.NotFound", $"Season with ID {request.SeasonId} was not found"),
                    StatusCodes.Status404NotFound);
            }

            // Check if player already assigned to this team in this season
            var existingAssignment = await _dbContext.TeamPlayerSeasons
                .FirstOrDefaultAsync(tps => 
                    tps.PlayerId == request.PlayerId && 
                    tps.TeamId == request.TeamId && 
                    tps.SeasonId == request.SeasonId, 
                    cancellationToken);

            if (existingAssignment != null)
            {
                return Result<Guid>.Failure(
                    Error.Conflict("Player.AlreadyAssigned", "Player is already assigned to this team for this season"),
                    StatusCodes.Status409Conflict);
            }

            var assignment = new TeamPlayerSeason(
                request.TeamId,
                request.PlayerId,
                request.SeasonId,
                request.ShirtNumber,
                request.StartDate,
                request.EndDate);

            _dbContext.TeamPlayerSeasons.Add(assignment);
            await _dbContext.SaveChangesAsync(cancellationToken);

            // Invalidate cache
            _cache.Remove($"player_{request.PlayerId}");
            _cache.Remove($"players_");

            return Result<Guid>.Success(assignment.Id, StatusCodes.Status201Created);
        }
    }
}
