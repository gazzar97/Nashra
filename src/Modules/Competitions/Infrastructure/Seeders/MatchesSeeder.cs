using SportsData.Modules.Competitions.Domain;

namespace SportsData.Modules.Competitions.Infrastructure.Seeders
{
    public class MatchesSeeder
    {
        private readonly CompetitionsDbContext _context;

        public MatchesSeeder(CompetitionsDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            if (_context.Matches.Any())
            {
                return; // Already seeded
            }

            // Get existing seasons and teams for Egyptian Premier League
            var egyptianLeague = _context.Leagues.FirstOrDefault(l => l.Name == "Egyptian Premier League");
            if (egyptianLeague == null) return;

            var currentSeason = _context.Seasons
                .FirstOrDefault(s => s.LeagueId == egyptianLeague.Id && s.IsCurrent);
            if (currentSeason == null) return;

            // Get teams
            var alAhly = _context.Teams.FirstOrDefault(t => t.Code == "AHL");
            var zamalek = _context.Teams.FirstOrDefault(t => t.Code == "ZAM");
            var pyramidsFC = _context.Teams.FirstOrDefault(t => t.Code == "PYR");
            var alMasry = _context.Teams.FirstOrDefault(t => t.Code == "MAS");
            var ismaily = _context.Teams.FirstOrDefault(t => t.Code == "ISM");
            var futureFC = _context.Teams.FirstOrDefault(t => t.Code == "FUT");

            if (alAhly == null || zamalek == null || pyramidsFC == null) return;

            var matches = new List<Match>();

            // Classic Cairo Derby - Finished
            if (alAhly != null && zamalek != null)
            {
                var match1 = Match.Create(
                    currentSeason.Id,
                    alAhly.Id,
                    zamalek.Id,
                    new DateTime(2024, 9, 15, 20, 0, 0),
                    "Cairo International Stadium"
                );
                match1.SetFinalScore(2, 1);
                matches.Add(match1);
            }

            // Al Ahly vs Pyramids FC - Finished
            if (alAhly != null && pyramidsFC != null)
            {
                var match2 = Match.Create(
                    currentSeason.Id,
                    pyramidsFC.Id,
                    alAhly.Id,
                    new DateTime(2024, 9, 22, 19, 30, 0),
                    "30 June Stadium"
                );
                match2.SetFinalScore(1, 3);
                matches.Add(match2);
            }

            // Zamalek vs Ismaily - Finished
            if (zamalek != null && ismaily != null)
            {
                var match3 = Match.Create(
                    currentSeason.Id,
                    zamalek.Id,
                    ismaily.Id,
                    new DateTime(2024, 10, 5, 18, 0, 0),
                    "Cairo International Stadium"
                );
                match3.SetFinalScore(2, 0);
                matches.Add(match3);
            }

            // Al Masry vs Future FC - Finished
            if (alMasry != null && futureFC != null)
            {
                var match4 = Match.Create(
                    currentSeason.Id,
                    alMasry.Id,
                    futureFC.Id,
                    new DateTime(2024, 10, 12, 17, 0, 0),
                    "Al Masry Club Stadium"
                );
                match4.SetFinalScore(1, 1);
                matches.Add(match4);
            }

            // Pyramids FC vs Zamalek - Finished
            if (pyramidsFC != null && zamalek != null)
            {
                var match5 = Match.Create(
                    currentSeason.Id,
                    pyramidsFC.Id,
                    zamalek.Id,
                    new DateTime(2024, 10, 20, 20, 0, 0),
                    "30 June Stadium"
                );
                match5.SetFinalScore(0, 2);
                matches.Add(match5);
            }

            // Al Ahly vs Ismaily - Scheduled (upcoming)
            if (alAhly != null && ismaily != null)
            {
                var match6 = Match.Create(
                    currentSeason.Id,
                    alAhly.Id,
                    ismaily.Id,
                    new DateTime(2024, 12, 28, 19, 0, 0),
                    "Al Ahly WE Al Salam Stadium"
                );
                matches.Add(match6);
            }

            // Zamalek vs Future FC - Scheduled
            if (zamalek != null && futureFC != null)
            {
                var match7 = Match.Create(
                    currentSeason.Id,
                    zamalek.Id,
                    futureFC.Id,
                    new DateTime(2025, 1, 3, 18, 30, 0),
                    "Cairo International Stadium"
                );
                matches.Add(match7);
            }

            // Ismaily vs Pyramids FC - Scheduled
            if (ismaily != null && pyramidsFC != null)
            {
                var match8 = Match.Create(
                    currentSeason.Id,
                    ismaily.Id,
                    pyramidsFC.Id,
                    new DateTime(2025, 1, 10, 17, 0, 0),
                    "Ismailia Stadium"
                );
                matches.Add(match8);
            }

            // Al Masry vs Al Ahly - Scheduled
            if (alMasry != null && alAhly != null)
            {
                var match9 = Match.Create(
                    currentSeason.Id,
                    alMasry.Id,
                    alAhly.Id,
                    new DateTime(2025, 1, 17, 20, 0, 0),
                    "Al Masry Club Stadium"
                );
                matches.Add(match9);
            }

            // Future FC vs Al Ahly - Scheduled
            if (futureFC != null && alAhly != null)
            {
                var match10 = Match.Create(
                    currentSeason.Id,
                    futureFC.Id,
                    alAhly.Id,
                    new DateTime(2025, 1, 24, 19, 30, 0),
                    "Al Salam Stadium"
                );
                matches.Add(match10);
            }

            // Return fixture: Zamalek vs Al Ahly - Scheduled (big match)
            if (zamalek != null && alAhly != null)
            {
                var match11 = Match.Create(
                    currentSeason.Id,
                    zamalek.Id,
                    alAhly.Id,
                    new DateTime(2025, 2, 7, 20, 30, 0),
                    "Cairo International Stadium"
                );
                matches.Add(match11);
            }

            // Pyramids FC vs Al Masry - Scheduled
            if (pyramidsFC != null && alMasry != null)
            {
                var match12 = Match.Create(
                    currentSeason.Id,
                    pyramidsFC.Id,
                    alMasry.Id,
                    new DateTime(2025, 2, 14, 18, 0, 0),
                    "30 June Stadium"
                );
                matches.Add(match12);
            }

            await _context.Matches.AddRangeAsync(matches);
            await _context.SaveChangesAsync();
        }
    }
}
