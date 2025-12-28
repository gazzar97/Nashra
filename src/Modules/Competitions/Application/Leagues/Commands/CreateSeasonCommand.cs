using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SportsData.Modules.Competitions.Domain;
using SportsData.Modules.Competitions.Infrastructure;
using SportsData.Shared;

namespace SportsData.Modules.Competitions.Application.Leagues.Commands
{
    public record CreateSeasonCommand(
        Guid LeagueId,
        string Year,
        bool IsCurrent = false) : IRequest<Result<Guid>>;

    public class CreateSeasonCommandValidator : AbstractValidator<CreateSeasonCommand>
    {
        public CreateSeasonCommandValidator()
        {
            RuleFor(x => x.LeagueId)
                .NotEmpty()
                .WithMessage("League ID is required");

            RuleFor(x => x.Year)
                .NotEmpty()
                .MaximumLength(20)
                .WithMessage("Year is required and must not exceed 20 characters")
                .Matches(@"^\d{4}\/\d{4}$|^\d{4}-\d{4}$|^\d{4}$")
                .WithMessage("Year must be in format YYYY/YYYY, YYYY-YYYY, or YYYY");
        }
    }

    public class CreateSeasonCommandHandler : IRequestHandler<CreateSeasonCommand, Result<Guid>>
    {
        private readonly CompetitionsDbContext _dbContext;
        private readonly IMemoryCache _cache;

        public CreateSeasonCommandHandler(CompetitionsDbContext dbContext, IMemoryCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        public async Task<Result<Guid>> Handle(CreateSeasonCommand request, CancellationToken cancellationToken)
        {
            // Verify league exists
            var leagueExists = await _dbContext.Leagues.AnyAsync(l => l.Id == request.LeagueId, cancellationToken);
            if (!leagueExists)
            {
                return Result<Guid>.Failure(
                    Error.NotFound("League.NotFound", $"League with ID {request.LeagueId} was not found"),
                    StatusCodes.Status404NotFound);
            }

            // Check if season with same year already exists for this league
            var existingSeason = await _dbContext.Seasons
                .FirstOrDefaultAsync(s => s.LeagueId == request.LeagueId && s.Year == request.Year, cancellationToken);

            if (existingSeason != null)
            {
                return Result<Guid>.Failure(
                    Error.Conflict("Season.YearExists", $"A season with year '{request.Year}' already exists for this league"),
                    StatusCodes.Status409Conflict);
            }

            // If this season is marked as current, unmark all other seasons for this league
            if (request.IsCurrent)
            {
                var currentSeasons = await _dbContext.Seasons
                    .Where(s => s.LeagueId == request.LeagueId && s.IsCurrent)
                    .ToListAsync(cancellationToken);

                foreach (var currentSeason in currentSeasons)
                {
                    var isCurrentProperty = typeof(Season).GetProperty("IsCurrent");
                    isCurrentProperty?.SetValue(currentSeason, false);
                }
            }

            var season = new Season(request.LeagueId, request.Year, request.IsCurrent);

            _dbContext.Seasons.Add(season);
            await _dbContext.SaveChangesAsync(cancellationToken);

            // Invalidate cache
            _cache.Remove($"seasons_{request.LeagueId}");

            return Result<Guid>.Success(season.Id, StatusCodes.Status201Created);
        }
    }
}
