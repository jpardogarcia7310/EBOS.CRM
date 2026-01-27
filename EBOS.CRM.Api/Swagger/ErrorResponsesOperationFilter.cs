using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EBOS.CRM.Api.Swagger;

public class ErrorResponsesOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // 400 Validation error
        operation.Responses["400"] = new OpenApiResponse
        {
            Description = "Validation error",
            Content = new Dictionary<string, OpenApiMediaType>
            {
                ["application/problem+json"] = new OpenApiMediaType
                {
                    Schema = context.SchemaGenerator.GenerateSchema(typeof(ValidationProblemDetails), context.SchemaRepository)
                }
            }
        };

        // 404 Not found
        operation.Responses["404"] = new OpenApiResponse
        {
            Description = "Resource not found",
            Content = new Dictionary<string, OpenApiMediaType>
            {
                ["application/problem+json"] = new OpenApiMediaType
                {
                    Schema = context.SchemaGenerator.GenerateSchema(typeof(ProblemDetails), context.SchemaRepository)
                }
            }
        };

        // 500 Internal server error
        operation.Responses["500"] = new OpenApiResponse
        {
            Description = "Unexpected error",
            Content = new Dictionary<string, OpenApiMediaType>
            {
                ["application/problem+json"] = new OpenApiMediaType
                {
                    Schema = context.SchemaGenerator.GenerateSchema(typeof(ProblemDetails), context.SchemaRepository)
                }
            }
        };
    }
}