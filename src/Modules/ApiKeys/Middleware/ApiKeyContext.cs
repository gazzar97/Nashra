using SportsData.Modules.ApiKeys.Domain;

namespace SportsData.Modules.ApiKeys.Middleware
{
    public class ApiKeyContext
    {
        public Guid ApiKeyId { get; set; }
        public Guid OwnerId { get; set; }
        public ApiKeyPlan Plan { get; set; }
        public int RateLimitPerMinute { get; set; }
        public int RateLimitPerDay { get; set; }
    }
}
