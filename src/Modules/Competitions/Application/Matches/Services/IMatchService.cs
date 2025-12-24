using SportsData.Shared;

namespace SportsData.Modules.Competitions.Application.Matches.Services
{
    public interface IMatchService
    {
        Task<PagedList<MatchDto>> GetMatchesAsync(
            Guid seasonId,
            Guid? teamId,
            DateTime? fromDate,
            DateTime? toDate,
            int page,
            int pageSize);
    }

    public record MatchDto(
        Guid Id,
        string SeasonYear,
        DateTime MatchDate,
        string Status,  // Enum serialized as string in JSON (e.g., "Finished", "Live")
        TeamBasicDto HomeTeam,
        TeamBasicDto AwayTeam,
        ScoreDto? Score,
        string Venue
    );

    public record TeamBasicDto(Guid Id, string Name, string LogoUrl);
    
    public record ScoreDto(int Home, int Away);
}
