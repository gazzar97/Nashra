using FluentValidation;
using MediatR;
using SportsData.Modules.ApiKeys.Domain;
using SportsData.Modules.ApiKeys.Infrastructure.Repositories;
using SportsData.Shared;

namespace SportsData.Modules.ApiKeys.Application.ApiKeys.Queries
{
    public record GetApiKeysQuery : PagedRequest, IQuery<PagedList<ApiKeyDto>>
    {
        public Guid? OwnerId { get; init; }
        public ApiKeyPlan? Plan { get; init; }
        public bool? IsActive { get; init; }
    }

    public record ApiKeyDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public Guid OwnerId { get; init; }
        public ApiKeyPlan Plan { get; init; }
        public bool IsActive { get; init; }
        public int RateLimitPerMinute { get; init; }
        public int RateLimitPerDay { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? LastUsedAt { get; init; }
        public DateTime? ExpiresAt { get; init; }
        public DateTime? RevokedAt { get; init; }
    }

    public class GetApiKeysQueryValidator : AbstractValidator<GetApiKeysQuery>
    {
        public GetApiKeysQueryValidator()
        {
            RuleFor(x => x.Page).GreaterThan(0);
            RuleFor(x => x.PageSize).GreaterThan(0).LessThanOrEqualTo(100);
        }
    }

    public class GetApiKeysQueryHandler : IQueryHandler<GetApiKeysQuery, PagedList<ApiKeyDto>>
    {
        private readonly IApiKeyRepository _repository;

        public GetApiKeysQueryHandler(IApiKeyRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<PagedList<ApiKeyDto>>> Handle(GetApiKeysQuery request, CancellationToken cancellationToken)
        {
            var apiKeys = await _repository.GetAllAsync(
                request.Page,
                request.PageSize,
                request.OwnerId,
                request.Plan,
                request.IsActive,
                cancellationToken);

            var totalCount = await _repository.GetCountAsync(
                request.OwnerId,
                request.Plan,
                request.IsActive,
                cancellationToken);

            var dtos = apiKeys.Select(x => new ApiKeyDto
            {
                Id = x.Id,
                Name = x.Name,
                OwnerId = x.OwnerId,
                Plan = x.Plan,
                IsActive = x.IsActive,
                RateLimitPerMinute = x.RateLimitPerMinute,
                RateLimitPerDay = x.RateLimitPerDay,
                CreatedAt = x.CreatedAt,
                LastUsedAt = x.LastUsedAt,
                ExpiresAt = x.ExpiresAt,
                RevokedAt = x.RevokedAt
            }).ToList();

            var pagedList = new PagedList<ApiKeyDto>(dtos, totalCount, request.Page, request.PageSize);
            return Result<PagedList<ApiKeyDto>>.Success(pagedList);
        }
    }
}
