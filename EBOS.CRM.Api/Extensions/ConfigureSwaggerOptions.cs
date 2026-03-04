using System.Reflection;
using EBOS.CRM.Api.Swagger;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

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
                Description = $"EBOS.CRM API documentation (version {description.ApiVersion})"
            });
        }

        // Prevent schema ID collisions for types with the same name in different namespaces.
        options.CustomSchemaIds(type => type.FullName?.Replace("+", ".") ?? type.Name);

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
        options.OperationFilter<PaginationOperationFilter>();

        // 4. Optional diagnostic filter (you can remove it when you no longer need it)
        options.OperationFilter<DebugGroupNameOperationFilter>();

        var securityScheme = new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "JWT Authorization header using the Bearer scheme."
        };

        options.AddSecurityDefinition("Bearer", securityScheme);
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });

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

