using MediatR;
using SportsData.Shared;

namespace SportsData.Modules.Matches.Application.Matches.GetMatches
{
    public class GetMatchesQueryHandler : IRequestHandler<GetMatchesQuery, Result<List<MatchDto>>>
    {
        public async Task<Result<List<MatchDto>>> Handle(GetMatchesQuery request, CancellationToken cancellationToken)
        {
            var matches = new List<MatchDto>
            {
                new(Guid.NewGuid(), "Al Ahly vs Zamalek", "Scheduled")
            };

            await Task.Delay(100, cancellationToken); // Simulate async work

            return Result<List<MatchDto>>.Success(matches);
        }
    }
}
