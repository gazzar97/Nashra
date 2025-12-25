using MediatR;
using SportsData.Shared;

namespace SportsData.Modules.Matches.Application.Queries;

/// <summary>
/// Query to retrieve a paginated list of matches with optional filters.
/// </summary>
public record GetMatchesQuery(
    Guid? SeasonId = null,
    Guid? TeamId = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    string? Status = null,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<Result<PagedList<MatchDto>>>;

/// <summary>
/// DTO representing a match.
/// </summary>
public record MatchDto(
    Guid Id,
    Guid SeasonId,
    Guid HomeTeamId,
    Guid AwayTeamId,
    DateTime MatchDate,
    string Status,
    int? HomeScore,
    int? AwayScore,
    string Venue
);
