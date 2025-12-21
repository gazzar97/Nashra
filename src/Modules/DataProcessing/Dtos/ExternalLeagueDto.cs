namespace SportsData.Modules.DataProcessing.Dtos
{
    public class ExternalLeagueDto
    {
        public string Name { get; set; }
        public string Country { get; set; }
        public string LogoUrl { get; set; }
        public int Tier { get; set; }
        public string SeasonStart { get; set; }
        public string SeasonEnd { get; set; }
        public List<ExternalSeasonDto> Seasons { get; set; }
    }

    public class ExternalSeasonDto
    {
        public string Year { get; set; }
        public List<ExternalTeamDto> Teams { get; set; }
    }

    public class ExternalTeamDto
    {
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Code { get; set; }
        public string LogoUrl { get; set; }
        public int? FoundedYear { get; set; }
        public string Stadium { get; set; }
    }
}
