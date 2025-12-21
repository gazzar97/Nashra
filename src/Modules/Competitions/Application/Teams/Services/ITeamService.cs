using SportsData.Shared;

namespace SportsData.Modules.Competitions.Application.Teams.Services
{
    public interface ITeamService
    {
        Task<PagedList<TeamDto>> GetTeamsAsync(Guid? leagueId, Guid? seasonId, int page, int pageSize);
    }

    public record TeamDto(Guid Id, string Name, string ShortName, string Code, string LogoUrl, int? FoundedYear, string Stadium);
}
