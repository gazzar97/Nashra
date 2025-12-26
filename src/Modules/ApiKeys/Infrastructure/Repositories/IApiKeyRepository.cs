using SportsData.Modules.ApiKeys.Domain;

namespace SportsData.Modules.ApiKeys.Infrastructure.Repositories
{
    public interface IApiKeyRepository
    {
        Task<ApiKey?> GetByKeyHashAsync(string keyHash, CancellationToken cancellationToken = default);
        Task<ApiKey?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<ApiKey>> GetAllAsync(int page, int pageSize, Guid? ownerId = null, ApiKeyPlan? plan = null, bool? isActive = null, CancellationToken cancellationToken = default);
        Task<int> GetCountAsync(Guid? ownerId = null, ApiKeyPlan? plan = null, bool? isActive = null, CancellationToken cancellationToken = default);
        Task AddAsync(ApiKey apiKey, CancellationToken cancellationToken = default);
        Task UpdateAsync(ApiKey apiKey, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
