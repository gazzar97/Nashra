using Microsoft.EntityFrameworkCore;
using SportsData.Modules.ApiKeys.Application.ApiKeys.Services;
using SportsData.Modules.ApiKeys.Domain;
using SportsData.Modules.ApiKeys.Infrastructure.Repositories;

namespace SportsData.Modules.ApiKeys.Infrastructure.Seeders
{
    public class ApiKeysSeeder
    {
        private readonly ApiKeysDbContext _context;
        private readonly IApiKeyService _apiKeyService;
        private readonly IApiKeyRepository _repository;

        public ApiKeysSeeder(
            ApiKeysDbContext context,
            IApiKeyService apiKeyService,
            IApiKeyRepository repository)
        {
            _context = context;
            _apiKeyService = apiKeyService;
            _repository = repository;
        }

        public async Task SeedAsync()
        {
            // Check if any API keys already exist
            if (await _context.ApiKeys.AnyAsync())
            {
                Console.WriteLine("API keys already seeded.");
                return;
            }

            Console.WriteLine("Seeding API keys...");

            // Sample owner ID (you can replace this with actual customer IDs)
            var sampleOwnerId = Guid.Parse("00000000-0000-0000-0000-000000000001");

            // Create Free tier key
            var freeKeyResult = await _apiKeyService.GenerateApiKeyAsync();
            if (freeKeyResult.IsSuccess)
            {
                var freeKeyHash = _apiKeyService.HashApiKey(freeKeyResult.Value!);
                var freeKey = ApiKey.Create(
                    freeKeyHash,
                    "Free Tier Test Key",
                    sampleOwnerId,
                    ApiKeyPlan.Free,
                    30,  // 30 requests per minute
                    1000 // 1000 requests per day
                );
                await _repository.AddAsync(freeKey);
                Console.WriteLine($"Created Free tier key: {freeKeyResult.Value}");
            }

            // Create Pro tier key
            var proKeyResult = await _apiKeyService.GenerateApiKeyAsync();
            if (proKeyResult.IsSuccess)
            {
                var proKeyHash = _apiKeyService.HashApiKey(proKeyResult.Value!);
                var proKey = ApiKey.Create(
                    proKeyHash,
                    "Pro Tier Test Key",
                    sampleOwnerId,
                    ApiKeyPlan.Pro,
                    300,   // 300 requests per minute
                    50000  // 50,000 requests per day
                );
                await _repository.AddAsync(proKey);
                Console.WriteLine($"Created Pro tier key: {proKeyResult.Value}");
            }

            // Create Enterprise tier key
            var enterpriseKeyResult = await _apiKeyService.GenerateApiKeyAsync();
            if (enterpriseKeyResult.IsSuccess)
            {
                var enterpriseKeyHash = _apiKeyService.HashApiKey(enterpriseKeyResult.Value!);
                var enterpriseKey = ApiKey.Create(
                    enterpriseKeyHash,
                    "Enterprise Tier Test Key",
                    sampleOwnerId,
                    ApiKeyPlan.Enterprise,
                    1000,    // 1000 requests per minute
                    1000000  // 1,000,000 requests per day
                );
                await _repository.AddAsync(enterpriseKey);
                Console.WriteLine($"Created Enterprise tier key: {enterpriseKeyResult.Value}");
            }

            await _repository.SaveChangesAsync();
            Console.WriteLine("API keys seeding completed.");
        }
    }
}
