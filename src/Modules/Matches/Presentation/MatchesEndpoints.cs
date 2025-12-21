using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SportsData.Modules.Matches.Application.Matches.GetMatches;
using SportsData.Modules.Matches.Application.Matches.Queries;
using SportsData.Shared;
using Asp.Versioning;

namespace SportsData.Modules.Matches.Presentation
{
    public class MatchesEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var apiVersionSet = app.NewApiVersionSet()
                .HasApiVersion(new Asp.Versioning.ApiVersion(1))
                .ReportApiVersions()
                .Build();

            var group = app.MapGroup("api/v{version:apiVersion}/matches")
                .WithTags("Matches")
                .WithApiVersionSet(apiVersionSet)
                .MapToApiVersion(1);

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

            group.MapGet("/{id}/statistics", (Guid id) => Results.Ok($"Statistics for Match {id}"));
        }
    }
}
