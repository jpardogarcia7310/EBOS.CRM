using EBOS.CRM.Api.Swagger;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace EBOS.CRM.Api.Extensions;

public static class SwaggerConfig
{
    public static void Configure(SwaggerGenOptions options)
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "EBOS.CRM API",
            Version = "v1"
        });
        options.SwaggerDoc("v2", new OpenApiInfo
        {
            Title = "EBOS.CRM API",
            Version = "v2"
        });

        // Incluir comentarios XML si existen
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            options.IncludeXmlComments(xmlPath);
        }

        // Filtros personalizados
        options.SchemaFilter<ValidationProblemDetailsSchemaFilter>();
        options.OperationFilter<ValidationProblemDetailsOperationFilter>();

        // Respuestas de error comunes
        options.AddErrorResponses();
    }

    public static void DefaultVersion(IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
        });
    }
}