using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EBOS.CRM.Api.Swagger;

public sealed class ValidationProblemDetailsOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (!operation.Responses.TryGetValue("400", out var response))
            return;

        var example = new OpenApiObject
        {
            ["type"] = new OpenApiString("https://tools.ietf.org/html/rfc7231#section-6.5.1"),
            ["title"] = new OpenApiString("One or more validation errors occurred."),
            ["status"] = new OpenApiInteger(400),
            ["errors"] = new OpenApiObject
            {
                ["name"] = new OpenApiArray { new OpenApiString("The Name field is required.") },
                ["iso31661A2Code"] = new OpenApiArray { new
                    OpenApiString("The Iso31661A2Code field must have 2 characters.") }
            },
            ["errorsDetailed"] = new OpenApiObject
            {
                ["name"] = new OpenApiArray
                {
                    new OpenApiObject
                    {
                        ["message"] = new OpenApiString("The Name field is required."),
                        ["code"] = new OpenApiString("VAL_NAME_REQUIRED")
                    }
                },
                ["iso31661A2Code"] = new OpenApiArray
                {
                    new OpenApiObject
                    {
                        ["message"] = new OpenApiString("The Iso31661A2Code field must have 2 characters."),
                        ["code"] = new OpenApiString("VAL_ISOA2_LENGTH")
                    }
                }
            }
        };

        response.Content ??= new Dictionary<string, OpenApiMediaType>();
        response.Content["application/problem+json"] = new OpenApiMediaType
        {
            Example = example
        };
    }
}