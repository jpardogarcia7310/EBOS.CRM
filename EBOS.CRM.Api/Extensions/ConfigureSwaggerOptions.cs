using EBOS.CRM.Api.Swagger;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;


namespace EBOS.CRM.Api.Extensions;

public sealed class ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) : IConfigureOptions<SwaggerGenOptions>
{
    public void Configure(SwaggerGenOptions options)
    {
        // Un doc por cada versión detectada
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, new OpenApiInfo
            {
                Title = "EBOS.CRM API",
                Version = description.ApiVersion.ToString(),
                Description = $"Documentación de la API EBOS.CRM (versión {description.ApiVersion})"
            });
        }

        // XML comments si existen
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            options.IncludeXmlComments(xmlPath);
        }

        // Filtros personalizados y respuestas comunes
        options.SchemaFilter<ValidationProblemDetailsSchemaFilter>();
        options.OperationFilter<ValidationProblemDetailsOperationFilter>();
        options.AddErrorResponses();

        // INCLUSION: opción A (activa SOLO una de las dos)
        options.DocInclusionPredicate((version, apiDesc) =>
        {
            if (!apiDesc.TryGetMethodInfo(out var methodInfo)) return false;
            var controllerVersions = methodInfo.DeclaringType?
                .GetCustomAttributes(true)
                .OfType<ApiVersionAttribute>()
                .SelectMany(attr => attr.Versions)
                .ToArray();

            return controllerVersions?.Any(v => $"v{v.MajorVersion}" == version) ?? false;
        });

        // TAGS por versión y controlador
        options.TagActionsBy(apiDesc =>
        {
            var controller = apiDesc.ActionDescriptor.RouteValues["controller"] ?? "Default";
            var group = apiDesc.GroupName?.ToUpperInvariant();
            return group is null ? new[] { controller } : new[] { $"{group} - {controller}" };
        });
    }
}