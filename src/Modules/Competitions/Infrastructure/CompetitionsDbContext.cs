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
        public DbSet<LeagueStanding> LeagueStandings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Table configurations
            modelBuilder.Entity<League>().ToTable($"{Competitions}leagues").HasKey(x => x.Id);
            modelBuilder.Entity<Team>().ToTable($"{Competitions}team").HasKey(x => x.Id);
            modelBuilder.Entity<Season>().ToTable($"{Competitions}season").HasKey(x => x.Id);
            modelBuilder.Entity<Player>().ToTable($"{Competitions}player").HasKey(x => x.Id);
            modelBuilder.Entity<LeagueTeamSeason>().ToTable($"{Competitions}leagueTeamSeason").HasKey(x => x.Id);
            modelBuilder.Entity<TeamPlayerSeason>().ToTable($"{Competitions}teamPlayerSeason").HasKey(x => x.Id);
            modelBuilder.Entity<LeagueStanding>().ToTable($"{Competitions}leagueStanding").HasKey(x => x.Id);

            // Indexes for Season
            modelBuilder.Entity<Season>()
                .HasIndex(s => s.LeagueId)
                .HasDatabaseName("IX_Season_LeagueId");

            // Indexes for LeagueTeamSeason
            modelBuilder.Entity<LeagueTeamSeason>()
                .HasIndex(lts => lts.LeagueId)
                .HasDatabaseName("IX_LeagueTeamSeason_LeagueId");

            modelBuilder.Entity<LeagueTeamSeason>()
                .HasIndex(lts => lts.TeamId)
                .HasDatabaseName("IX_LeagueTeamSeason_TeamId");

            modelBuilder.Entity<LeagueTeamSeason>()
                .HasIndex(lts => lts.SeasonId)
                .HasDatabaseName("IX_LeagueTeamSeason_SeasonId");

            modelBuilder.Entity<LeagueTeamSeason>()
                .HasIndex(lts => new { lts.LeagueId, lts.TeamId, lts.SeasonId })
                .IsUnique()
                .HasDatabaseName("IX_LeagueTeamSeason_Unique");

            // Indexes for TeamPlayerSeason
            modelBuilder.Entity<TeamPlayerSeason>()
                .HasIndex(tps => tps.TeamId)
                .HasDatabaseName("IX_TeamPlayerSeason_TeamId");

            modelBuilder.Entity<TeamPlayerSeason>()
                .HasIndex(tps => tps.PlayerId)
                .HasDatabaseName("IX_TeamPlayerSeason_PlayerId");

            modelBuilder.Entity<TeamPlayerSeason>()
                .HasIndex(tps => tps.SeasonId)
                .HasDatabaseName("IX_TeamPlayerSeason_SeasonId");

            modelBuilder.Entity<TeamPlayerSeason>()
                .HasIndex(tps => new { tps.TeamId, tps.PlayerId, tps.SeasonId })
                .HasDatabaseName("IX_TeamPlayerSeason_Composite");

            // Indexes for LeagueStanding
            modelBuilder.Entity<LeagueStanding>()
                .HasIndex(ls => ls.LeagueId)
                .HasDatabaseName("IX_LeagueStanding_LeagueId");

            modelBuilder.Entity<LeagueStanding>()
                .HasIndex(ls => ls.SeasonId)
                .HasDatabaseName("IX_LeagueStanding_SeasonId");

            modelBuilder.Entity<LeagueStanding>()
                .HasIndex(ls => ls.TeamId)
                .HasDatabaseName("IX_LeagueStanding_TeamId");

            modelBuilder.Entity<LeagueStanding>()
                .HasIndex(ls => new { ls.LeagueId, ls.SeasonId, ls.TeamId })
                .IsUnique()
                .HasDatabaseName("IX_LeagueStanding_Unique");

            modelBuilder.Entity<LeagueStanding>()
                .HasIndex(ls => new { ls.LeagueId, ls.SeasonId, ls.Position })
                .HasDatabaseName("IX_LeagueStanding_Position");
        }
    }
}
