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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("matches");
            
            modelBuilder.Entity<Match>().HasKey(x => x.Id);
        }
    }
}
