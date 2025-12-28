using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SportsData.Modules.Competitions.Infrastructure;
using SportsData.Shared;

namespace SportsData.Modules.Competitions.Application.Leagues.Queries
{
    public record GetLeagueStandingsQuery(
        Guid LeagueId,
        Guid SeasonId) : IRequest<Result<List<StandingDto>>>;

    public record StandingDto(
        int Position,
        Guid TeamId,
        string TeamName,
        string TeamCode,
        int Played,
        int Won,
        int Drawn,
        int Lost,
        int GoalsFor,
        int GoalsAgainst,
        int GoalDifference,
        int Points);

    public class GetLeagueStandingsQueryValidator : AbstractValidator<GetLeagueStandingsQuery>
    {
        public GetLeagueStandingsQueryValidator()
        {
            RuleFor(x => x.LeagueId)
                .NotEmpty()
                .WithMessage("League ID is required");

            RuleFor(x => x.SeasonId)
                .NotEmpty()
                .WithMessage("Season ID is required");
        }
    }

    public class GetLeagueStandingsQueryHandler : IRequestHandler<GetLeagueStandingsQuery, Result<List<StandingDto>>>
    {
        private readonly CompetitionsDbContext _dbContext;
        private readonly IMemoryCache _cache;

        public GetLeagueStandingsQueryHandler(CompetitionsDbContext dbContext, IMemoryCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        public async Task<Result<List<StandingDto>>> Handle(GetLeagueStandingsQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"standings_{request.LeagueId}_{request.SeasonId}";

            if (_cache.TryGetValue(cacheKey, out List<StandingDto>? cachedResult) && cachedResult != null)
            {
                return Result<List<StandingDto>>.Success(cachedResult, StatusCodes.Status200OK);
            }

            // Verify league exists
            var leagueExists = await _dbContext.Leagues.AnyAsync(l => l.Id == request.LeagueId, cancellationToken);
            if (!leagueExists)
            {
                return Result<List<StandingDto>>.Failure(
                    Error.NotFound("League.NotFound", $"League with ID {request.LeagueId} was not found"),
                    StatusCodes.Status404NotFound);
            }

            // Verify season exists
            var seasonExists = await _dbContext.Seasons.AnyAsync(s => s.Id == request.SeasonId, cancellationToken);
            if (!seasonExists)
            {
                return Result<List<StandingDto>>.Failure(
                    Error.NotFound("Season.NotFound", $"Season with ID {request.SeasonId} was not found"),
                    StatusCodes.Status404NotFound);
            }

            var standings = await (from standing in _dbContext.LeagueStandings.AsNoTracking()
                                   join team in _dbContext.Teams on standing.TeamId equals team.Id
                                   where standing.LeagueId == request.LeagueId && standing.SeasonId == request.SeasonId
                                   orderby standing.Position
                                   select new StandingDto(
                                       standing.Position,
                                       standing.TeamId,
                                       team.Name,
                                       team.Code,
                                       standing.Played,
                                       standing.Won,
                                       standing.Drawn,
                                       standing.Lost,
                                       standing.GoalsFor,
                                       standing.GoalsAgainst,
                                       standing.GoalDifference,
                                       standing.Points))
                                  .ToListAsync(cancellationToken);

            _cache.Set(cacheKey, standings, TimeSpan.FromMinutes(5));

            return Result<List<StandingDto>>.Success(standings, StatusCodes.Status200OK);
        }
    }
}
