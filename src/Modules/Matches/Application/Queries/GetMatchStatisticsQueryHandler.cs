using MediatR;
using Microsoft.EntityFrameworkCore;
using SportsData.Modules.Matches.Infrastructure;
using SportsData.Shared;

namespace SportsData.Modules.Matches.Application.Queries;

public class GetMatchStatisticsQueryHandler : IRequestHandler<GetMatchStatisticsQuery, Result<MatchStatisticsDto?>>
{
    private readonly MatchesDbContext _context;

    public GetMatchStatisticsQueryHandler(MatchesDbContext context)
    {
        _context = context;
    }

    public async Task<Result<MatchStatisticsDto?>> Handle(GetMatchStatisticsQuery request, CancellationToken cancellationToken)
    {
        // First check if match exists
        var matchExists = await _context.Matches.AnyAsync(m => m.Id == request.MatchId, cancellationToken);
        if (!matchExists)
            return Result<MatchStatisticsDto?>.Failure("Match not found.");

        // Get statistics
        var statistics = await _context.MatchStatistics
            .Where(s => s.MatchId == request.MatchId)
            .Select(s => new MatchStatisticsDto(
                s.MatchId,
                s.PossessionHome,
                s.PossessionAway,
                s.ShotsHome,
                s.ShotsAway,
                s.ShotsOnTargetHome,
                s.ShotsOnTargetAway,
                s.CornersHome,
                s.CornersAway,
                s.FoulsHome,
                s.FoulsAway,
                s.YellowCardsHome,
                s.YellowCardsAway,
                s.RedCardsHome,
                s.RedCardsAway
            ))
            .FirstOrDefaultAsync(cancellationToken);

        // Return success even if statistics is null (not yet available)
        return Result<MatchStatisticsDto?>.Success(statistics);
    }
}
