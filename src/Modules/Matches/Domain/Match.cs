using SportsData.Shared;

namespace SportsData.Modules.Matches.Domain
{
    public class Match : Entity
    {
        public Guid HomeTeamId { get; private set; }
        public Guid AwayTeamId { get; private set; }
        public Guid SeasonId { get; private set; }
        public DateTime StartTime { get; private set; }
        public MatchStatus Status { get; private set; }
        public int? HomeScore { get; private set; }
        public int? AwayScore { get; private set; }

        private Match() { }

        public Match(Guid homeTeamId, Guid awayTeamId, Guid seasonId, DateTime startTime)
        {
            HomeTeamId = homeTeamId;
            AwayTeamId = awayTeamId;
            SeasonId = seasonId;
            StartTime = startTime;
            Status = MatchStatus.Scheduled;
        }

        public void Start()
        {
            Status = MatchStatus.Live;
        }

        public void UpdateScore(int home, int away)
        {
            HomeScore = home;
            AwayScore = away;
        }

        public void Finish()
        {
            Status = MatchStatus.Finished;
        }
    }

    public enum MatchStatus
    {
        Scheduled,
        Live,
        Finished,
        Postponed
    }
}
