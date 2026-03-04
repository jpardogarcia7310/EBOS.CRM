using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Options;

namespace EBOS.CRM.Api.Infrastructure;

public sealed class CrmProblemDetailsFactory(IOptions<ApiBehaviorOptions> options) : ProblemDetailsFactory
{
    private readonly ApiBehaviorOptions _options = options.Value;

    public override ProblemDetails CreateProblemDetails(
        HttpContext httpContext,
        int? statusCode = null,
        string? title = null,
        string? type = null,
        string? detail = null,
        string? instance = null)
    {
        var problemDetails = new ProblemDetails
        {
            Status = statusCode ?? StatusCodes.Status500InternalServerError,
            Title = title,
            Type = type,
            Detail = detail,
            Instance = instance
        };

        ApplyDefaults(httpContext, problemDetails, statusCode);
        return problemDetails;
    }

    public override ValidationProblemDetails CreateValidationProblemDetails(
        HttpContext httpContext,
        ModelStateDictionary modelStateDictionary,
        int? statusCode = null,
        string? title = null,
        string? type = null,
        string? detail = null,
        string? instance = null)
    {
        statusCode ??= StatusCodes.Status400BadRequest;

        var problemDetails = new ValidationProblemDetails(modelStateDictionary)
        {
            Status = statusCode,
            Type = type,
            Detail = detail,
            Instance = instance
        };

        if (!string.IsNullOrEmpty(title))
        {
            problemDetails.Title = title;
        }

        ApplyDefaults(httpContext, problemDetails, statusCode);
        return problemDetails;
    }

    private void ApplyDefaults(HttpContext httpContext, ProblemDetails problemDetails, int? statusCode)
    {
        if (_options.ClientErrorMapping.TryGetValue(statusCode ?? problemDetails.Status ?? 500, out var mapping))
        {
            problemDetails.Title ??= mapping.Title;
            problemDetails.Type ??= mapping.Link;
        }

        problemDetails.Instance ??= httpContext.Request.Path;

        problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;
    }
}
