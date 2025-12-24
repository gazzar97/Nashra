using FluentValidation;
using MediatR;
using SportsData.Modules.Competitions.Application.Matches.Services;
using SportsData.Shared;
using SportsData.Shared.Caching;

namespace SportsData.Modules.Competitions.Application.Matches.Queries
{
    public record GetMatchStatsQuery(Guid MatchId) : IQuery<MatchStatsDto?>;

    public class GetMatchStatsQueryValidator : AbstractValidator<GetMatchStatsQuery>
    {
        public GetMatchStatsQueryValidator()
        {
            RuleFor(x => x.MatchId)
                .NotEmpty()
                .WithMessage("MatchId is required.");
        }
    }

    public class GetMatchStatsQueryHandler : IQueryHandler<GetMatchStatsQuery, MatchStatsDto?>
    {
        private readonly ICacheService _cacheService;
        private readonly IMatchStatsService _matchStatsService;

        public GetMatchStatsQueryHandler(ICacheService cacheService, IMatchStatsService matchStatsService)
        {
            _cacheService = cacheService;
            _matchStatsService = matchStatsService;
        }

        public async Task<Result<MatchStatsDto?>> Handle(GetMatchStatsQuery request, CancellationToken cancellationToken)
        {
            // Build cache key from match ID
            var cacheKey = $"match_stats_{request.MatchId}";

            var statsDto = await _cacheService.GetOrCreateAsync(
                cacheKey,
                async ct => await _matchStatsService.GetMatchStatsAsync(request.MatchId),
                TimeSpan.FromMinutes(10), // 10 minutes cache (stats are relatively stable)
                cancellationToken);

            // Return success even if null (null indicates no stats available - 204 scenario)
            return Result<MatchStatsDto?>.Success(statsDto);
        }
    }
}
