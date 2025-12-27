using Microsoft.EntityFrameworkCore;
using SportsData.Modules.Competitions.Domain;

namespace SportsData.Modules.Competitions.Infrastructure
{
    public class CompetitionsDbContext : DbContext
    {
        public const string Competitions = "competitions_";

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
            
            modelBuilder.Entity<League>().ToTable($"{Competitions}leagues").HasKey(x => x.Id);
            modelBuilder.Entity<Team>().ToTable($"{Competitions}team").HasKey(x => x.Id);
            modelBuilder.Entity<Season>().ToTable($"{Competitions}season").HasKey(x => x.Id);
            modelBuilder.Entity<Player>().ToTable($"{Competitions}player").HasKey(x => x.Id);
            modelBuilder.Entity<LeagueTeamSeason>().ToTable($"{Competitions}leagueTeamSeason").HasKey(x => x.Id);
            modelBuilder.Entity<TeamPlayerSeason>().ToTable($"{Competitions}teamPlayerSeason").HasKey(x => x.Id);
        }
    }
}
