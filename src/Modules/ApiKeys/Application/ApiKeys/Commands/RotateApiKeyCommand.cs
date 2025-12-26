using FluentValidation;
using MediatR;
using SportsData.Modules.ApiKeys.Application.ApiKeys.Services;
using SportsData.Modules.ApiKeys.Domain;
using SportsData.Modules.ApiKeys.Infrastructure.Repositories;
using SportsData.Shared;

namespace SportsData.Modules.ApiKeys.Application.ApiKeys.Commands
{
    public record RotateApiKeyCommand : ICommand<CreateApiKeyResponse>
    {
        public Guid Id { get; init; }
    }

    public class RotateApiKeyCommandValidator : AbstractValidator<RotateApiKeyCommand>
    {
        public RotateApiKeyCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("API Key ID is required");
        }
    }

    public class RotateApiKeyCommandHandler : ICommandHandler<RotateApiKeyCommand, CreateApiKeyResponse>
    {
        private readonly IApiKeyService _apiKeyService;
        private readonly IApiKeyRepository _repository;

        public RotateApiKeyCommandHandler(IApiKeyService apiKeyService, IApiKeyRepository repository)
        {
            _apiKeyService = apiKeyService;
            _repository = repository;
        }

        public async Task<Result<CreateApiKeyResponse>> Handle(RotateApiKeyCommand request, CancellationToken cancellationToken)
        {
            // Get existing key
            var oldKey = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (oldKey == null)
                return Result<CreateApiKeyResponse>.Failure("API key not found");

            // Revoke old key
            oldKey.Revoke();
            await _repository.UpdateAsync(oldKey, cancellationToken);

            // Generate new raw API key
            var keyResult = await _apiKeyService.GenerateApiKeyAsync();
            if (!keyResult.IsSuccess)
                return Result<CreateApiKeyResponse>.Failure(keyResult.Errors);

            var rawKey = keyResult.Value!;
            var keyHash = _apiKeyService.HashApiKey(rawKey);

            // Create new API key with same settings
            var newKey = ApiKey.Create(
                keyHash,
                oldKey.Name,
                oldKey.OwnerId,
                oldKey.Plan,
                oldKey.RateLimitPerMinute,
                oldKey.RateLimitPerDay,
                oldKey.ExpiresAt);

            await _repository.AddAsync(newKey, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            // Return response with new raw key
            var response = new CreateApiKeyResponse
            {
                Id = newKey.Id,
                RawKey = rawKey,
                Name = newKey.Name,
                Plan = newKey.Plan,
                RateLimitPerMinute = newKey.RateLimitPerMinute,
                RateLimitPerDay = newKey.RateLimitPerDay,
                CreatedAt = newKey.CreatedAt,
                ExpiresAt = newKey.ExpiresAt
            };

            return Result<CreateApiKeyResponse>.Success(response);
        }
    }
}
