using EBOS.CRM.Api.Swagger;

namespace EBOS.CRM.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IApplicationBuilder UseApiErrorHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ErrorHandlingMiddleware>();
    }

    public static void AddErrorResponses(this SwaggerGenOptions options)
    {
        options.OperationFilter<ErrorResponsesOperationFilter>();
    }
}

