using SportsData.Shared;

namespace SportsData.Modules.Competitions.Domain
{
    public class Team : Entity
    {
        public string Name { get; private set; }
        public string ShortName { get; private set; }
        public string Code { get; private set; } // e.g. "AHL" for Al Ahly
        public string LogoUrl { get; private set; }
        public int? FoundedYear { get; private set; }
        public string Stadium { get; private set; }
        
        // Foreign Key link to League is now managed via LeagueTeamSeason

        private Team() { }

        public Team(string name, string shortName, string code, string logoUrl, int? foundedYear, string stadium)
        {
            Name = name;
            ShortName = shortName;
            Code = code;
            LogoUrl = logoUrl;
            FoundedYear = foundedYear;
            Stadium = stadium;
        }
    }
}
