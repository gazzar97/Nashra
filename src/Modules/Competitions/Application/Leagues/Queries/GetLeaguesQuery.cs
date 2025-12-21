using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SportsData.Modules.Competitions.Application.Leagues.Services;
using SportsData.Modules.Competitions.Infrastructure;
using SportsData.Shared;
using SportsData.Shared.Caching;

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
        private readonly ICacheService _cacheService;
        private readonly ILeagueService _leagueService;

        public GetLeaguesQueryHandler(ICacheService cacheService, ILeagueService leagueService)
        {
            _leagueService = leagueService;
            _cacheService = cacheService;
        }

        public async Task<Result<PagedList<LeagueDto>>> Handle(GetLeaguesQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"leagues_{request.Country}_{request.Page}_{request.PageSize}";

            var pagedResult = await _cacheService.GetOrCreateAsync(
                cacheKey,
                async ct => await _leagueService.GetLeagues(request.Country ?? string.Empty, request.Page, request.PageSize),
                TimeSpan.FromMinutes(10),
                cancellationToken);

            return Result<PagedList<LeagueDto>>.Success(pagedResult);
        }
    }
}
