using MediatR;
using SportsData.Modules.Competitions.Domain;
using SportsData.Shared;
using System;
using SportsData.Modules.Competitions.Infrastructure;

namespace SportsData.Modules.Competitions.Application.Teams.CreateTeam
{
    public record CreateTeamCommand(string Name, string ShortName, string Code, string LogoUrl, int? FoundedYear, string Stadium) : ICommand<Guid>;

    public class CreateTeamCommandHandler : ICommandHandler<CreateTeamCommand, Guid>
    {
        private readonly CompetitionsDbContext _context;

        public CreateTeamCommandHandler(CompetitionsDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Guid>> Handle(CreateTeamCommand request, CancellationToken cancellationToken)
        {
            var team = new Team(request.Name, request.ShortName, request.Code, request.LogoUrl, request.FoundedYear, request.Stadium);

            _context.Teams.Add(team);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(team.Id);
        }
    }
}
