using SportsData.Shared;

namespace SportsData.Modules.Competitions.Domain
{
    public class League : Entity
    {
        public string Name { get; private set; }
        public string Country { get; private set; }
        public string LogoUrl { get; private set; }
        public int Tier { get; private set; }
        public string SeasonStart { get; private set; } // e.g. "August"
        public string SeasonEnd { get; private set; }   // e.g. "May"

        private League() { } // EF Core

        public League(string name, string country, string logoUrl, int tier, string seasonStart, string seasonEnd)
        {
            Name = name;
            Country = country;
            LogoUrl = logoUrl;
            Tier = tier;
            SeasonStart = seasonStart;
            SeasonEnd = seasonEnd;
        }

        public static League Create(string name, string country, string logoUrl, int tier, string seasonStart, string seasonEnd)
        {
            // Validation logic here
            return new League(name, country, logoUrl, tier, seasonStart, seasonEnd);
        }
    }
}
