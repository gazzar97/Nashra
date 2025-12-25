using MediatR;
using SportsData.Shared;

namespace SportsData.Modules.Matches.Application.Queries;

/// <summary>
/// Query to retrieve a single match by ID.
/// </summary>
public record GetMatchByIdQuery(Guid MatchId) : IRequest<Result<MatchDto>>;
