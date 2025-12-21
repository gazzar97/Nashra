using SportsData.Shared;

namespace SportsData.Modules.Competitions.Domain
{
    public class Season : Entity
    {
        public Guid LeagueId { get; private set; }
        public string Year { get; private set; } // e.g. "2023/2024"
        public bool IsCurrent { get; private set; }

        private Season() { }

        public Season(Guid leagueId, string year, bool isCurrent = false)
        {
            LeagueId = leagueId;
            Year = year;
            IsCurrent = isCurrent;
        }
    }
}
