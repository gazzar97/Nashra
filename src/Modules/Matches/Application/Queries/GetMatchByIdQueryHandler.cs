using MediatR;
using Microsoft.EntityFrameworkCore;
using SportsData.Modules.Matches.Infrastructure;
using SportsData.Shared;

namespace SportsData.Modules.Matches.Application.Queries;

public class GetMatchByIdQueryHandler : IRequestHandler<GetMatchByIdQuery, Result<MatchDto>>
{
    private readonly MatchesDbContext _context;

    public GetMatchByIdQueryHandler(MatchesDbContext context)
    {
        _context = context;
    }

    public async Task<Result<MatchDto>> Handle(GetMatchByIdQuery request, CancellationToken cancellationToken)
    {
        var match = await _context.Matches
            .Where(m => m.Id == request.MatchId)
            .Select(m => new MatchDto(
                m.Id,
                m.SeasonId,
                m.HomeTeamId,
                m.AwayTeamId,
                m.MatchDate,
                m.Status.ToString(),
                m.HomeScore,
                m.AwayScore,
                m.Venue
            ))
            .FirstOrDefaultAsync(cancellationToken);

        if (match == null)
            return Result<MatchDto>.Failure("Match not found.");

        return Result<MatchDto>.Success(match);
    }
}
