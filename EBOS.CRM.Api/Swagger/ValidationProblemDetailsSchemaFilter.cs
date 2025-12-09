using Microsoft.AspNetCore.Mvc;// si usas ValidationProblemDetails
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EBOS.CRM.Api.Swagger;

public class ValidationProblemDetailsSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema == null || context == null) return;

        if (context.Type == typeof(ValidationProblemDetails))
        {
            schema.Description ??= "ValidationProblemDetails with standard RFC7807 fields and an optional errorsDetailed extension.";

            var errorsDetailedExample = new OpenApiObject
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

            schema.Extensions["x-errors-detailed"] = errorsDetailedExample;
        }
    }
}