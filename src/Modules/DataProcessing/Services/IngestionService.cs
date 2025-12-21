using SportsData.Modules.DataProcessing.Dtos;
using MediatR;

namespace SportsData.Modules.DataProcessing.Services
{
    public interface IIngestionService
    {
        Task ImportLeaguesAsync(List<ExternalLeagueDto> leagues);
    }

    public class IngestionService : IIngestionService
    {
        private readonly ISender _sender;

        public IngestionService(ISender sender)
        {
            _sender = sender;
        }

        public async Task ImportLeaguesAsync(List<ExternalLeagueDto> leagues)
        {
            foreach (var leagueDto in leagues)
            {
                var createLeagueCommand = new SportsData.Modules.Competitions.Application.Leagues.CreateLeague.CreateLeagueCommand(
                    leagueDto.Name, leagueDto.Country, leagueDto.LogoUrl, leagueDto.Tier, leagueDto.SeasonStart, leagueDto.SeasonEnd);

                var leagueResult = await _sender.Send(createLeagueCommand);

                if (leagueResult.IsSuccess)
                {
                    // Handle Seasons and Teams if necessary
                    // For MVP, just creating the league
                }
            }
        }
    }
}
