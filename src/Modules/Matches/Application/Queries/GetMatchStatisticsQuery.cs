using MediatR;
using SportsData.Shared;

namespace SportsData.Modules.Matches.Application.Queries;

/// <summary>
/// Query to retrieve statistics for a specific match.
/// </summary>
public record GetMatchStatisticsQuery(Guid MatchId) : IRequest<Result<MatchStatisticsDto?>>;

/// <summary>
/// DTO representing comprehensive match statistics.
/// </summary>
public record MatchStatisticsDto(
    Guid MatchId,
    int PossessionHome,
    int PossessionAway,
    int ShotsHome,
    int ShotsAway,
    int ShotsOnTargetHome,
    int ShotsOnTargetAway,
    int CornersHome,
    int CornersAway,
    int FoulsHome,
    int FoulsAway,
    int YellowCardsHome,
    int YellowCardsAway,
    int RedCardsHome,
    int RedCardsAway
);
