using Microsoft.EntityFrameworkCore;
using SportsData.Modules.ApiKeys.Domain;

namespace SportsData.Modules.ApiKeys.Infrastructure
{
    public class ApiKeysDbContext : DbContext
    {
        public const string DEFAULT_SCHEMA = "apikeys_";
        public ApiKeysDbContext(DbContextOptions<ApiKeysDbContext> options) : base(options)
        {
        }

        public DbSet<ApiKey> ApiKeys { get; set; }
        public DbSet<ApiUsageLog> ApiUsageLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApiKey>().ToTable($"{DEFAULT_SCHEMA}apikeys");
            modelBuilder.Entity<ApiUsageLog>().ToTable($"{DEFAULT_SCHEMA}apiUsageLog");

            // ApiKey configuration
            modelBuilder.Entity<ApiKey>(entity =>
            {
                entity.HasKey(x => x.Id);
                
                entity.Property(x => x.KeyHash)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(x => x.OwnerId)
                    .IsRequired();

                entity.Property(x => x.Plan)
                    .IsRequired()
                    .HasConversion<int>();

                entity.Property(x => x.IsActive)
                    .IsRequired();

                entity.Property(x => x.RateLimitPerMinute)
                    .IsRequired();

                entity.Property(x => x.RateLimitPerDay)
                    .IsRequired();

                entity.Property(x => x.CreatedAt)
                    .IsRequired();

                // Indexes
                entity.HasIndex(x => x.KeyHash)
                    .IsUnique()
                    .HasDatabaseName("IX_ApiKeys_KeyHash");

                entity.HasIndex(x => x.OwnerId)
                    .HasDatabaseName("IX_ApiKeys_OwnerId");

                entity.HasIndex(x => x.IsActive)
                    .HasDatabaseName("IX_ApiKeys_IsActive");
            });

            // ApiUsageLog configuration
            modelBuilder.Entity<ApiUsageLog>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.ApiKeyId)
                    .IsRequired();

                entity.Property(x => x.Endpoint)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(x => x.Method)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(x => x.StatusCode)
                    .IsRequired();

                entity.Property(x => x.ResponseTimeMs)
                    .IsRequired();

                entity.Property(x => x.Timestamp)
                    .IsRequired();

                // Indexes
                entity.HasIndex(x => x.ApiKeyId)
                    .HasDatabaseName("IX_ApiUsageLogs_ApiKeyId");

                entity.HasIndex(x => x.Timestamp)
                    .HasDatabaseName("IX_ApiUsageLogs_Timestamp");
            });
        }
    }
}
