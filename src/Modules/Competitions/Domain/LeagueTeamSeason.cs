using SportsData.Shared;

namespace SportsData.Modules.Competitions.Domain
{
    public class LeagueTeamSeason : Entity
    {
        public Guid LeagueId { get; private set; }
        public Guid TeamId { get; private set; }
        public Guid SeasonId { get; private set; }

        private LeagueTeamSeason() { }

        public LeagueTeamSeason(Guid leagueId, Guid teamId, Guid seasonId)
        {
            LeagueId = leagueId;
            TeamId = teamId;
            SeasonId = seasonId;
        }
    }
}
