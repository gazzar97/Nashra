using SportsData.Shared;

namespace SportsData.Modules.Competitions.Domain
{
    public class TeamPlayerSeason : Entity
    {
        public Guid TeamId { get; private set; }
        public Guid PlayerId { get; private set; }
        public Guid SeasonId { get; private set; }
        public int? ShirtNumber { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }

        private TeamPlayerSeason() { }

        public TeamPlayerSeason(Guid teamId, Guid playerId, Guid seasonId, int? shirtNumber, DateTime startDate, DateTime? endDate)
        {
            TeamId = teamId;
            PlayerId = playerId;
            SeasonId = seasonId;
            ShirtNumber = shirtNumber;
            StartDate = startDate;
            EndDate = endDate;
        }
    }
}
