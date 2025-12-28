using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SportsData.Modules.Competitions.Infrastructure;
using SportsData.Shared;

namespace SportsData.Modules.Competitions.Application.Players.Commands
{
    public record UpdatePlayerCommand(
        Guid Id,
        string Name,
        DateTime? DateOfBirth,
        string Nationality,
        string Position,
        int? Height,
        int? Weight) : IRequest<Result>;

    public class UpdatePlayerCommandValidator : AbstractValidator<UpdatePlayerCommand>
    {
        public UpdatePlayerCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Player ID is required");

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

    public class UpdatePlayerCommandHandler : IRequestHandler<UpdatePlayerCommand, Result>
    {
        private readonly CompetitionsDbContext _dbContext;
        private readonly IMemoryCache _cache;

        public UpdatePlayerCommandHandler(CompetitionsDbContext dbContext, IMemoryCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        public async Task<Result> Handle(UpdatePlayerCommand request, CancellationToken cancellationToken)
        {
            var player = await _dbContext.Players.FindAsync(new object[] { request.Id }, cancellationToken);

            if (player == null)
            {
                return Result.Failure(
                    Error.NotFound("Player.NotFound", $"Player with ID {request.Id} was not found"),
                    StatusCodes.Status404NotFound);
            }

            // Update player properties using reflection (since setters are private)
            var nameProperty = typeof(Domain.Player).GetProperty("Name");
            var dobProperty = typeof(Domain.Player).GetProperty("DateOfBirth");
            var nationalityProperty = typeof(Domain.Player).GetProperty("Nationality");
            var positionProperty = typeof(Domain.Player).GetProperty("Position");
            var heightProperty = typeof(Domain.Player).GetProperty("Height");
            var weightProperty = typeof(Domain.Player).GetProperty("Weight");

            nameProperty?.SetValue(player, request.Name);
            dobProperty?.SetValue(player, request.DateOfBirth);
            nationalityProperty?.SetValue(player, request.Nationality);
            positionProperty?.SetValue(player, request.Position);
            heightProperty?.SetValue(player, request.Height);
            weightProperty?.SetValue(player, request.Weight);

            await _dbContext.SaveChangesAsync(cancellationToken);

            // Invalidate cache
            _cache.Remove($"player_{request.Id}");
            _cache.Remove($"players_");

            return Result.Success(StatusCodes.Status204NoContent);
        }
    }
}
