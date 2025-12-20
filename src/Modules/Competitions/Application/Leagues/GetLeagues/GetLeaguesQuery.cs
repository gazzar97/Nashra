using MediatR;
using SportsData.Shared;

namespace SportsData.Modules.Competitions.Application.Leagues.GetLeagues
{
    public class GetLeaguesQuery : IRequest<Result<List<LeagueDto>>>
    {
    }
}
