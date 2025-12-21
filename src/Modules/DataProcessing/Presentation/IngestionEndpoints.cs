using Carter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SportsData.Modules.DataProcessing.Dtos;
using SportsData.Modules.DataProcessing.Services;
using Asp.Versioning;

namespace SportsData.Modules.DataProcessing.Presentation
{
    public class IngestionEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var apiVersionSet = app.NewApiVersionSet()
                .HasApiVersion(new Asp.Versioning.ApiVersion(1))
                .ReportApiVersions()
                .Build();

            var group = app.MapGroup("api/v{version:apiVersion}/ingestion")
                .WithTags("Ingestion")
                .WithApiVersionSet(apiVersionSet)
                .MapToApiVersion(1);

            group.MapPost("leagues", async (IIngestionService ingestionService, List<ExternalLeagueDto> leagues) =>
            {
                await ingestionService.ImportLeaguesAsync(leagues);
                return Results.Ok("Ingestion started");
            });
        }
    }
}
