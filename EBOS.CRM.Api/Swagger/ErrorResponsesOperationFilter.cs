using Microsoft.OpenApi.Models;

namespace EBOS.CRM.Api.Swagger;

public sealed class ErrorResponsesOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var schemaGenerator = context.SchemaGenerator;
        var schemaRepository = context.SchemaRepository;

        operation.Responses["400"] = new OpenApiResponse
        {
            Description = "Validation error",
            Content = new Dictionary<string, OpenApiMediaType>
            {
                ["application/problem+json"] = new OpenApiMediaType
                {
                    Schema = schemaGenerator.GenerateSchema(typeof(ValidationProblemDetails), schemaRepository)
                }
            }
        };

        operation.Responses["404"] = new OpenApiResponse
        {
            Description = "Resource not found",
            Content = new Dictionary<string, OpenApiMediaType>
            {
                ["application/problem+json"] = new OpenApiMediaType
                {
                    Schema = schemaGenerator.GenerateSchema(typeof(ProblemDetails), schemaRepository)
                }
            }
        };

        operation.Responses["500"] = new OpenApiResponse
        {
            Description = "Unexpected error",
            Content = new Dictionary<string, OpenApiMediaType>
            {
                ["application/problem+json"] = new OpenApiMediaType
                {
                    Schema = schemaGenerator.GenerateSchema(typeof(ProblemDetails), schemaRepository)
                }
            }
        };
    }
}
