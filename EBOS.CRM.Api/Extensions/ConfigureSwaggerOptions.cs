using EBOS.CRM.Api.Swagger;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace EBOS.CRM.Api.Extensions;

// Usamos constructor principal (primary constructor) como solicitaste
public sealed class ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider = provider ?? throw new ArgumentNullException(nameof(provider));

    public void Configure(SwaggerGenOptions options)
    {
        // Crear un SwaggerDoc por cada versión detectada
        foreach (var description in _provider.ApiVersionDescriptions)
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

        // --- BLOQUE DE DIAGNÓSTICO ---
        // Registrar temporalmente el filtro de debug para exponer x-groupName en cada operación
        // (quítalo cuando ya no lo necesites)
        options.OperationFilter<DebugGroupNameOperationFilter>();
        // --- FIN BLOQUE DE DIAGNÓSTICO ---

        // INCLUSION: usar GroupName proporcionado por VersionedApiExplorer
        // Esto asegura que cada SwaggerDoc solo incluya las operaciones asignadas a esa versión
        options.DocInclusionPredicate((docName, apiDesc) =>
        {
            // apiDesc.GroupName es establecido por VersionedApiExplorer y coincide con docName
            // Si GroupName es null, excluimos la operación
            return string.Equals(apiDesc.GroupName, docName, StringComparison.OrdinalIgnoreCase);
        });

        // TAGS por versión y controlador (opcional)
        options.TagActionsBy(apiDesc =>
        {
            var controller = apiDesc.ActionDescriptor.RouteValues["controller"] ?? "Default";
            var group = apiDesc.GroupName?.ToUpperInvariant();
            return group is null ? new[] { controller } : [$"{group} - {controller}"];
        });
    }
}