using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SportsData.Modules.Competitions.Infrastructure;
using SportsData.Shared;

namespace SportsData.Modules.Competitions.Application.Leagues.Queries
{
    public record GetLeagueByIdQuery(Guid Id) : IRequest<Result<LeagueDetailDto>>;

    public record LeagueDetailDto(
        Guid Id,
        string Name,
        string Country,
        string LogoUrl,
        int Tier,
        string SeasonStart,
        string SeasonEnd);

    public class GetLeagueByIdQueryValidator : AbstractValidator<GetLeagueByIdQuery>
    {
        public GetLeagueByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("League ID is required");
        }
    }

    public class GetLeagueByIdQueryHandler : IRequestHandler<GetLeagueByIdQuery, Result<LeagueDetailDto>>
    {
        private readonly CompetitionsDbContext _dbContext;
        private readonly IMemoryCache _cache;

        public GetLeagueByIdQueryHandler(CompetitionsDbContext dbContext, IMemoryCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        public async Task<Result<LeagueDetailDto>> Handle(GetLeagueByIdQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"league_{request.Id}";

            if (_cache.TryGetValue(cacheKey, out LeagueDetailDto? cachedResult) && cachedResult != null)
            {
                return Result<LeagueDetailDto>.Success(cachedResult, StatusCodes.Status200OK);
            }

            var league = await _dbContext.Leagues
                .AsNoTracking()
                .Where(l => l.Id == request.Id)
                .Select(l => new LeagueDetailDto(
                    l.Id,
                    l.Name,
                    l.Country,
                    l.LogoUrl,
                    l.Tier,
                    l.SeasonStart,
                    l.SeasonEnd))
                .FirstOrDefaultAsync(cancellationToken);

            if (league == null)
            {
                return Result<LeagueDetailDto>.Failure(
                    Error.NotFound("League.NotFound", $"League with ID {request.Id} was not found"),
                    StatusCodes.Status404NotFound);
            }

            _cache.Set(cacheKey, league, TimeSpan.FromMinutes(10));

            return Result<LeagueDetailDto>.Success(league, StatusCodes.Status200OK);
        }
    }
}
