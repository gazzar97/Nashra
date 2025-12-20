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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("competitions");
            
            modelBuilder.Entity<League>().HasKey(x => x.Id);
            modelBuilder.Entity<Team>().HasKey(x => x.Id);
        }
    }
}
