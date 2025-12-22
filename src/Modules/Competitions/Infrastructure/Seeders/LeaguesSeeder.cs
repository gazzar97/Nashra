using SportsData.Modules.Competitions.Domain;

namespace SportsData.Modules.Competitions.Infrastructure.Seeders
{
    public class LeaguesSeeder
    {
        private readonly CompetitionsDbContext _dbContext;

        public LeaguesSeeder(CompetitionsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SeedAsync()
        {
            if (!_dbContext.Leagues.Any())
            {
                var leagues = new List<League>
                {
                    League.Create("Egyptian Premier League", "Egypt", "https://upload.wikimedia.org/wikipedia/en/thumb/f/f6/Egyptian_Premier_League_logo.png/1200px-Egyptian_Premier_League_logo.png", 1, "August", "May"),
                    League.Create("Egyptian Second Division A", "Egypt", "", 2, "September", "June")
                };

                _dbContext.Leagues.AddRange(leagues);
                
                await _dbContext.SaveChangesAsync();

                var seasons = new List<Season>();
                
                // Seed seasons for Egyptian Premier League
                var plLeague = leagues.First(l => l.Name == "Egyptian Premier League");
                seasons.Add(new Season(plLeague.Id, "2023/2024", true));
                seasons.Add(new Season(plLeague.Id, "2022/2023", false));
                seasons.Add(new Season(plLeague.Id, "2021/2022", false));

                // Seed seasons for Second Division
                var sdLeague = leagues.First(l => l.Name == "Egyptian Second Division A");
                seasons.Add(new Season(sdLeague.Id, "2023/2024", true));

                _dbContext.Seasons.AddRange(seasons);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
