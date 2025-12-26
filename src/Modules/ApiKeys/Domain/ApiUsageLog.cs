using SportsData.Shared;

namespace SportsData.Modules.ApiKeys.Domain
{
    public class ApiUsageLog : Entity
    {
        public Guid ApiKeyId { get; private set; }
        public string Endpoint { get; private set; }
        public string Method { get; private set; }
        public int StatusCode { get; private set; }
        public int ResponseTimeMs { get; private set; }
        public DateTime Timestamp { get; private set; }

        private ApiUsageLog() { } // EF Core

        private ApiUsageLog(
            Guid apiKeyId,
            string endpoint,
            string method,
            int statusCode,
            int responseTimeMs)
        {
            ApiKeyId = apiKeyId;
            Endpoint = endpoint;
            Method = method;
            StatusCode = statusCode;
            ResponseTimeMs = responseTimeMs;
            Timestamp = DateTime.UtcNow;
        }

        public static ApiUsageLog Create(
            Guid apiKeyId,
            string endpoint,
            string method,
            int statusCode,
            int responseTimeMs)
        {
            if (apiKeyId == Guid.Empty)
                throw new ArgumentException("API Key ID cannot be empty", nameof(apiKeyId));

            if (string.IsNullOrWhiteSpace(endpoint))
                throw new ArgumentException("Endpoint cannot be empty", nameof(endpoint));

            if (string.IsNullOrWhiteSpace(method))
                throw new ArgumentException("Method cannot be empty", nameof(method));

            return new ApiUsageLog(apiKeyId, endpoint, method, statusCode, responseTimeMs);
        }
    }
}
