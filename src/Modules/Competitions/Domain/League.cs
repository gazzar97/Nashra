using SportsData.Shared;

namespace SportsData.Modules.Competitions.Domain
{
    public class League : Entity
    {
        public string Name { get; private set; }
        public string Country { get; private set; }
        public string LogoUrl { get; private set; }

        private League() { } // EF Core

        public League(string name, string country, string logoUrl)
        {
            Name = name;
            Country = country;
            LogoUrl = logoUrl;
        }

        public static League Create(string name, string country, string logoUrl)
        {
            // Validation logic here
            return new League(name, country, logoUrl);
        }
    }
}
