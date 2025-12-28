using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using SportsData.Modules.Competitions.Application.Players.Services;
using SportsData.Shared;

namespace SportsData.Modules.Competitions.Application.Players.Queries
{
    public record GetPlayerByIdQuery(Guid Id) : IRequest<Result<PlayerDetailDto>>;

    public class GetPlayerByIdQueryValidator : AbstractValidator<GetPlayerByIdQuery>
    {
        public GetPlayerByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Player ID is required");
        }
    }

    public class GetPlayerByIdQueryHandler : IRequestHandler<GetPlayerByIdQuery, Result<PlayerDetailDto>>
    {
        private readonly IPlayerService _playerService;
        private readonly IMemoryCache _cache;

        public GetPlayerByIdQueryHandler(IPlayerService playerService, IMemoryCache cache)
        {
            _playerService = playerService;
            _cache = cache;
        }

        public async Task<Result<PlayerDetailDto>> Handle(GetPlayerByIdQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"player_{request.Id}";

            if (_cache.TryGetValue(cacheKey, out PlayerDetailDto? cachedResult) && cachedResult != null)
            {
                return Result<PlayerDetailDto>.Success(cachedResult, StatusCodes.Status200OK);
            }

            var player = await _playerService.GetPlayerByIdAsync(request.Id);

            if (player == null)
            {
                return Result<PlayerDetailDto>.Failure(
                    Error.NotFound("Player.NotFound", $"Player with ID {request.Id} was not found"),
                    StatusCodes.Status404NotFound);
            }

            _cache.Set(cacheKey, player, TimeSpan.FromMinutes(10));

            return Result<PlayerDetailDto>.Success(player, StatusCodes.Status200OK);
        }
    }
}
