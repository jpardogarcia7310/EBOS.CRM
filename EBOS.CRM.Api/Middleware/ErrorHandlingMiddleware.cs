using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace EBOS.CRM.Api.Middleware;

public class ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/swagger") ||
            context.Request.Path.StartsWithSegments("/swagger/v1/swagger.json"))
        {
            await next(context);
            return;
        }

        try
        {
            await next(context);
        }
        catch (ValidationException vex)
        {
            logger.LogWarning(vex, "Validation error");

            var errors = vex.Errors
                .GroupBy(e => e.PropertyName ?? string.Empty)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage ?? "Invalid value").ToArray()
                );

            var errorsDetailed = vex.Errors
                .GroupBy(e => e.PropertyName ?? string.Empty)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => new
                    {
                        message = e.ErrorMessage ?? "Invalid value",
                        code = string.IsNullOrWhiteSpace(e.ErrorCode)
                            ? ComputeStableCode(e.PropertyName ?? string.Empty, e.ErrorMessage ?? "Invalid value")
                            : e.ErrorCode
                    }).ToArray()
                );

            var problem = new ValidationProblemDetails(errors)
            {
                Title = "One or more validation errors occurred.",
                Status = StatusCodes.Status400BadRequest,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Extensions =
                {
                    ["errorsDetailed"] = errorsDetailed
                }
            };

            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(problem);
        }
        catch (KeyNotFoundException knf)
        {
            logger.LogWarning(knf, "Not found");

            var problem = new ProblemDetails
            {
                Title = "Resource not found",
                Detail = knf.Message,
                Status = StatusCodes.Status404NotFound
            };

            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsJsonAsync(problem);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");

            var problem = new ProblemDetails
            {
                Title = "An unexpected error occurred.",
                Detail = "Please contact support if the problem persists.",
                Status = StatusCodes.Status500InternalServerError
            };

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(problem);
        }
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

