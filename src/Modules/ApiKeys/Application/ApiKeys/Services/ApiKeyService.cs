using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using SportsData.Modules.ApiKeys.Domain;
using SportsData.Modules.ApiKeys.Infrastructure.Repositories;
using SportsData.Shared;

namespace SportsData.Modules.ApiKeys.Application.ApiKeys.Services
{
    public class ApiKeyService : IApiKeyService
    {
        private readonly IApiKeyRepository _repository;
        private readonly IConfiguration _configuration;
        private const string KeyPrefix = "sk_live_";
        private const int KeyLength = 32;

        public ApiKeyService(IApiKeyRepository repository, IConfiguration configuration)
        {
            _repository = repository;
            _configuration = configuration;
        }

        public Task<Result<string>> GenerateApiKeyAsync()
        {
            try
            {
                // Generate cryptographically secure random bytes
                var randomBytes = new byte[KeyLength];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(randomBytes);
                }

                // Convert to base64 and make URL-safe
                var randomString = Convert.ToBase64String(randomBytes)
                    .Replace("+", "")
                    .Replace("/", "")
                    .Replace("=", "")
                    .Substring(0, KeyLength);

                var apiKey = $"{KeyPrefix}{randomString}";
                return Task.FromResult(Result<string>.Success(apiKey));
            }
            catch (Exception ex)
            {
                return Task.FromResult(Result<string>.Failure($"Failed to generate API key: {ex.Message}"));
            }
        }

        public string HashApiKey(string rawKey)
        {
            if (string.IsNullOrWhiteSpace(rawKey))
                throw new ArgumentException("Raw key cannot be empty", nameof(rawKey));

            // Get secret from configuration or use default for development
            var secret = _configuration["ApiKeys:HashSecret"] ?? "default-secret-change-in-production";
            var secretBytes = Encoding.UTF8.GetBytes(secret);

            using (var hmac = new HMACSHA256(secretBytes))
            {
                var keyBytes = Encoding.UTF8.GetBytes(rawKey);
                var hashBytes = hmac.ComputeHash(keyBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        public async Task<Result<ApiKey>> ValidateApiKeyAsync(string rawKey, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(rawKey))
                return Result<ApiKey>.Failure("API key is required");

            try
            {
                var keyHash = HashApiKey(rawKey);
                var apiKey = await _repository.GetByKeyHashAsync(keyHash, cancellationToken);

                if (apiKey == null)
                    return Result<ApiKey>.Failure("Invalid API key");

                if (!apiKey.IsValid())
                {
                    if (apiKey.RevokedAt.HasValue)
                        return Result<ApiKey>.Failure("API key has been revoked");
                    
                    if (!apiKey.IsActive)
                        return Result<ApiKey>.Failure("API key is not active");
                    
                    if (apiKey.ExpiresAt.HasValue && apiKey.ExpiresAt.Value <= DateTime.UtcNow)
                        return Result<ApiKey>.Failure("API key has expired");
                }

                return Result<ApiKey>.Success(apiKey);
            }
            catch (Exception ex)
            {
                return Result<ApiKey>.Failure($"Failed to validate API key: {ex.Message}");
            }
        }
    }
}
