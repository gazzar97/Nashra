using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using SportsData.Modules.Competitions.Domain;
using SportsData.Modules.Competitions.Infrastructure;
using SportsData.Shared;

namespace SportsData.Modules.Competitions.Application.Players.Commands
{
    public record CreatePlayerCommand(
        string Name,
        DateTime? DateOfBirth,
        string Nationality,
        string Position,
        int? Height,
        int? Weight) : IRequest<Result<Guid>>;

    public class CreatePlayerCommandValidator : AbstractValidator<CreatePlayerCommand>
    {
        public CreatePlayerCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(200)
                .WithMessage("Player name is required and must not exceed 200 characters");

            RuleFor(x => x.Nationality)
                .NotEmpty()
                .MaximumLength(100)
                .WithMessage("Nationality is required and must not exceed 100 characters");

            RuleFor(x => x.Position)
                .NotEmpty()
                .MaximumLength(50)
                .WithMessage("Position is required and must not exceed 50 characters");

            RuleFor(x => x.Height)
                .GreaterThan(0)
                .LessThanOrEqualTo(300)
                .When(x => x.Height.HasValue)
                .WithMessage("Height must be between 1 and 300 cm");

            RuleFor(x => x.Weight)
                .GreaterThan(0)
                .LessThanOrEqualTo(200)
                .When(x => x.Weight.HasValue)
                .WithMessage("Weight must be between 1 and 200 kg");

            RuleFor(x => x.DateOfBirth)
                .LessThan(DateTime.UtcNow)
                .When(x => x.DateOfBirth.HasValue)
                .WithMessage("Date of birth must be in the past");
        }
    }

    public class CreatePlayerCommandHandler : IRequestHandler<CreatePlayerCommand, Result<Guid>>
    {
        private readonly CompetitionsDbContext _dbContext;
        private readonly IMemoryCache _cache;

        public CreatePlayerCommandHandler(CompetitionsDbContext dbContext, IMemoryCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        public async Task<Result<Guid>> Handle(CreatePlayerCommand request, CancellationToken cancellationToken)
        {
            var player = new Player(
                request.Name,
                request.DateOfBirth,
                request.Nationality,
                request.Position,
                request.Height,
                request.Weight);

            _dbContext.Players.Add(player);
            await _dbContext.SaveChangesAsync(cancellationToken);

            // Invalidate cache
            _cache.Remove($"players_");

            return Result<Guid>.Success(player.Id, StatusCodes.Status201Created);
        }
    }
}
