using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SportsData.Modules.Competitions.Infrastructure;
using SportsData.Shared;

namespace SportsData.Modules.Competitions.Application.Players.Queries
{
    public record GetPlayerCareerQuery(Guid PlayerId) : IRequest<Result<PlayerCareerDto>>;

    public record PlayerCareerDto(
        Guid PlayerId,
        string PlayerName,
        string Nationality,
        string Position,
        List<CareerEntryDto> Career);

    public record CareerEntryDto(
        Guid TeamId,
        string TeamName,
        string TeamCode,
        Guid SeasonId,
        string SeasonYear,
        int? ShirtNumber,
        DateTime StartDate,
        DateTime? EndDate,
        bool IsCurrent);

    public class GetPlayerCareerQueryValidator : AbstractValidator<GetPlayerCareerQuery>
    {
        public GetPlayerCareerQueryValidator()
        {
            RuleFor(x => x.PlayerId)
                .NotEmpty()
                .WithMessage("Player ID is required");
        }
    }

    public class GetPlayerCareerQueryHandler : IRequestHandler<GetPlayerCareerQuery, Result<PlayerCareerDto>>
    {
        private readonly CompetitionsDbContext _dbContext;
        private readonly IMemoryCache _cache;

        public GetPlayerCareerQueryHandler(CompetitionsDbContext dbContext, IMemoryCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        public async Task<Result<PlayerCareerDto>> Handle(GetPlayerCareerQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"player_career_{request.PlayerId}";

            if (_cache.TryGetValue(cacheKey, out PlayerCareerDto? cachedResult) && cachedResult != null)
            {
                return Result<PlayerCareerDto>.Success(cachedResult, StatusCodes.Status200OK);
            }

            var player = await _dbContext.Players
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == request.PlayerId, cancellationToken);

            if (player == null)
            {
                return Result<PlayerCareerDto>.Failure(
                    Error.NotFound("Player.NotFound", $"Player with ID {request.PlayerId} was not found"),
                    StatusCodes.Status404NotFound);
            }

            var career = await (from tps in _dbContext.TeamPlayerSeasons.AsNoTracking()
                               join team in _dbContext.Teams on tps.TeamId equals team.Id
                               join season in _dbContext.Seasons on tps.SeasonId equals season.Id
                               where tps.PlayerId == request.PlayerId
                               orderby tps.StartDate descending
                               select new CareerEntryDto(
                                   team.Id,
                                   team.Name,
                                   team.Code,
                                   season.Id,
                                   season.Year,
                                   tps.ShirtNumber,
                                   tps.StartDate,
                                   tps.EndDate,
                                   tps.EndDate == null))
                              .ToListAsync(cancellationToken);

            var result = new PlayerCareerDto(
                player.Id,
                player.Name,
                player.Nationality,
                player.Position,
                career);

            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(15));

            return Result<PlayerCareerDto>.Success(result, StatusCodes.Status200OK);
        }
    }
}
