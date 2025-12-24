using FluentValidation;
using MediatR;
using SportsData.Modules.Competitions.Application.Matches.Services;
using SportsData.Shared;
using SportsData.Shared.Caching;

namespace SportsData.Modules.Competitions.Application.Matches.Queries
{
    public record GetMatchesQuery : PagedRequest, IQuery<PagedList<MatchDto>>
    {
        public Guid SeasonId { get; init; }      // Required
        public Guid? TeamId { get; init; }       // Optional filter
        public DateTime? FromDate { get; init; } // Optional filter
        public DateTime? ToDate { get; init; }   // Optional filter
    }

    public class GetMatchesQueryValidator : AbstractValidator<GetMatchesQuery>
    {
        public GetMatchesQueryValidator()
        {
            RuleFor(x => x.SeasonId)
                .NotEmpty()
                .WithMessage("SeasonId is required.");

            RuleFor(x => x.Page)
                .GreaterThan(0)
                .WithMessage("Page must be greater than 0.");

            RuleFor(x => x.PageSize)
                .GreaterThan(0)
                .WithMessage("PageSize must be greater than 0.")
                .LessThanOrEqualTo(100)
                .WithMessage("PageSize cannot exceed 100.");

            RuleFor(x => x)
                .Must(x => !x.FromDate.HasValue || !x.ToDate.HasValue || x.FromDate.Value <= x.ToDate.Value)
                .WithMessage("FromDate must be less than or equal to ToDate.");
        }
    }

    public class GetMatchesQueryHandler : IQueryHandler<GetMatchesQuery, PagedList<MatchDto>>
    {
        private readonly ICacheService _cacheService;
        private readonly IMatchService _matchService;

        public GetMatchesQueryHandler(ICacheService cacheService, IMatchService matchService)
        {
            _cacheService = cacheService;
            _matchService = matchService;
        }

        public async Task<Result<PagedList<MatchDto>>> Handle(GetMatchesQuery request, CancellationToken cancellationToken)
        {
            // Build cache key from query parameters
            var cacheKey = $"matches_{request.SeasonId}_{request.TeamId}_{request.FromDate:yyyyMMdd}_{request.ToDate:yyyyMMdd}_{request.Page}_{request.PageSize}";

            var pagedResult = await _cacheService.GetOrCreateAsync(
                cacheKey,
                async ct => await _matchService.GetMatchesAsync(
                    request.SeasonId,
                    request.TeamId,
                    request.FromDate,
                    request.ToDate,
                    request.Page,
                    request.PageSize),
                TimeSpan.FromMinutes(5), // 5 minutes cache for live match scenarios
                cancellationToken);

            return Result<PagedList<MatchDto>>.Success(pagedResult);
        }
    }
}
