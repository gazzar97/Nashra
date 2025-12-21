using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SportsData.Modules.Competitions.Application.Leagues.GetLeagues;
using SportsData.Shared;

namespace SportsData.Modules.Competitions.Presentation
{
    public class CompetitionsEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/competitions").WithTags("Competitions");

            group.MapGet("leagues", async (ISender sender) =>
            {
                var result = await sender.Send(new GetLeaguesQuery());
                return result.ToHttpResult();

            });
        }
    }
}
