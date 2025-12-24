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
        public DbSet<Match> Matches { get; set; }
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
            
            // Match entity configuration
            modelBuilder.Entity<Match>().HasKey(x => x.Id);
            
            // Performance indexes for Match queries
            modelBuilder.Entity<Match>()
                .HasIndex(m => new { m.SeasonId, m.MatchDate })
                .HasDatabaseName("IX_Match_SeasonId_MatchDate");
            
            modelBuilder.Entity<Match>()
                .HasIndex(m => m.HomeTeamId)
                .HasDatabaseName("IX_Match_HomeTeamId");
            
            modelBuilder.Entity<Match>()
                .HasIndex(m => m.AwayTeamId)
                .HasDatabaseName("IX_Match_AwayTeamId");
            
            modelBuilder.Entity<Match>()
                .HasIndex(m => m.Status)
                .HasDatabaseName("IX_Match_Status");
        }
    }
}
