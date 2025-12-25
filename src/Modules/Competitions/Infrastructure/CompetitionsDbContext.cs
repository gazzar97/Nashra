using Microsoft.EntityFrameworkCore;
using SportsData.Modules.Competitions.Domain;

namespace SportsData.Modules.Competitions.Infrastructure
{
    public class CompetitionsDbContext : DbContext
    {
        public CompetitionsDbContext(DbContextOptions<CompetitionsDbContext> options) : base(options)
        {
        }

        public DbSet<League> Leagues { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Season> Seasons { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<LeagueTeamSeason> LeagueTeamSeasons { get; set; }
        public DbSet<TeamPlayerSeason> TeamPlayerSeasons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("competitions");
            
            modelBuilder.Entity<League>().HasKey(x => x.Id);
            modelBuilder.Entity<Team>().HasKey(x => x.Id);
            modelBuilder.Entity<Season>().HasKey(x => x.Id);
            modelBuilder.Entity<Player>().HasKey(x => x.Id);
            modelBuilder.Entity<LeagueTeamSeason>().HasKey(x => x.Id);
            modelBuilder.Entity<TeamPlayerSeason>().HasKey(x => x.Id);
        }
    }
}
