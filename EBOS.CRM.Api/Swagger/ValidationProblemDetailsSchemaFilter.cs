using System.Diagnostics.CodeAnalysis;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace EBOS.CRM.Api.Swagger;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public sealed class ValidationProblemDetailsSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type != typeof(ValidationProblemDetails))
            return;

        schema.Description ??=
            "ValidationProblemDetails with standard RFC7807 fields and an optional errorsDetailed extension.";

        schema.Extensions["x-errors-detailed"] = new OpenApiObject
        {
            ["name"] = new OpenApiArray
            {
                new OpenApiObject
                {
                    ["message"] = new OpenApiString("El campo Name es obligatorio."),
                    ["code"] = new OpenApiString("VAL_NAME_REQUIRED")
                }
            },
            ["iso31661A2Code"] = new OpenApiArray
            {
                new OpenApiObject
                {
                    ["message"] = new OpenApiString("El campo Iso31661A2Code debe tener 2 caracteres."),
                    ["code"] = new OpenApiString("VAL_ISOA2_LENGTH")
                }
            }
        };
    }
}
