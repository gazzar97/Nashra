using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SportsData.Modules.Matches.Application.Matches.GetMatches;
using SportsData.Modules.Matches.Application.Matches.Queries;
using SportsData.Shared;

namespace SportsData.Modules.Matches.Presentation
{
    public class MatchesEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/matches").WithTags("Matches");

            group.MapGet("/", async (ISender sender) =>
            {
                var result = await sender.Send(new GetMatchesQuery());

                return result.ToHttpResult();
            });
            group.MapGet("/getMatchById", async (ISender sender,int matchId) =>
            {
                var result = await sender.Send(new GetMatchByIdQuery() { matchId = matchId });

                return result.ToHttpResult();
            });
        }
    }
}
