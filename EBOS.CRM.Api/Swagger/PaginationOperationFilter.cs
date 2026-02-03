using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EBOS.CRM.Api.Swagger;

public sealed class PaginationOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Parameters is not null)
        {
            foreach (var parameter in operation.Parameters)
            {
                if (string.Equals(parameter.Name, "pageNumber", StringComparison.OrdinalIgnoreCase))
                {
                    parameter.Description ??= "Page number (1-based).";
                }
                else if (string.Equals(parameter.Name, "pageSize", StringComparison.OrdinalIgnoreCase))
                {
                    parameter.Description ??= "Page size (capped by configured MaxPageSize).";
                }
            }
        }

        if (operation.Responses.TryGetValue("200", out var response))
        {
            response.Headers ??= new Dictionary<string, OpenApiHeader>();
            response.Headers["X-Total-Count"] = new OpenApiHeader
            {
                Description = "Total items available before pagination.",
                Schema = new OpenApiSchema { Type = "integer", Format = "int32" }
            };
        }

        if (!operation.Responses.ContainsKey("400"))
        {
            var schema = context.SchemaGenerator.GenerateSchema(typeof(ProblemDetails), context.SchemaRepository);
            operation.Responses["400"] = new OpenApiResponse
            {
                Description = "Bad Request",
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/json"] = new OpenApiMediaType { Schema = schema }
                }
            };
        }
    }
}
