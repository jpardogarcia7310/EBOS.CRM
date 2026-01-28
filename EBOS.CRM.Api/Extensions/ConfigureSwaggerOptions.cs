using System.Reflection;
using EBOS.CRM.Api.Swagger;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EBOS.CRM.Api.Extensions;

public sealed class ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
    : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider = provider 
        ?? throw new ArgumentNullException(nameof(provider));

    public void Configure(SwaggerGenOptions options)
    {
        // 1. Create a SwaggerDoc for each detected version
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, new OpenApiInfo
            {
                Title = "EBOS.CRM API",
                Version = description.ApiVersion.ToString(),
                Description = $"Documentación de la API EBOS.CRM (versión {description.ApiVersion})"
            });
        }

        // 2. Include XML comments if they exist
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            options.IncludeXmlComments(xmlPath);
        }

        // 3. Custom filters (one-time, no duplicates)
        options.SchemaFilter<ValidationProblemDetailsSchemaFilter>();
        options.OperationFilter<ValidationProblemDetailsOperationFilter>();
        options.OperationFilter<ErrorResponsesOperationFilter>();

        // 4. Optional diagnostic filter (you can remove it when you no longer need it)
        options.OperationFilter<DebugGroupNameOperationFilter>();

        // 5. Include only the operations whose GroupName matches the document
        options.DocInclusionPredicate((docName, apiDesc) =>
            string.Equals(apiDesc.GroupName, docName, StringComparison.OrdinalIgnoreCase));

        // 6. Group by version + driver (optional, but very useful)
        options.TagActionsBy(apiDesc =>
        {
            var controller = apiDesc.ActionDescriptor.RouteValues["controller"] ?? "Default";
            var group = apiDesc.GroupName?.ToUpperInvariant();
            return group is null ? new[] { controller } : new[] { $"{group} - {controller}" };
        });
    }
}
