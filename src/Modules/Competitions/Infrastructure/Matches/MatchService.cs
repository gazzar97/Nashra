using Microsoft.EntityFrameworkCore;
using SportsData.Modules.Competitions.Application.Matches.Services;
using SportsData.Modules.Competitions.Domain;
using SportsData.Shared;

namespace SportsData.Modules.Competitions.Infrastructure.Matches
{
    public class MatchService : IMatchService
    {
        private readonly CompetitionsDbContext _dbContext;

        public MatchService(CompetitionsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedList<MatchDto>> GetMatchesAsync(
            Guid seasonId,
            Guid? teamId,
            DateTime? fromDate,
            DateTime? toDate,
            int page,
            int pageSize)
        {
            // Start with base query filtered by season
            var query = _dbContext.Matches
                .AsNoTracking()
                .Where(m => m.SeasonId == seasonId);

            // Apply team filter (match if home OR away team)
            if (teamId.HasValue)
            {
                query = query.Where(m => m.HomeTeamId == teamId.Value || m.AwayTeamId == teamId.Value);
            }

            // Apply date range filters
            if (fromDate.HasValue)
            {
                query = query.Where(m => m.MatchDate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(m => m.MatchDate <= toDate.Value);
            }

            // Project to DTO with team and season details
            var projectedQuery = query
                .OrderBy(m => m.MatchDate)
                .Select(m => new MatchDto(
                    m.Id,
                    _dbContext.Seasons
                        .Where(s => s.Id == m.SeasonId)
                        .Select(s => s.Year)
                        .FirstOrDefault() ?? string.Empty,
                    m.MatchDate,
                    m.Status.ToString(), // Enum to string conversion
                    new TeamBasicDto(
                        _dbContext.Teams
                            .Where(t => t.Id == m.HomeTeamId)
                            .Select(t => t.Id)
                            .FirstOrDefault(),
                        _dbContext.Teams
                            .Where(t => t.Id == m.HomeTeamId)
                            .Select(t => t.Name)
                            .FirstOrDefault() ?? string.Empty,
                        _dbContext.Teams
                            .Where(t => t.Id == m.HomeTeamId)
                            .Select(t => t.LogoUrl)
                            .FirstOrDefault() ?? string.Empty
                    ),
                    new TeamBasicDto(
                        _dbContext.Teams
                            .Where(t => t.Id == m.AwayTeamId)
                            .Select(t => t.Id)
                            .FirstOrDefault(),
                        _dbContext.Teams
                            .Where(t => t.Id == m.AwayTeamId)
                            .Select(t => t.Name)
                            .FirstOrDefault() ?? string.Empty,
                        _dbContext.Teams
                            .Where(t => t.Id == m.AwayTeamId)
                            .Select(t => t.LogoUrl)
                            .FirstOrDefault() ?? string.Empty
                    ),
                    m.HomeScore.HasValue && m.AwayScore.HasValue
                        ? new ScoreDto(m.HomeScore.Value, m.AwayScore.Value)
                        : null,
                    m.Venue
                ));

            return await projectedQuery.ToPagedListAsync(page, pageSize);
        }
    }
}
