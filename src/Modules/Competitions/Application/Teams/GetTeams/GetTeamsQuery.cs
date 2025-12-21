using FluentValidation;
using MediatR;
using SportsData.Modules.Competitions.Application.Teams.Services;
using SportsData.Shared;
using SportsData.Shared.Caching;

namespace SportsData.Modules.Competitions.Application.Teams.GetTeams
{
    public record GetTeamsQuery : PagedRequest, IQuery<PagedList<TeamDto>>
    {
        public Guid? LeagueId { get; init; }
        public Guid? SeasonId { get; init; }
    }

    public class GetTeamsQueryValidator : AbstractValidator<GetTeamsQuery>
    {
        public GetTeamsQueryValidator()
        {
            RuleFor(x => x.Page).GreaterThan(0);
            RuleFor(x => x.PageSize).GreaterThan(0).LessThanOrEqualTo(100);
        }
    }

    public class GetTeamsQueryHandler : IQueryHandler<GetTeamsQuery, PagedList<TeamDto>>
    {
        private readonly ICacheService _cacheService;
        private readonly ITeamService _teamService;

        public GetTeamsQueryHandler(ICacheService cacheService, ITeamService teamService)
        {
            _cacheService = cacheService;
            _teamService = teamService;
        }

        public async Task<Result<PagedList<TeamDto>>> Handle(GetTeamsQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"teams_{request.LeagueId}_{request.SeasonId}_{request.Page}_{request.PageSize}";

            var pagedResult = await _cacheService.GetOrCreateAsync(
                cacheKey,
                async ct => await _teamService.GetTeamsAsync(request.LeagueId, request.SeasonId, request.Page, request.PageSize),
                TimeSpan.FromMinutes(10),
                cancellationToken);

            return Result<PagedList<TeamDto>>.Success(pagedResult);
        }
    }
}
