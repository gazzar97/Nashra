using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SportsData.Modules.Competitions.Infrastructure;
using SportsData.Shared;

namespace SportsData.Modules.Competitions.Application.Leagues.Queries
{
    public record GetSeasonByIdQuery(Guid Id) : IRequest<Result<SeasonDetailDto>>;

    public record SeasonDetailDto(
        Guid Id,
        Guid LeagueId,
        string LeagueName,
        string Year,
        bool IsCurrent);

    public class GetSeasonByIdQueryValidator : AbstractValidator<GetSeasonByIdQuery>
    {
        public GetSeasonByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Season ID is required");
        }
    }

    public class GetSeasonByIdQueryHandler : IRequestHandler<GetSeasonByIdQuery, Result<SeasonDetailDto>>
    {
        private readonly CompetitionsDbContext _dbContext;
        private readonly IMemoryCache _cache;

        public GetSeasonByIdQueryHandler(CompetitionsDbContext dbContext, IMemoryCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        public async Task<Result<SeasonDetailDto>> Handle(GetSeasonByIdQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"season_{request.Id}";

            if (_cache.TryGetValue(cacheKey, out SeasonDetailDto? cachedResult) && cachedResult != null)
            {
                return Result<SeasonDetailDto>.Success(cachedResult, StatusCodes.Status200OK);
            }

            var season = await (from s in _dbContext.Seasons.AsNoTracking()
                               join l in _dbContext.Leagues on s.LeagueId equals l.Id
                               where s.Id == request.Id
                               select new SeasonDetailDto(
                                   s.Id,
                                   s.LeagueId,
                                   l.Name,
                                   s.Year,
                                   s.IsCurrent))
                              .FirstOrDefaultAsync(cancellationToken);

            if (season == null)
            {
                return Result<SeasonDetailDto>.Failure(
                    Error.NotFound("Season.NotFound", $"Season with ID {request.Id} was not found"),
                    StatusCodes.Status404NotFound);
            }

            _cache.Set(cacheKey, season, TimeSpan.FromMinutes(10));

            return Result<SeasonDetailDto>.Success(season, StatusCodes.Status200OK);
        }
    }
}
