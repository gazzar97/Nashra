using Azure.Core;
using Microsoft.EntityFrameworkCore;
using SportsData.Modules.Competitions.Application.Leagues.GetLeagues;
using SportsData.Modules.Competitions.Application.Leagues.Queries;
using SportsData.Modules.Competitions.Application.Leagues.Services;
using SportsData.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SportsData.Modules.Competitions.Infrastructure.Leagues
{
    public class LeagueService : ILeagueService
    {
        private readonly CompetitionsDbContext _dbContext;

        public LeagueService(CompetitionsDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<PagedList<LeagueDto>> GetLeagues(string Country,int Page, int PageSize)
        {
            var query = _dbContext.Leagues.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(Country))
            {
                query = query.Where(l => l.Country == Country);
            }

            var projectedQuery = query.Select(l => new LeagueDto(l.Id, l.Name, l.Country, l.LogoUrl));

            var pagedResult = await projectedQuery.ToPagedListAsync(Page, PageSize);

            return pagedResult;
        }

        public async Task<List<SeasonDto>> GetSeasons(Guid leagueId)
        {
            var seasons = await _dbContext.Seasons
                .AsNoTracking()
                .Where(s => s.LeagueId == leagueId)
                .OrderByDescending(s => s.Year)
                .ToListAsync();

            return seasons.Select(s => new SeasonDto(s.Id, s.Year, s.IsCurrent)).ToList();
        }
    }
}
