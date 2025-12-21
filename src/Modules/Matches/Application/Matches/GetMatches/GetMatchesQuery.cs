using MediatR;
using SportsData.Shared;

namespace SportsData.Modules.Matches.Application.Matches.GetMatches
{
    public class GetMatchesQuery : IRequest<Result<List<MatchDto>>>;
}
