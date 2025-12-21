using SportsData.Modules.Competitions.Application.Leagues.GetLeagues;
using SportsData.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsData.Modules.Competitions.Application.Leagues.Services
{
    public interface ILeagueService 
    {
        Task<PagedList<LeagueDto>> GetLeagues(string Country, int Page, int PageSize);
    }
}
