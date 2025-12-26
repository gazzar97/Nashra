using FluentValidation;
using MediatR;
using SportsData.Modules.ApiKeys.Infrastructure.Repositories;
using SportsData.Shared;

namespace SportsData.Modules.ApiKeys.Application.ApiKeys.Queries
{
    public record GetApiKeyByIdQuery : IQuery<ApiKeyDto>
    {
        public Guid Id { get; init; }
    }

    public class GetApiKeyByIdQueryValidator : AbstractValidator<GetApiKeyByIdQuery>
    {
        public GetApiKeyByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("API Key ID is required");
        }
    }

    public class GetApiKeyByIdQueryHandler : IQueryHandler<GetApiKeyByIdQuery, ApiKeyDto>
    {
        private readonly IApiKeyRepository _repository;

        public GetApiKeyByIdQueryHandler(IApiKeyRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<ApiKeyDto>> Handle(GetApiKeyByIdQuery request, CancellationToken cancellationToken)
        {
            var apiKey = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (apiKey == null)
                return Result<ApiKeyDto>.Failure("API key not found");

            var dto = new ApiKeyDto
            {
                Id = apiKey.Id,
                Name = apiKey.Name,
                OwnerId = apiKey.OwnerId,
                Plan = apiKey.Plan,
                IsActive = apiKey.IsActive,
                RateLimitPerMinute = apiKey.RateLimitPerMinute,
                RateLimitPerDay = apiKey.RateLimitPerDay,
                CreatedAt = apiKey.CreatedAt,
                LastUsedAt = apiKey.LastUsedAt,
                ExpiresAt = apiKey.ExpiresAt,
                RevokedAt = apiKey.RevokedAt
            };

            return Result<ApiKeyDto>.Success(dto);
        }
    }
}
