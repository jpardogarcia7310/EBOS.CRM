using System.Security.Cryptography;
using System.Text;

namespace EBOS.CRM.Api.Middleware;

public class TenantRequirementMiddleware(RequestDelegate next)
{
    private static readonly PathString ApiBasePath = new("/api");

    public async Task Invoke(HttpContext context, ITenantContext tenantContext)
    {
        if (!context.Request.Path.StartsWithSegments(ApiBasePath) ||
            context.Request.Path.StartsWithSegments("/swagger") ||
            context.Request.Method == HttpMethods.Options)
        {
            await next(context);
            return;
        }

        if (tenantContext.TenantId <= 0)
        {
            var errors = new Dictionary<string, string[]>
            {
                ["tenantId"] = ["TenantId is required."]
            };

            var errorsDetailed = new Dictionary<string, object[]>
            {
                ["tenantId"] =
                [
                    new
                    {
                        message = "TenantId is required.",
                        code = ComputeStableCode("tenantId", "TenantId is required.")
                    }
                ]
            };

            var problem = new ValidationProblemDetails(errors)
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "One or more validation errors occurred.",
                Status = StatusCodes.Status400BadRequest,
                Instance = context.Request.Path,
                Extensions =
                {
                    ["errorsDetailed"] = errorsDetailed
                }
            };

            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsJsonAsync(problem);
            return;
        }

        await next(context);
    }

    private static string ComputeStableCode(string key, string message)
    {
        var payload = $"{key}|{message}";
        var bytes = Encoding.UTF8.GetBytes(payload);
        var hash = SHA256.HashData(bytes);
        var hex = Convert.ToHexString(hash);
        return $"VAL_{hex.Substring(0, 12)}";
    }
}
