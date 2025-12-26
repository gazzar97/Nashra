using FluentValidation;
using MediatR;
using SportsData.Modules.ApiKeys.Application.ApiKeys.Services;
using SportsData.Modules.ApiKeys.Domain;
using SportsData.Modules.ApiKeys.Infrastructure.Repositories;
using SportsData.Shared;

namespace SportsData.Modules.ApiKeys.Application.ApiKeys.Commands
{
    public record CreateApiKeyCommand : ICommand<CreateApiKeyResponse>
    {
        public string Name { get; init; } = string.Empty;
        public Guid OwnerId { get; init; }
        public ApiKeyPlan Plan { get; init; }
        public int? RateLimitPerMinute { get; init; }
        public int? RateLimitPerDay { get; init; }
        public DateTime? ExpiresAt { get; init; }
    }

    public record CreateApiKeyResponse
    {
        public Guid Id { get; init; }
        public string RawKey { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public ApiKeyPlan Plan { get; init; }
        public int RateLimitPerMinute { get; init; }
        public int RateLimitPerDay { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? ExpiresAt { get; init; }
    }

    public class CreateApiKeyCommandValidator : AbstractValidator<CreateApiKeyCommand>
    {
        public CreateApiKeyCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");

            RuleFor(x => x.OwnerId)
                .NotEmpty().WithMessage("Owner ID is required");

            RuleFor(x => x.Plan)
                .IsInEnum().WithMessage("Invalid plan");

            When(x => x.RateLimitPerMinute.HasValue, () =>
            {
                RuleFor(x => x.RateLimitPerMinute!.Value)
                    .GreaterThan(0).WithMessage("Rate limit per minute must be positive");
            });

            When(x => x.RateLimitPerDay.HasValue, () =>
            {
                RuleFor(x => x.RateLimitPerDay!.Value)
                    .GreaterThan(0).WithMessage("Rate limit per day must be positive");
            });

            When(x => x.ExpiresAt.HasValue, () =>
            {
                RuleFor(x => x.ExpiresAt!.Value)
                    .GreaterThan(DateTime.UtcNow).WithMessage("Expiration date must be in the future");
            });
        }
    }

    public class CreateApiKeyCommandHandler : ICommandHandler<CreateApiKeyCommand, CreateApiKeyResponse>
    {
        private readonly IApiKeyService _apiKeyService;
        private readonly IApiKeyRepository _repository;

        public CreateApiKeyCommandHandler(IApiKeyService apiKeyService, IApiKeyRepository repository)
        {
            _apiKeyService = apiKeyService;
            _repository = repository;
        }

        public async Task<Result<CreateApiKeyResponse>> Handle(CreateApiKeyCommand request, CancellationToken cancellationToken)
        {
            // Generate raw API key
            var keyResult = await _apiKeyService.GenerateApiKeyAsync();
            if (!keyResult.IsSuccess)
                return Result<CreateApiKeyResponse>.Failure(keyResult.Errors);

            var rawKey = keyResult.Value!;
            var keyHash = _apiKeyService.HashApiKey(rawKey);

            // Set default rate limits based on plan
            var rateLimitPerMinute = request.RateLimitPerMinute ?? GetDefaultRateLimitPerMinute(request.Plan);
            var rateLimitPerDay = request.RateLimitPerDay ?? GetDefaultRateLimitPerDay(request.Plan);

            // Create API key entity
            var apiKey = ApiKey.Create(
                keyHash,
                request.Name,
                request.OwnerId,
                request.Plan,
                rateLimitPerMinute,
                rateLimitPerDay,
                request.ExpiresAt);

            // Save to database
            await _repository.AddAsync(apiKey, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            // Return response with raw key (shown only once!)
            var response = new CreateApiKeyResponse
            {
                Id = apiKey.Id,
                RawKey = rawKey,
                Name = apiKey.Name,
                Plan = apiKey.Plan,
                RateLimitPerMinute = apiKey.RateLimitPerMinute,
                RateLimitPerDay = apiKey.RateLimitPerDay,
                CreatedAt = apiKey.CreatedAt,
                ExpiresAt = apiKey.ExpiresAt
            };

            return Result<CreateApiKeyResponse>.Success(response);
        }

        private int GetDefaultRateLimitPerMinute(ApiKeyPlan plan)
        {
            return plan switch
            {
                ApiKeyPlan.Free => 30,
                ApiKeyPlan.Pro => 300,
                ApiKeyPlan.Enterprise => 1000,
                _ => 30
            };
        }

        private int GetDefaultRateLimitPerDay(ApiKeyPlan plan)
        {
            return plan switch
            {
                ApiKeyPlan.Free => 1000,
                ApiKeyPlan.Pro => 50000,
                ApiKeyPlan.Enterprise => 1000000,
                _ => 1000
            };
        }
    }
}
