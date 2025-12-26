using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SportsData.Modules.ApiKeys.Application.ApiKeys.Commands;
using SportsData.Modules.ApiKeys.Application.ApiKeys.Queries;
using SportsData.Shared;
using Asp.Versioning;

namespace SportsData.Modules.ApiKeys.Presentation
{
    public class ApiKeysEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var apiVersionSet = app.NewApiVersionSet()
                .HasApiVersion(new ApiVersion(1))
                .ReportApiVersions()
                .Build();

            var group = app.MapGroup("api/v{version:apiVersion}/admin/api-keys")
                .WithTags("API Keys")
                .WithApiVersionSet(apiVersionSet)
                .MapToApiVersion(1);

            // Create API key
            group.MapPost("", async (CreateApiKeyCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return result.ToHttpResult();
            })
            .WithName("CreateApiKey")
            .WithSummary("Create a new API key")
            .WithDescription("Creates a new API key for a customer. The raw key is returned only once and must be stored securely.")
            .Produces<Envelope<CreateApiKeyResponse>>(StatusCodes.Status200OK)
            .Produces<Envelope<object>>(StatusCodes.Status400BadRequest);

            // Get all API keys
            group.MapGet("", async ([AsParameters] GetApiKeysQuery query, ISender sender) =>
            {
                var result = await sender.Send(query);
                return result.ToHttpResult();
            })
            .WithName("GetApiKeys")
            .WithSummary("Get all API keys")
            .WithDescription("Retrieves a paginated list of API keys with optional filters.")
            .Produces<Envelope<PagedList<ApiKeyDto>>>(StatusCodes.Status200OK);

            // Get API key by ID
            group.MapGet("{id}", async (Guid id, ISender sender) =>
            {
                var query = new GetApiKeyByIdQuery { Id = id };
                var result = await sender.Send(query);
                return result.ToHttpResult();
            })
            .WithName("GetApiKeyById")
            .WithSummary("Get API key by ID")
            .WithDescription("Retrieves a single API key by its ID.")
            .Produces<Envelope<ApiKeyDto>>(StatusCodes.Status200OK)
            .Produces<Envelope<object>>(StatusCodes.Status404NotFound);

            // Revoke API key
            group.MapDelete("{id}", async (Guid id, ISender sender) =>
            {
                var command = new RevokeApiKeyCommand { Id = id };
                var result = await sender.Send(command);
                return result.ToHttpResult();
            })
            .WithName("RevokeApiKey")
            .WithSummary("Revoke an API key")
            .WithDescription("Revokes an API key, making it unusable for future requests.")
            .Produces<Envelope<bool>>(StatusCodes.Status200OK)
            .Produces<Envelope<object>>(StatusCodes.Status404NotFound);

            // Rotate API key
            group.MapPost("{id}/rotate", async (Guid id, ISender sender) =>
            {
                var command = new RotateApiKeyCommand { Id = id };
                var result = await sender.Send(command);
                return result.ToHttpResult();
            })
            .WithName("RotateApiKey")
            .WithSummary("Rotate an API key")
            .WithDescription("Revokes the old API key and creates a new one with the same settings. The new raw key is returned only once.")
            .Produces<Envelope<CreateApiKeyResponse>>(StatusCodes.Status200OK)
            .Produces<Envelope<object>>(StatusCodes.Status404NotFound);
        }
    }
}
