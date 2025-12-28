using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using SportsData.Modules.Competitions.Domain;
using SportsData.Modules.Competitions.Infrastructure;
using SportsData.Shared;

namespace SportsData.Modules.Competitions.Application.Leagues.Commands
{
    public record CreateLeagueCommand(
        string Name,
        string Country,
        string LogoUrl,
        int Tier,
        string SeasonStart,
        string SeasonEnd) : IRequest<Result<Guid>>;

    public class CreateLeagueCommandValidator : AbstractValidator<CreateLeagueCommand>
    {
        public CreateLeagueCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(200)
                .WithMessage("League name is required and must not exceed 200 characters");

            RuleFor(x => x.Country)
                .NotEmpty()
                .MaximumLength(100)
                .WithMessage("Country is required and must not exceed 100 characters");

            RuleFor(x => x.LogoUrl)
                .NotEmpty()
                .MaximumLength(500)
                .WithMessage("Logo URL is required and must not exceed 500 characters");

            RuleFor(x => x.Tier)
                .GreaterThan(0)
                .WithMessage("Tier must be greater than 0");

            RuleFor(x => x.SeasonStart)
                .NotEmpty()
                .MaximumLength(50)
                .WithMessage("Season start is required and must not exceed 50 characters");

            RuleFor(x => x.SeasonEnd)
                .NotEmpty()
                .MaximumLength(50)
                .WithMessage("Season end is required and must not exceed 50 characters");
        }
    }

    public class CreateLeagueCommandHandler : IRequestHandler<CreateLeagueCommand, Result<Guid>>
    {
        private readonly CompetitionsDbContext _dbContext;
        private readonly IMemoryCache _cache;

        public CreateLeagueCommandHandler(CompetitionsDbContext dbContext, IMemoryCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        public async Task<Result<Guid>> Handle(CreateLeagueCommand request, CancellationToken cancellationToken)
        {
            var league = League.Create(
                request.Name,
                request.Country,
                request.LogoUrl,
                request.Tier,
                request.SeasonStart,
                request.SeasonEnd);

            _dbContext.Leagues.Add(league);
            await _dbContext.SaveChangesAsync(cancellationToken);

            // Invalidate cache
            _cache.Remove("leagues_");

            return Result<Guid>.Success(league.Id, StatusCodes.Status201Created);
        }
    }
}
