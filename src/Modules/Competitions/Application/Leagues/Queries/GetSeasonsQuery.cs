using FluentValidation;
using MediatR;
using SportsData.Modules.Competitions.Application.Leagues.Services;
using SportsData.Shared;
using SportsData.Shared.Caching;

namespace SportsData.Modules.Competitions.Application.Leagues.Queries
{
    public record GetSeasonsQuery : PagedRequest, IQuery<PagedList<SeasonDto>>
    {
        public Guid Id { get; init; }
    }

    public record SeasonDto(Guid Id, string Year, bool IsCurrent);

    public class GetSeasonsQueryValidator : AbstractValidator<GetSeasonsQuery>
    {
        public GetSeasonsQueryValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Page).GreaterThan(0);
            RuleFor(x => x.PageSize).GreaterThan(0).LessThanOrEqualTo(100);
        }
    }

    public class GetSeasonsQueryHandler : IQueryHandler<GetSeasonsQuery, PagedList<SeasonDto>>
    {
        private readonly ICacheService _cacheService;
        private readonly ILeagueService _leagueService;

        public GetSeasonsQueryHandler(ICacheService cacheService, ILeagueService leagueService)
        {
            _leagueService = leagueService;
            _cacheService = cacheService;
        }

        public async Task<Result<PagedList<SeasonDto>>> Handle(GetSeasonsQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"leagues_{request.Id}_seasons";

            var seasons = await _cacheService.GetOrCreateAsync(
                cacheKey,
                async ct => await _leagueService.GetSeasons(request.Id,request.Page,request.PageSize),
                TimeSpan.FromMinutes(30),
                cancellationToken);

            return Result<PagedList<SeasonDto>>.Success(seasons);
        }
    }
}
