using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using SportsData.Modules.Competitions.Application.Players.Services;
using SportsData.Shared;

namespace SportsData.Modules.Competitions.Application.Players.Queries
{
    public record GetPlayersQuery(
        Guid? TeamId,
        Guid? SeasonId,
        string? Position,
        string? Nationality,
        int Page = 1,
        int PageSize = 10) : IRequest<Result<PagedList<PlayerDto>>>;

    public class GetPlayersQueryValidator : AbstractValidator<GetPlayersQuery>
    {
        public GetPlayersQueryValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThan(0)
                .WithMessage("Page must be greater than 0");

            RuleFor(x => x.PageSize)
                .GreaterThan(0)
                .LessThanOrEqualTo(100)
                .WithMessage("PageSize must be between 1 and 100");

            RuleFor(x => x.Position)
                .MaximumLength(50)
                .When(x => !string.IsNullOrWhiteSpace(x.Position));

            RuleFor(x => x.Nationality)
                .MaximumLength(100)
                .When(x => !string.IsNullOrWhiteSpace(x.Nationality));
        }
    }

    public class GetPlayersQueryHandler : IRequestHandler<GetPlayersQuery, Result<PagedList<PlayerDto>>>
    {
        private readonly IPlayerService _playerService;
        private readonly IMemoryCache _cache;

        public GetPlayersQueryHandler(IPlayerService playerService, IMemoryCache cache)
        {
            _playerService = playerService;
            _cache = cache;
        }

        public async Task<Result<PagedList<PlayerDto>>> Handle(GetPlayersQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"players_{request.TeamId}_{request.SeasonId}_{request.Position}_{request.Nationality}_{request.Page}_{request.PageSize}";

            if (_cache.TryGetValue(cacheKey, out PagedList<PlayerDto>? cachedResult) && cachedResult != null)
            {
                return Result<PagedList<PlayerDto>>.Success(cachedResult, StatusCodes.Status200OK);
            }

            var players = await _playerService.GetPlayersAsync(
                request.TeamId,
                request.SeasonId,
                request.Position,
                request.Nationality,
                request.Page,
                request.PageSize);

            _cache.Set(cacheKey, players, TimeSpan.FromMinutes(5));

            return Result<PagedList<PlayerDto>>.Success(players, StatusCodes.Status200OK);
        }
    }
}
