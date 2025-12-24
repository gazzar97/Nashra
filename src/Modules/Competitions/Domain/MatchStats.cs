using SportsData.Shared;

namespace SportsData.Modules.Competitions.Domain
{
    /// <summary>
    /// Represents detailed statistics for a team in a specific match.
    /// Each match has two MatchStats records (one for home team, one for away team).
    /// </summary>
    public class MatchStats : Entity
    {
        public Guid MatchId { get; private set; }
        public Guid TeamId { get; private set; }
        public bool IsHome { get; private set; }

        // Ball Control
        public decimal PossessionPercentage { get; private set; }

        // Attacking
        public int Shots { get; private set; }
        public int ShotsOnTarget { get; private set; }

        // Set Pieces
        public int Corners { get; private set; }

        // Discipline
        public int YellowCards { get; private set; }
        public int RedCards { get; private set; }

        // Defense
        public int Fouls { get; private set; }

        // EF Core constructor
        private MatchStats() { }

        private MatchStats(
            Guid matchId,
            Guid teamId,
            bool isHome,
            decimal possessionPercentage,
            int shots,
            int shotsOnTarget,
            int corners,
            int yellowCards,
            int redCards,
            int fouls)
        {
            MatchId = matchId;
            TeamId = teamId;
            IsHome = isHome;
            PossessionPercentage = possessionPercentage;
            Shots = shots;
            ShotsOnTarget = shotsOnTarget;
            Corners = corners;
            YellowCards = yellowCards;
            RedCards = redCards;
            Fouls = fouls;
        }

        /// <summary>
        /// Creates new match statistics for a team.
        /// </summary>
        public static MatchStats Create(
            Guid matchId,
            Guid teamId,
            bool isHome,
            decimal possessionPercentage,
            int shots,
            int shotsOnTarget,
            int corners,
            int yellowCards,
            int redCards,
            int fouls)
        {
            // Business rule: Possession must be between 0 and 100
            if (possessionPercentage < 0 || possessionPercentage > 100)
            {
                throw new ArgumentException("Possession percentage must be between 0 and 100.", nameof(possessionPercentage));
            }

            // Business rule: All count fields must be non-negative
            if (shots < 0)
                throw new ArgumentException("Shots cannot be negative.", nameof(shots));

            if (shotsOnTarget < 0)
                throw new ArgumentException("Shots on target cannot be negative.", nameof(shotsOnTarget));

            if (shotsOnTarget > shots)
                throw new ArgumentException("Shots on target cannot exceed total shots.", nameof(shotsOnTarget));

            if (corners < 0)
                throw new ArgumentException("Corners cannot be negative.", nameof(corners));

            if (yellowCards < 0)
                throw new ArgumentException("Yellow cards cannot be negative.", nameof(yellowCards));

            if (redCards < 0)
                throw new ArgumentException("Red cards cannot be negative.", nameof(redCards));

            if (fouls < 0)
                throw new ArgumentException("Fouls cannot be negative.", nameof(fouls));

            return new MatchStats(
                matchId,
                teamId,
                isHome,
                possessionPercentage,
                shots,
                shotsOnTarget,
                corners,
                yellowCards,
                redCards,
                fouls);
        }

        /// <summary>
        /// Updates the match statistics.
        /// </summary>
        public void UpdateStats(
            decimal possessionPercentage,
            int shots,
            int shotsOnTarget,
            int corners,
            int yellowCards,
            int redCards,
            int fouls)
        {
            // Apply same validation rules
            if (possessionPercentage < 0 || possessionPercentage > 100)
                throw new ArgumentException("Possession percentage must be between 0 and 100.", nameof(possessionPercentage));

            if (shots < 0 || shotsOnTarget < 0 || shotsOnTarget > shots || 
                corners < 0 || yellowCards < 0 || redCards < 0 || fouls < 0)
                throw new ArgumentException("Invalid statistics values.");

            PossessionPercentage = possessionPercentage;
            Shots = shots;
            ShotsOnTarget = shotsOnTarget;
            Corners = corners;
            YellowCards = yellowCards;
            RedCards = redCards;
            Fouls = fouls;
        }
    }
}
