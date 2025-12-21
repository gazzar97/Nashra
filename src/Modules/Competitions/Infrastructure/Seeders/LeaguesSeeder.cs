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
            }
        }
    }
}
