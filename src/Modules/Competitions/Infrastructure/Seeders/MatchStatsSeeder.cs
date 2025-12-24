using SportsData.Modules.Competitions.Domain;

namespace SportsData.Modules.Competitions.Infrastructure.Seeders
{
    public class MatchStatsSeeder
    {
        private readonly CompetitionsDbContext _context;

        public MatchStatsSeeder(CompetitionsDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            if (_context.MatchStats.Any())
            {
                return; // Already seeded
            }

            // Get existing finished matches
            var finishedMatches = _context.Matches
                .Where(m => m.Status == MatchStatus.Finished)
                .ToList();

            if (!finishedMatches.Any())
            {
                return; // No finished matches to seed stats for
            }

            var matchStatsList = new List<MatchStats>();

            // Seed stats for each finished match
            foreach (var match in finishedMatches)
            {
                // Generate realistic stats based on the match score
                var homeScore = match.HomeScore ?? 0;
                var awayScore = match.AwayScore ?? 0;

                // Calculate possession based on score (team with more goals typically has more possession)
                var homePossession = 50m + (homeScore - awayScore) * 5m;
                homePossession = Math.Max(30m, Math.Min(70m, homePossession)); // Clamp between 30-70
                var awayPossession = 100m - homePossession;

                // Home team stats
                var homeStats = MatchStats.Create(
                    matchId: match.Id,
                    teamId: match.HomeTeamId,
                    isHome: true,
                    possessionPercentage: homePossession,
                    shots: 8 + homeScore * 2 + Random.Shared.Next(0, 5),
                    shotsOnTarget: 3 + homeScore + Random.Shared.Next(0, 3),
                    corners: 4 + Random.Shared.Next(0, 6),
                    yellowCards: Random.Shared.Next(0, 4),
                    redCards: Random.Shared.Next(0, 2) == 1 ? 1 : 0,
                    fouls: 8 + Random.Shared.Next(0, 8)
                );

                // Away team stats
                var awayStats = MatchStats.Create(
                    matchId: match.Id,
                    teamId: match.AwayTeamId,
                    isHome: false,
                    possessionPercentage: awayPossession,
                    shots: 8 + awayScore * 2 + Random.Shared.Next(0, 5),
                    shotsOnTarget: 3 + awayScore + Random.Shared.Next(0, 3),
                    corners: 4 + Random.Shared.Next(0, 6),
                    yellowCards: Random.Shared.Next(0, 4),
                    redCards: Random.Shared.Next(0, 2) == 1 ? 1 : 0,
                    fouls: 8 + Random.Shared.Next(0, 8)
                );

                matchStatsList.Add(homeStats);
                matchStatsList.Add(awayStats);
            }

            await _context.MatchStats.AddRangeAsync(matchStatsList);
            await _context.SaveChangesAsync();
        }
    }
}
