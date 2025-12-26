using Microsoft.EntityFrameworkCore;
using SportsData.Modules.ApiKeys.Domain;

namespace SportsData.Modules.ApiKeys.Infrastructure.Repositories
{
    public class ApiKeyRepository : IApiKeyRepository
    {
        private readonly ApiKeysDbContext _context;

        public ApiKeyRepository(ApiKeysDbContext context)
        {
            _context = context;
        }

        public async Task<ApiKey?> GetByKeyHashAsync(string keyHash, CancellationToken cancellationToken = default)
        {
            return await _context.ApiKeys
                .FirstOrDefaultAsync(x => x.KeyHash == keyHash, cancellationToken);
        }

        public async Task<ApiKey?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.ApiKeys
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<ApiKey>> GetAllAsync(
            int page,
            int pageSize,
            Guid? ownerId = null,
            ApiKeyPlan? plan = null,
            bool? isActive = null,
            CancellationToken cancellationToken = default)
        {
            var query = _context.ApiKeys.AsQueryable();

            if (ownerId.HasValue)
                query = query.Where(x => x.OwnerId == ownerId.Value);

            if (plan.HasValue)
                query = query.Where(x => x.Plan == plan.Value);

            if (isActive.HasValue)
                query = query.Where(x => x.IsActive == isActive.Value);

            return await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> GetCountAsync(
            Guid? ownerId = null,
            ApiKeyPlan? plan = null,
            bool? isActive = null,
            CancellationToken cancellationToken = default)
        {
            var query = _context.ApiKeys.AsQueryable();

            if (ownerId.HasValue)
                query = query.Where(x => x.OwnerId == ownerId.Value);

            if (plan.HasValue)
                query = query.Where(x => x.Plan == plan.Value);

            if (isActive.HasValue)
                query = query.Where(x => x.IsActive == isActive.Value);

            return await query.CountAsync(cancellationToken);
        }

        public async Task AddAsync(ApiKey apiKey, CancellationToken cancellationToken = default)
        {
            await _context.ApiKeys.AddAsync(apiKey, cancellationToken);
        }

        public async Task UpdateAsync(ApiKey apiKey, CancellationToken cancellationToken = default)
        {
            _context.ApiKeys.Update(apiKey);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
