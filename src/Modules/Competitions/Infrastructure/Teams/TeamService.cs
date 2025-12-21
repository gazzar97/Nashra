using Microsoft.EntityFrameworkCore;
using SportsData.Modules.Competitions.Application.Teams.Services;
using SportsData.Shared;

namespace SportsData.Modules.Competitions.Infrastructure.Teams
{
    public class TeamService : ITeamService
    {
        private readonly CompetitionsDbContext _dbContext;

        public TeamService(CompetitionsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedList<TeamDto>> GetTeamsAsync(Guid? leagueId, Guid? seasonId, int page, int pageSize)
        {
            var query = _dbContext.Teams.AsNoTracking().AsQueryable();

            if (leagueId.HasValue || seasonId.HasValue)
            {
                // We need to join with LeagueTeamSeasons to filter
                var ltsQuery = _dbContext.LeagueTeamSeasons.AsNoTracking().AsQueryable();

                if (leagueId.HasValue)
                {
                    ltsQuery = ltsQuery.Where(x => x.LeagueId == leagueId.Value);
                }

                if (seasonId.HasValue)
                {
                    ltsQuery = ltsQuery.Where(x => x.SeasonId == seasonId.Value);
                }

                var teamIds = ltsQuery.Select(x => x.TeamId).Distinct();
                query = query.Where(t => teamIds.Contains(t.Id));
            }

            var projectedQuery = query.Select(t => new TeamDto(t.Id, t.Name, t.ShortName, t.Code, t.LogoUrl, t.FoundedYear, t.Stadium));

            return await projectedQuery.ToPagedListAsync(page, pageSize);
        }
    }
}
