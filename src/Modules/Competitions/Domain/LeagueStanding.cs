using SportsData.Shared;

namespace SportsData.Modules.Competitions.Domain
{
    public class LeagueStanding : Entity
    {
        public Guid LeagueId { get; private set; }
        public Guid SeasonId { get; private set; }
        public Guid TeamId { get; private set; }
        public int Position { get; private set; }
        public int Played { get; private set; }
        public int Won { get; private set; }
        public int Drawn { get; private set; }
        public int Lost { get; private set; }
        public int GoalsFor { get; private set; }
        public int GoalsAgainst { get; private set; }
        public int GoalDifference { get; private set; }
        public int Points { get; private set; }

        private LeagueStanding() { }

        public LeagueStanding(
            Guid leagueId,
            Guid seasonId,
            Guid teamId,
            int position,
            int played,
            int won,
            int drawn,
            int lost,
            int goalsFor,
            int goalsAgainst)
        {
            LeagueId = leagueId;
            SeasonId = seasonId;
            TeamId = teamId;
            Position = position;
            Played = played;
            Won = won;
            Drawn = drawn;
            Lost = lost;
            GoalsFor = goalsFor;
            GoalsAgainst = goalsAgainst;
            GoalDifference = goalsFor - goalsAgainst;
            Points = (won * 3) + drawn;
        }

        public void UpdateStats(int played, int won, int drawn, int lost, int goalsFor, int goalsAgainst)
        {
            Played = played;
            Won = won;
            Drawn = drawn;
            Lost = lost;
            GoalsFor = goalsFor;
            GoalsAgainst = goalsAgainst;
            GoalDifference = goalsFor - goalsAgainst;
            Points = (won * 3) + drawn;
        }

        public void UpdatePosition(int position)
        {
            Position = position;
        }
    }
}
