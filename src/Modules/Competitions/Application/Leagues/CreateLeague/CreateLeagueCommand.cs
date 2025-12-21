using FluentValidation;
using MediatR;
using SportsData.Modules.Competitions.Domain;
using SportsData.Modules.Competitions.Infrastructure;
using SportsData.Shared;

namespace SportsData.Modules.Competitions.Application.Leagues.CreateLeague
{
    public record CreateLeagueCommand(string Name, string Country, string LogoUrl, int Tier, string SeasonStart, string SeasonEnd) : ICommand<Guid>;

    public class CreateLeagueCommandHandler : ICommandHandler<CreateLeagueCommand, Guid>
    {
        private readonly CompetitionsDbContext _context;

        public CreateLeagueCommandHandler(CompetitionsDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Guid>> Handle(CreateLeagueCommand request, CancellationToken cancellationToken)
        {
            var league = League.Create(request.Name, request.Country, request.LogoUrl, request.Tier, request.SeasonStart, request.SeasonEnd);

            _context.Leagues.Add(league);
            await _context.SaveChangesAsync(cancellationToken);

            return  Result<Guid>.Success(league.Id);
        }
    }
}
