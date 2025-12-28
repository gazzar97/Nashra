using SportsData.Shared;

namespace SportsData.Modules.Competitions.Application.Players.Services
{
    public interface IPlayerService
    {
        Task<PagedList<PlayerDto>> GetPlayersAsync(Guid? teamId, Guid? seasonId, string? position, string? nationality, int page, int pageSize);
        Task<PlayerDetailDto?> GetPlayerByIdAsync(Guid playerId);
        Task<PagedList<PlayerDto>> GetPlayersByTeamAsync(Guid teamId, Guid seasonId, int page, int pageSize);
    }

    public record PlayerDto(
        Guid Id, 
        string Name, 
        DateTime? DateOfBirth, 
        string Nationality, 
        string Position, 
        int? Height, 
        int? Weight,
        int? ShirtNumber,
        string? TeamName);

    public record PlayerDetailDto(
        Guid Id,
        string Name,
        DateTime? DateOfBirth,
        string Nationality,
        string Position,
        int? Height,
        int? Weight,
        PlayerCurrentTeamDto? CurrentTeam,
        List<PlayerCareerEntryDto> CareerHistory);

    public record PlayerCurrentTeamDto(
        Guid TeamId,
        string TeamName,
        Guid SeasonId,
        string SeasonYear,
        int? ShirtNumber,
        DateTime StartDate);

    public record PlayerCareerEntryDto(
        Guid TeamId,
        string TeamName,
        Guid SeasonId,
        string SeasonYear,
        int? ShirtNumber,
        DateTime StartDate,
        DateTime? EndDate);
}
