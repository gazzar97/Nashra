using SportsData.Shared;

namespace SportsData.Modules.Competitions.Application.Matches.Services
{
    public interface IMatchStatsService
    {
        Task<MatchStatsDto?> GetMatchStatsAsync(Guid matchId);
    }

    public record MatchStatsDto(
        Guid MatchId,
        TeamStatsDto HomeTeam,
        TeamStatsDto AwayTeam
    );

    public record TeamStatsDto(
        Guid TeamId,
        decimal Possession,
        int Shots,
        int ShotsOnTarget,
        int Corners,
        int YellowCards,
        int RedCards,
        int Fouls
    );
}
