using Microsoft.AspNetCore.Mvc;

namespace EBOS.CRM.Api.Extensions;

public static class SwaggerConfig
{
    public static void ApiVersioning(IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.ReportApiVersions = true;
        });
        services.AddVersionedApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV"; // genera grupos v1, v2...
            options.SubstituteApiVersionInUrl = true;
        });
    }
}
