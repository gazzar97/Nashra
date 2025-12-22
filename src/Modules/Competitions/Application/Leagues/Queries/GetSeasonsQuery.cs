using FluentValidation;
using MediatR;
using SportsData.Modules.Competitions.Application.Leagues.Services;
using SportsData.Shared;
using SportsData.Shared.Caching;

namespace SportsData.Modules.Competitions.Application.Leagues.Queries
{
    public record GetSeasonsQuery(Guid Id) : IQuery<List<SeasonDto>>;

    public record SeasonDto(Guid Id, string Year, bool IsCurrent);

    public class GetSeasonsQueryValidator : AbstractValidator<GetSeasonsQuery>
    {
        public GetSeasonsQueryValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    public class GetSeasonsQueryHandler : IQueryHandler<GetSeasonsQuery, List<SeasonDto>>
    {
        private readonly ICacheService _cacheService;
        private readonly ILeagueService _leagueService;

        public GetSeasonsQueryHandler(ICacheService cacheService, ILeagueService leagueService)
        {
            _leagueService = leagueService;
            _cacheService = cacheService;
        }

        public async Task<Result<List<SeasonDto>>> Handle(GetSeasonsQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"leagues_{request.Id}_seasons";

            var seasons = await _cacheService.GetOrCreateAsync(
                cacheKey,
                async ct => await _leagueService.GetSeasons(request.Id),
                TimeSpan.FromMinutes(30),
                cancellationToken);

            return Result<List<SeasonDto>>.Success(seasons);
        }
    }
}
