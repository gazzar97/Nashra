using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SportsData.Modules.Competitions.Application.Leagues.Services;
using SportsData.Modules.Competitions.Infrastructure;
using SportsData.Shared;

namespace SportsData.Modules.Competitions.Application.Leagues.GetLeagues
{
    public record GetLeaguesQuery : PagedRequest, IQuery<PagedList<LeagueDto>>
    {
        public string? Country { get; init; }
    }

    public record LeagueDto(Guid Id, string Name, string Country, string LogoUrl);

    public class GetLeaguesQueryValidator : AbstractValidator<GetLeaguesQuery>
    {
        public GetLeaguesQueryValidator()
        {
            RuleFor(x => x.Page).GreaterThan(0);
            RuleFor(x => x.PageSize).GreaterThan(0).LessThanOrEqualTo(100);
        }
    }

    public class GetLeaguesQueryHandler : IQueryHandler<GetLeaguesQuery, PagedList<LeagueDto>>
    {
        private readonly IMemoryCache _cache;
        private readonly ILeagueService _leagueService;

        public GetLeaguesQueryHandler(IMemoryCache cache,ILeagueService leagueService)
        {
            _leagueService = leagueService;
            _cache = cache;
        }
        public async Task<Result<PagedList<LeagueDto>>> Handle(GetLeaguesQuery request, CancellationToken cancellationToken)
        {
           
            var cacheKey = $"leagues_{request.Country}_{request.Page}_{request.PageSize}";

            if (_cache.TryGetValue(cacheKey, out PagedList<LeagueDto>? cachedLeagues) && cachedLeagues is not null)
            {
                return Result<PagedList<LeagueDto>>.Success(cachedLeagues);
            }

            var pagedResult = await _leagueService.GetLeagues(request.Country ?? string.Empty, request.Page, request.PageSize);
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));

            _cache.Set(cacheKey, pagedResult, cacheOptions);

            return Result<PagedList<LeagueDto>>.Success(pagedResult);
        }
    }
}
