using SportsData.Shared;

namespace SportsData.Modules.Competitions.Domain
{
    /// <summary>
    /// Represents a football match within a season.
    /// Core aggregate for match results, fixtures, and live scores.
    /// </summary>
    public class Match : Entity
    {
        public Guid SeasonId { get; private set; }
        public Guid HomeTeamId { get; private set; }
        public Guid AwayTeamId { get; private set; }
        public DateTime MatchDate { get; private set; }
        public MatchStatus Status { get; private set; }
        public int? HomeScore { get; private set; }
        public int? AwayScore { get; private set; }
        public string Venue { get; private set; }

        // EF Core constructor
        private Match() { }

        private Match(
            Guid seasonId,
            Guid homeTeamId,
            Guid awayTeamId,
            DateTime matchDate,
            string venue,
            MatchStatus status = MatchStatus.Scheduled)
        {
            SeasonId = seasonId;
            HomeTeamId = homeTeamId;
            AwayTeamId = awayTeamId;
            MatchDate = matchDate;
            Venue = venue;
            Status = status;
            HomeScore = null;
            AwayScore = null;
        }

        /// <summary>
        /// Creates a new scheduled match.
        /// </summary>
        public static Match Create(
            Guid seasonId,
            Guid homeTeamId,
            Guid awayTeamId,
            DateTime matchDate,
            string venue)
        {
            // Business rule: Home team cannot be the same as away team
            if (homeTeamId == awayTeamId)
            {
                throw new InvalidOperationException("Home team and away team cannot be the same.");
            }

            // Business rule: Venue is required
            if (string.IsNullOrWhiteSpace(venue))
            {
                throw new ArgumentException("Venue is required.", nameof(venue));
            }

            return new Match(seasonId, homeTeamId, awayTeamId, matchDate, venue);
        }

        /// <summary>
        /// Updates the match score and sets status to Finished.
        /// </summary>
        public void SetFinalScore(int homeScore, int awayScore)
        {
            if (homeScore < 0 || awayScore < 0)
            {
                throw new ArgumentException("Scores cannot be negative.");
            }

            HomeScore = homeScore;
            AwayScore = awayScore;
            Status = MatchStatus.Finished;
        }

        /// <summary>
        /// Updates the match score during live play.
        /// </summary>
        public void UpdateLiveScore(int homeScore, int awayScore)
        {
            if (homeScore < 0 || awayScore < 0)
            {
                throw new ArgumentException("Scores cannot be negative.");
            }

            HomeScore = homeScore;
            AwayScore = awayScore;
            Status = MatchStatus.Live;
        }

        /// <summary>
        /// Marks the match as postponed.
        /// </summary>
        public void Postpone()
        {
            Status = MatchStatus.Postponed;
            HomeScore = null;
            AwayScore = null;
        }

        /// <summary>
        /// Marks the match as cancelled.
        /// </summary>
        public void Cancel()
        {
            Status = MatchStatus.Cancelled;
            HomeScore = null;
            AwayScore = null;
        }

        /// <summary>
        /// Reschedules the match to a new date.
        /// </summary>
        public void Reschedule(DateTime newMatchDate)
        {
            MatchDate = newMatchDate;
            Status = MatchStatus.Scheduled;
            HomeScore = null;
            AwayScore = null;
        }
    }
}
