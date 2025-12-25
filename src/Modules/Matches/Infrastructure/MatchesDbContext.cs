using Microsoft.EntityFrameworkCore;
using SportsData.Modules.Matches.Domain;

namespace SportsData.Modules.Matches.Infrastructure
{
    public class MatchesDbContext : DbContext
    {
        public MatchesDbContext(DbContextOptions<MatchesDbContext> options) : base(options)
        {
        }

        public DbSet<Match> Matches { get; set; }
        public DbSet<MatchStatistics> MatchStatistics { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("matches");
            
            modelBuilder.Entity<Match>().HasKey(x => x.Id);
            modelBuilder.Entity<MatchStatistics>().HasKey(x => x.Id);

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

            // Performance index for statistics queries
            modelBuilder.Entity<MatchStatistics>()
                .HasIndex(s => s.MatchId)
                .HasDatabaseName("IX_MatchStatistics_MatchId");
        }
    }
}
