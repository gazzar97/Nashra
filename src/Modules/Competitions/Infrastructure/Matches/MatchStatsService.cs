using Microsoft.EntityFrameworkCore;
using SportsData.Modules.Competitions.Application.Matches.Services;
using SportsData.Modules.Competitions.Domain;

namespace SportsData.Modules.Competitions.Infrastructure.Matches
{
    public class MatchStatsService : IMatchStatsService
    {
        private readonly CompetitionsDbContext _dbContext;

        public MatchStatsService(CompetitionsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<MatchStatsDto?> GetMatchStatsAsync(Guid matchId)
        {
            // Verify match exists
            var matchExists = await _dbContext.Matches
                .AsNoTracking()
                .AnyAsync(m => m.Id == matchId);

            if (!matchExists)
            {
                return null; // Match doesn't exist - will result in 404
            }

            // Query both home and away stats for the match
            var stats = await _dbContext.MatchStats
                .AsNoTracking()
                .Where(ms => ms.MatchId == matchId)
                .OrderBy(ms => ms.IsHome ? 0 : 1) // Home first, then away
                .ToListAsync();

            // If no stats found, return null (will result in 204 No Content)
            if (stats.Count == 0)
            {
                return null;
            }

            // Separate home and away stats
            var homeStats = stats.FirstOrDefault(s => s.IsHome);
            var awayStats = stats.FirstOrDefault(s => !s.IsHome);

            // Both should exist, but handle edge cases
            if (homeStats == null || awayStats == null)
            {
                return null;
            }

            // Map to DTO
            return new MatchStatsDto(
                matchId,
                new TeamStatsDto(
                    homeStats.TeamId,
                    homeStats.PossessionPercentage,
                    homeStats.Shots,
                    homeStats.ShotsOnTarget,
                    homeStats.Corners,
                    homeStats.YellowCards,
                    homeStats.RedCards,
                    homeStats.Fouls
                ),
                new TeamStatsDto(
                    awayStats.TeamId,
                    awayStats.PossessionPercentage,
                    awayStats.Shots,
                    awayStats.ShotsOnTarget,
                    awayStats.Corners,
                    awayStats.YellowCards,
                    awayStats.RedCards,
                    awayStats.Fouls
                )
            );
        }
    }
}
