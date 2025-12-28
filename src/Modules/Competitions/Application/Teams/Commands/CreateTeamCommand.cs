using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SportsData.Modules.Competitions.Infrastructure;
using SportsData.Shared;

namespace SportsData.Modules.Competitions.Application.Teams.Commands
{
    public record CreateTeamCommand(
        string Name,
        string ShortName,
        string Code,
        string LogoUrl,
        int? FoundedYear,
        string Stadium) : IRequest<Result<Guid>>;

    public class CreateTeamCommandValidator : AbstractValidator<CreateTeamCommand>
    {
        public CreateTeamCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(200)
                .WithMessage("Team name is required and must not exceed 200 characters");

            RuleFor(x => x.ShortName)
                .NotEmpty()
                .MaximumLength(100)
                .WithMessage("Short name is required and must not exceed 100 characters");

            RuleFor(x => x.Code)
                .NotEmpty()
                .MaximumLength(10)
                .WithMessage("Code is required and must not exceed 10 characters");

            RuleFor(x => x.LogoUrl)
                .NotEmpty()
                .MaximumLength(500)
                .WithMessage("Logo URL is required and must not exceed 500 characters");

            RuleFor(x => x.FoundedYear)
                .GreaterThan(1800)
                .LessThanOrEqualTo(DateTime.UtcNow.Year)
                .When(x => x.FoundedYear.HasValue)
                .WithMessage("Founded year must be between 1800 and current year");

            RuleFor(x => x.Stadium)
                .NotEmpty()
                .MaximumLength(200)
                .WithMessage("Stadium is required and must not exceed 200 characters");
        }
    }

    public class CreateTeamCommandHandler : IRequestHandler<CreateTeamCommand, Result<Guid>>
    {
        private readonly CompetitionsDbContext _dbContext;
        private readonly IMemoryCache _cache;

        public CreateTeamCommandHandler(CompetitionsDbContext dbContext, IMemoryCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        public async Task<Result<Guid>> Handle(CreateTeamCommand request, CancellationToken cancellationToken)
        {
            // Check if team with same code already exists
            var existingTeam = await _dbContext.Teams
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (existingTeam != null)
            {
                return Result<Guid>.Failure(
                    Error.Conflict("Team.CodeExists", $"A team with code '{request.Code}' already exists"),
                    StatusCodes.Status409Conflict);
            }

            var team = new Domain.Team(
                request.Name,
                request.ShortName,
                request.Code,
                request.LogoUrl,
                request.FoundedYear,
                request.Stadium);

            _dbContext.Teams.Add(team);
            await _dbContext.SaveChangesAsync(cancellationToken);

            // Invalidate cache
            _cache.Remove("teams_");

            return Result<Guid>.Success(team.Id, StatusCodes.Status201Created);
        }
    }
}
