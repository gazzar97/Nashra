using MediatR;
using Microsoft.EntityFrameworkCore;
using SportsData.Modules.Matches.Domain;
using SportsData.Modules.Matches.Infrastructure;
using SportsData.Shared;

namespace SportsData.Modules.Matches.Application.Queries;

public class GetMatchesQueryHandler : IRequestHandler<GetMatchesQuery, Result<PagedList<MatchDto>>>
{
    private readonly MatchesDbContext _context;

    public GetMatchesQueryHandler(MatchesDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PagedList<MatchDto>>> Handle(GetMatchesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Matches.AsQueryable();

        // Apply filters
        if (request.SeasonId.HasValue)
            query = query.Where(m => m.SeasonId == request.SeasonId.Value);

        if (request.TeamId.HasValue)
            query = query.Where(m => m.HomeTeamId == request.TeamId.Value || m.AwayTeamId == request.TeamId.Value);

        if (request.FromDate.HasValue)
            query = query.Where(m => m.MatchDate >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            query = query.Where(m => m.MatchDate <= request.ToDate.Value);

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            if (Enum.TryParse<MatchStatus>(request.Status, true, out var status))
            {
                query = query.Where(m => m.Status == status);
            }
        }

        // Order by match date descending
        query = query.OrderByDescending(m => m.MatchDate);

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var matches = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
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
            .ToListAsync(cancellationToken);

        var pagedList = new PagedList<MatchDto>(matches, totalCount, request.PageNumber, request.PageSize);

        return Result<PagedList<MatchDto>>.Success(pagedList);
    }
}
