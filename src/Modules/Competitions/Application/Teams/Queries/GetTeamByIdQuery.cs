using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SportsData.Modules.Competitions.Infrastructure;
using SportsData.Shared;

namespace SportsData.Modules.Competitions.Application.Teams.Queries
{
    public record GetTeamByIdQuery(Guid Id) : IRequest<Result<TeamDetailDto>>;

    public record TeamDetailDto(
        Guid Id,
        string Name,
        string ShortName,
        string Code,
        string LogoUrl,
        int? FoundedYear,
        string Stadium);

    public class GetTeamByIdQueryValidator : AbstractValidator<GetTeamByIdQuery>
    {
        public GetTeamByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Team ID is required");
        }
    }

    public class GetTeamByIdQueryHandler : IRequestHandler<GetTeamByIdQuery, Result<TeamDetailDto>>
    {
        private readonly CompetitionsDbContext _dbContext;
        private readonly IMemoryCache _cache;

        public GetTeamByIdQueryHandler(CompetitionsDbContext dbContext, IMemoryCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        public async Task<Result<TeamDetailDto>> Handle(GetTeamByIdQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"team_{request.Id}";

            if (_cache.TryGetValue(cacheKey, out TeamDetailDto? cachedResult) && cachedResult != null)
            {
                return Result<TeamDetailDto>.Success(cachedResult, StatusCodes.Status200OK);
            }

            var team = await _dbContext.Teams
                .AsNoTracking()
                .Where(t => t.Id == request.Id)
                .Select(t => new TeamDetailDto(
                    t.Id,
                    t.Name,
                    t.ShortName,
                    t.Code,
                    t.LogoUrl,
                    t.FoundedYear,
                    t.Stadium))
                .FirstOrDefaultAsync(cancellationToken);

            if (team == null)
            {
                return Result<TeamDetailDto>.Failure(
                    Error.NotFound("Team.NotFound", $"Team with ID {request.Id} was not found"),
                    StatusCodes.Status404NotFound);
            }

            _cache.Set(cacheKey, team, TimeSpan.FromMinutes(10));

            return Result<TeamDetailDto>.Success(team, StatusCodes.Status200OK);
        }
    }
}
