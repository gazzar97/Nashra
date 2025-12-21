using Carter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MediatR;
using SportsData.Shared;
using SportsData.Modules.Matches.Application.Matches.GetMatches;

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

                if (!result.IsSuccess)
                {
                    return Results.BadRequest(Envelope<List<MatchDto>>.Failure(result.Errors));
                }

                return Results.Ok(Envelope<List<MatchDto>>.Success(result.Value!));
            });
        }
    }
}
