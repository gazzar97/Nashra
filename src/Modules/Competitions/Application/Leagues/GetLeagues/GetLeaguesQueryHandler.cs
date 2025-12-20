using MediatR;
using Microsoft.EntityFrameworkCore;
using SportsData.Modules.Competitions.Infrastructure;
using SportsData.Shared;

namespace SportsData.Modules.Competitions.Application.Leagues.GetLeagues
{
    public class GetLeaguesQueryHandler : IRequestHandler<GetLeaguesQuery, Result<List<LeagueDto>>>
    {
        private readonly CompetitionsDbContext _dbContext;

        public GetLeaguesQueryHandler(CompetitionsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<List<LeagueDto>>> Handle(GetLeaguesQuery request, CancellationToken cancellationToken)
        {
            var leagues = await _dbContext.Leagues
                .Select(l => new LeagueDto(l.Id, l.Name, l.Country, l.LogoUrl))
                .ToListAsync(cancellationToken);

            // If empty, we still return success 
            if (leagues.Count == 0)
            {
                // For MVP, if no data, let's return a hardcoded one so user sees something
                // In production, this would just be an empty list
                return Result<List<LeagueDto>>.Success(new List<LeagueDto>
                {
                    new(Guid.NewGuid(), "Egyptian Premier League", "Egypt", "")
                });
            }

            return Result<List<LeagueDto>>.Success(leagues);
        }
    }
}
