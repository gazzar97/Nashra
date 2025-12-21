using SportsData.Shared;

namespace SportsData.Modules.Matches.Domain
{
    public class MatchStatistics : Entity
    {
        public Guid MatchId { get; private set; }
        public int PossessionHome { get; private set; }
        public int PossessionAway { get; private set; }
        public int ShotsHome { get; private set; }
        public int ShotsAway { get; private set; }
        public int CornersHome { get; private set; }
        public int CornersAway { get; private set; }
        public int FoulsHome { get; private set; }
        public int FoulsAway { get; private set; }
        public int YellowCardsHome { get; private set; }
        public int YellowCardsAway { get; private set; }
        public int RedCardsHome { get; private set; }
        public int RedCardsAway { get; private set; }

        private MatchStatistics() { }

        public MatchStatistics(Guid matchId, int possessionHome, int possessionAway, int shotsHome, int shotsAway, 
            int cornersHome, int cornersAway, int foulsHome, int foulsAway, 
            int yellowCardsHome, int yellowCardsAway, int redCardsHome, int redCardsAway)
        {
            MatchId = matchId;
            PossessionHome = possessionHome;
            PossessionAway = possessionAway;
            ShotsHome = shotsHome;
            ShotsAway = shotsAway;
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
