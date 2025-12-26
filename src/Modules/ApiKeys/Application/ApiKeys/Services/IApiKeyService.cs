using SportsData.Modules.ApiKeys.Domain;
using SportsData.Shared;

namespace SportsData.Modules.ApiKeys.Application.ApiKeys.Services
{
    public interface IApiKeyService
    {
        Task<Result<string>> GenerateApiKeyAsync();
        string HashApiKey(string rawKey);
        Task<Result<ApiKey>> ValidateApiKeyAsync(string rawKey, CancellationToken cancellationToken = default);
    }
}
