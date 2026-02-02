using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EBOS.CRM.Api.Extensions;

public static class ApiBehaviorConfig
{
    public static void Configure(ApiBehaviorOptions options)
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var simple = BuildSimpleErrors(context.ModelState);
            var detailed = BuildDetailedErrors(context.HttpContext, simple);

            var pd = new ValidationProblemDetails(simple)
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "One or more validation errors occurred.",
                Status = StatusCodes.Status400BadRequest,
                Instance = context.HttpContext.Request.Path,
                Extensions =
                {
                    ["errorsDetailed"] = detailed
                }
            };

            return new BadRequestObjectResult(pd)
            {
                ContentTypes = { "application/problem+json" }
            };
        };
    }

    private static Dictionary<string, string[]> BuildSimpleErrors(ModelStateDictionary modelState)
    {
        static string ToCamel(string s) =>
            string.IsNullOrEmpty(s) ? s : JsonNamingPolicy.CamelCase.ConvertName(s);

        static string TrimKey(string k) =>
            string.IsNullOrEmpty(k) ? k : k.Split('.')[^1];

        return modelState
            .Where(kvp => kvp.Value?.Errors.Count > 0)
            .ToDictionary(
                kvp => ToCamel(TrimKey(kvp.Key)),
                kvp => kvp.Value!.Errors
                    .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage)
                        ? e.Exception?.Message ?? "Invalid value"
                        : e.ErrorMessage)
                    .ToArray()
            );
    }

    private static Dictionary<string, object[]> BuildDetailedErrors(HttpContext http,
        Dictionary<string, string[]> simple)
    {
        var detailed = new Dictionary<string, object[]>();
        foreach (var (key, value) in simple)
        {
            detailed[key] = [.. value.Select(m => new { message = m, code = ComputeStableCode(key, m) })];
        }

        return detailed;
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
