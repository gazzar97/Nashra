using FluentValidation;
using MediatR;
using SportsData.Modules.ApiKeys.Infrastructure.Repositories;
using SportsData.Shared;

namespace SportsData.Modules.ApiKeys.Application.ApiKeys.Commands
{
    public record RevokeApiKeyCommand : ICommand<bool>
    {
        public Guid Id { get; init; }
    }

    public class RevokeApiKeyCommandValidator : AbstractValidator<RevokeApiKeyCommand>
    {
        public RevokeApiKeyCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("API Key ID is required");
        }
    }

    public class RevokeApiKeyCommandHandler : ICommandHandler<RevokeApiKeyCommand, bool>
    {
        private readonly IApiKeyRepository _repository;

        public RevokeApiKeyCommandHandler(IApiKeyRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<bool>> Handle(RevokeApiKeyCommand request, CancellationToken cancellationToken)
        {
            var apiKey = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (apiKey == null)
                return Result<bool>.Failure("API key not found");

            apiKey.Revoke();
            await _repository.UpdateAsync(apiKey, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
