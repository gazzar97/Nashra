using SportsData.Shared;

namespace SportsData.Modules.ApiKeys.Domain
{
    public class ApiKey : Entity
    {
        public string KeyHash { get; private set; }
        public string Name { get; private set; }
        public Guid OwnerId { get; private set; }
        public ApiKeyPlan Plan { get; private set; }
        public bool IsActive { get; private set; }
        public int RateLimitPerMinute { get; private set; }
        public int RateLimitPerDay { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? ExpiresAt { get; private set; }
        public DateTime? LastUsedAt { get; private set; }
        public DateTime? RevokedAt { get; private set; }

        private ApiKey() { } // EF Core

        private ApiKey(
            string keyHash,
            string name,
            Guid ownerId,
            ApiKeyPlan plan,
            int rateLimitPerMinute,
            int rateLimitPerDay,
            DateTime? expiresAt)
        {
            KeyHash = keyHash;
            Name = name;
            OwnerId = ownerId;
            Plan = plan;
            IsActive = true;
            RateLimitPerMinute = rateLimitPerMinute;
            RateLimitPerDay = rateLimitPerDay;
            CreatedAt = DateTime.UtcNow;
            ExpiresAt = expiresAt;
        }

        public static ApiKey Create(
            string keyHash,
            string name,
            Guid ownerId,
            ApiKeyPlan plan,
            int rateLimitPerMinute,
            int rateLimitPerDay,
            DateTime? expiresAt = null)
        {
            if (string.IsNullOrWhiteSpace(keyHash))
                throw new ArgumentException("Key hash cannot be empty", nameof(keyHash));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty", nameof(name));

            if (ownerId == Guid.Empty)
                throw new ArgumentException("Owner ID cannot be empty", nameof(ownerId));

            if (rateLimitPerMinute <= 0)
                throw new ArgumentException("Rate limit per minute must be positive", nameof(rateLimitPerMinute));

            if (rateLimitPerDay <= 0)
                throw new ArgumentException("Rate limit per day must be positive", nameof(rateLimitPerDay));

            if (expiresAt.HasValue && expiresAt.Value <= DateTime.UtcNow)
                throw new ArgumentException("Expiration date must be in the future", nameof(expiresAt));

            return new ApiKey(keyHash, name, ownerId, plan, rateLimitPerMinute, rateLimitPerDay, expiresAt);
        }

        public void Revoke()
        {
            RevokedAt = DateTime.UtcNow;
            IsActive = false;
        }

        public void Activate()
        {
            if (RevokedAt.HasValue)
                throw new InvalidOperationException("Cannot activate a revoked key");

            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void UpdateLastUsed()
        {
            LastUsedAt = DateTime.UtcNow;
        }

        public bool IsValid()
        {
            if (!IsActive) return false;
            if (RevokedAt.HasValue) return false;
            if (ExpiresAt.HasValue && ExpiresAt.Value <= DateTime.UtcNow) return false;

            return true;
        }
    }
}
