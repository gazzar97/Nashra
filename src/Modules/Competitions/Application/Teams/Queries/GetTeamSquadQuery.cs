using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SportsData.Modules.Competitions.Application.Players.Services;
using SportsData.Modules.Competitions.Infrastructure;
using SportsData.Shared;

namespace SportsData.Modules.Competitions.Application.Teams.Queries
{
    public record GetTeamSquadQuery(
        Guid TeamId,
        Guid SeasonId,
        int Page = 1,
        int PageSize = 50) : IRequest<Result<PagedList<SquadPlayerDto>>>;

    public record SquadPlayerDto(
        Guid PlayerId,
        string Name,
        string Position,
        string Nationality,
        int? ShirtNumber,
        DateTime? DateOfBirth,
        int? Height,
        int? Weight);

    public class GetTeamSquadQueryValidator : AbstractValidator<GetTeamSquadQuery>
    {
        public GetTeamSquadQueryValidator()
        {
            RuleFor(x => x.TeamId)
                .NotEmpty()
                .WithMessage("Team ID is required");

            RuleFor(x => x.SeasonId)
                .NotEmpty()
                .WithMessage("Season ID is required");

            RuleFor(x => x.Page)
                .GreaterThan(0)
                .WithMessage("Page must be greater than 0");

            RuleFor(x => x.PageSize)
                .GreaterThan(0)
                .LessThanOrEqualTo(100)
                .WithMessage("PageSize must be between 1 and 100");
        }
    }

    public class GetTeamSquadQueryHandler : IRequestHandler<GetTeamSquadQuery, Result<PagedList<SquadPlayerDto>>>
    {
        private readonly CompetitionsDbContext _dbContext;
        private readonly IMemoryCache _cache;

        public GetTeamSquadQueryHandler(CompetitionsDbContext dbContext, IMemoryCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        public async Task<Result<PagedList<SquadPlayerDto>>> Handle(GetTeamSquadQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"squad_{request.TeamId}_{request.SeasonId}_{request.Page}_{request.PageSize}";

            if (_cache.TryGetValue(cacheKey, out PagedList<SquadPlayerDto>? cachedResult) && cachedResult != null)
            {
                return Result<PagedList<SquadPlayerDto>>.Success(cachedResult, StatusCodes.Status200OK);
            }

            // Verify team exists
            var teamExists = await _dbContext.Teams.AnyAsync(t => t.Id == request.TeamId, cancellationToken);
            if (!teamExists)
            {
                return Result<PagedList<SquadPlayerDto>>.Failure(
                    Error.NotFound("Team.NotFound", $"Team with ID {request.TeamId} was not found"),
                    StatusCodes.Status404NotFound);
            }

            // Verify season exists
            var seasonExists = await _dbContext.Seasons.AnyAsync(s => s.Id == request.SeasonId, cancellationToken);
            if (!seasonExists)
            {
                return Result<PagedList<SquadPlayerDto>>.Failure(
                    Error.NotFound("Season.NotFound", $"Season with ID {request.SeasonId} was not found"),
                    StatusCodes.Status404NotFound);
            }

            var query = from player in _dbContext.Players.AsNoTracking()
                        join tps in _dbContext.TeamPlayerSeasons on player.Id equals tps.PlayerId
                        where tps.TeamId == request.TeamId && tps.SeasonId == request.SeasonId
                        orderby tps.ShirtNumber
                        select new SquadPlayerDto(
                            player.Id,
                            player.Name,
                            player.Position,
                            player.Nationality,
                            tps.ShirtNumber,
                            player.DateOfBirth,
                            player.Height,
                            player.Weight);

            var squad = await query.ToPagedListAsync(request.Page, request.PageSize);

            _cache.Set(cacheKey, squad, TimeSpan.FromMinutes(10));

            return Result<PagedList<SquadPlayerDto>>.Success(squad, StatusCodes.Status200OK);
        }
    }
}
