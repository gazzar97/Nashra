using SportsData.Modules.Competitions.Domain;

namespace SportsData.Modules.Competitions.Infrastructure.Seeders
{
    public class TeamsSeeder
    {
        private readonly CompetitionsDbContext _dbContext;

        public TeamsSeeder(CompetitionsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SeedAsync()
        {
            if (!_dbContext.Teams.Any())
            {
                var ahly = new Team("Al Ahly SC", "Al Ahly", "AHL", "https://upload.wikimedia.org/wikipedia/en/thumb/a/a2/Al_Ahly_SC_logo.png/180px-Al_Ahly_SC_logo.png", 1907, "Cairo International Stadium");
                var zamalek = new Team("Zamalek SC", "Zamalek", "ZAM", "https://upload.wikimedia.org/wikipedia/en/thumb/0/04/ZamalekSC.png/180px-ZamalekSC.png", 1911, "Cairo International Stadium");
                var pyramids = new Team("Pyramids FC", "Pyramids", "PYR", "https://upload.wikimedia.org/wikipedia/en/thumb/4/41/Pyramids_FC_logo.png/180px-Pyramids_FC_logo.png", 2008, "30 June Stadium");
                var masry = new Team("Al Masry SC", "Al Masry", "MAS", "https://upload.wikimedia.org/wikipedia/en/thumb/1/14/Al_Masry_SC_logo.png/180px-Al_Masry_SC_logo.png", 1920, "Port Said Stadium");

                var teams = new List<Team> { ahly, zamalek, pyramids, masry };

                _dbContext.Teams.AddRange(teams);
                await _dbContext.SaveChangesAsync();

                // Assign to Egyptian Premier League for 2023/2024 Season
                var league = _dbContext.Leagues.FirstOrDefault(l => l.Name == "Egyptian Premier League");
                if (league != null)
                {
                    var season = new Season(league.Id, "2023/2024", true);
                    _dbContext.Seasons.Add(season);
                    await _dbContext.SaveChangesAsync();

                    var leagueTeamSeasons = new List<LeagueTeamSeason>
                    {
                        new LeagueTeamSeason(league.Id, ahly.Id, season.Id),
                        new LeagueTeamSeason(league.Id, zamalek.Id, season.Id),
                        new LeagueTeamSeason(league.Id, pyramids.Id, season.Id),
                        new LeagueTeamSeason(league.Id, masry.Id, season.Id)
                    };

                    _dbContext.LeagueTeamSeasons.AddRange(leagueTeamSeasons);
                    await _dbContext.SaveChangesAsync();
                }
            }
        }
    }
}
