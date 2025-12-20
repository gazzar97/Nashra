using SportsData.Shared;

namespace SportsData.Modules.Competitions.Domain
{
    public class Team : Entity
    {
        public string Name { get; private set; }
        public string Code { get; private set; } // e.g. "AHL" for Al Ahly
        public string LogoUrl { get; private set; }
        
        // Foreign Key link to League could be managed here or via a join entity for Many-to-Many
        // For MVP, simple association
        public Guid CurrentLeagueId { get; private set; }

        private Team() { }

        public Team(string name, string code, string logoUrl, Guid currentLeagueId)
        {
            Name = name;
            Code = code;
            LogoUrl = logoUrl;
            CurrentLeagueId = currentLeagueId;
        }
    }
}
