using SportsData.Shared;

namespace SportsData.Modules.Matches.Domain
{
    /// <summary>
    /// Represents comprehensive statistics for a match.
    /// Contains all statistical data for both home and away teams.
    /// </summary>
    public class MatchStatistics : Entity
    {
        public Guid MatchId { get; private set; }
        
        // Ball Control
        public int PossessionHome { get; private set; }
        public int PossessionAway { get; private set; }
        
        // Attacking
        public int ShotsHome { get; private set; }
        public int ShotsAway { get; private set; }
        public int ShotsOnTargetHome { get; private set; }
        public int ShotsOnTargetAway { get; private set; }
        
        // Set Pieces
        public int CornersHome { get; private set; }
        public int CornersAway { get; private set; }
        
        // Defense
        public int FoulsHome { get; private set; }
        public int FoulsAway { get; private set; }
        
        // Discipline
        public int YellowCardsHome { get; private set; }
        public int YellowCardsAway { get; private set; }
        public int RedCardsHome { get; private set; }
        public int RedCardsAway { get; private set; }

        // EF Core constructor
        private MatchStatistics() { }

        private MatchStatistics(
            Guid matchId, 
            int possessionHome, 
            int possessionAway, 
            int shotsHome, 
            int shotsAway,
            int shotsOnTargetHome,
            int shotsOnTargetAway,
            int cornersHome, 
            int cornersAway, 
            int foulsHome, 
            int foulsAway, 
            int yellowCardsHome, 
            int yellowCardsAway, 
            int redCardsHome, 
            int redCardsAway)
        {
            MatchId = matchId;
            PossessionHome = possessionHome;
            PossessionAway = possessionAway;
            ShotsHome = shotsHome;
            ShotsAway = shotsAway;
            ShotsOnTargetHome = shotsOnTargetHome;
            ShotsOnTargetAway = shotsOnTargetAway;
            CornersHome = cornersHome;
            CornersAway = cornersAway;
            FoulsHome = foulsHome;
            FoulsAway = foulsAway;
            YellowCardsHome = yellowCardsHome;
            YellowCardsAway = yellowCardsAway;
            RedCardsHome = redCardsHome;
            RedCardsAway = redCardsAway;
        }

        /// <summary>
        /// Creates new match statistics with validation.
        /// </summary>
        public static MatchStatistics Create(
            Guid matchId,
            int possessionHome,
            int possessionAway,
            int shotsHome,
            int shotsAway,
            int shotsOnTargetHome,
            int shotsOnTargetAway,
            int cornersHome,
            int cornersAway,
            int foulsHome,
            int foulsAway,
            int yellowCardsHome,
            int yellowCardsAway,
            int redCardsHome,
            int redCardsAway)
        {
            // Business rule: Possession must total approximately 100
            if (possessionHome < 0 || possessionAway < 0 || possessionHome + possessionAway != 100)
            {
                throw new ArgumentException("Possession percentages must be non-negative and total 100.");
            }

            // Business rule: All count fields must be non-negative
            if (shotsHome < 0 || shotsAway < 0)
                throw new ArgumentException("Shots cannot be negative.");

            if (shotsOnTargetHome < 0 || shotsOnTargetAway < 0)
                throw new ArgumentException("Shots on target cannot be negative.");

            if (shotsOnTargetHome > shotsHome)
                throw new ArgumentException("Home shots on target cannot exceed total home shots.");

            if (shotsOnTargetAway > shotsAway)
                throw new ArgumentException("Away shots on target cannot exceed total away shots.");

            if (cornersHome < 0 || cornersAway < 0)
                throw new ArgumentException("Corners cannot be negative.");

            if (foulsHome < 0 || foulsAway < 0)
                throw new ArgumentException("Fouls cannot be negative.");

            if (yellowCardsHome < 0 || yellowCardsAway < 0)
                throw new ArgumentException("Yellow cards cannot be negative.");

            if (redCardsHome < 0 || redCardsAway < 0)
                throw new ArgumentException("Red cards cannot be negative.");

            return new MatchStatistics(
                matchId,
                possessionHome,
                possessionAway,
                shotsHome,
                shotsAway,
                shotsOnTargetHome,
                shotsOnTargetAway,
                cornersHome,
                cornersAway,
                foulsHome,
                foulsAway,
                yellowCardsHome,
                yellowCardsAway,
                redCardsHome,
                redCardsAway);
        }

        /// <summary>
        /// Updates the match statistics with validation.
        /// </summary>
        public void UpdateStatistics(
            int possessionHome,
            int possessionAway,
            int shotsHome,
            int shotsAway,
            int shotsOnTargetHome,
            int shotsOnTargetAway,
            int cornersHome,
            int cornersAway,
            int foulsHome,
            int foulsAway,
            int yellowCardsHome,
            int yellowCardsAway,
            int redCardsHome,
            int redCardsAway)
        {
            // Apply same validation rules
            if (possessionHome < 0 || possessionAway < 0 || possessionHome + possessionAway != 100)
                throw new ArgumentException("Possession percentages must be non-negative and total 100.");

            if (shotsHome < 0 || shotsAway < 0 || shotsOnTargetHome < 0 || shotsOnTargetAway < 0)
                throw new ArgumentException("Shot statistics cannot be negative.");

            if (shotsOnTargetHome > shotsHome || shotsOnTargetAway > shotsAway)
                throw new ArgumentException("Shots on target cannot exceed total shots.");

            if (cornersHome < 0 || cornersAway < 0 || foulsHome < 0 || foulsAway < 0)
                throw new ArgumentException("Corners and fouls cannot be negative.");

            if (yellowCardsHome < 0 || yellowCardsAway < 0 || redCardsHome < 0 || redCardsAway < 0)
                throw new ArgumentException("Cards cannot be negative.");

            PossessionHome = possessionHome;
            PossessionAway = possessionAway;
            ShotsHome = shotsHome;
            ShotsAway = shotsAway;
            ShotsOnTargetHome = shotsOnTargetHome;
            ShotsOnTargetAway = shotsOnTargetAway;
            CornersHome = cornersHome;
            CornersAway = cornersAway;
            FoulsHome = foulsHome;
            FoulsAway = foulsAway;
            YellowCardsHome = yellowCardsHome;
            YellowCardsAway = yellowCardsAway;
            RedCardsHome = redCardsHome;
            RedCardsAway = redCardsAway;
        }
    }
}
