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

    private static Dictionary<string, object[]> BuildDetailedErrors(HttpContext http, Dictionary<string, string[]> simple)
    {
        var detailed = new Dictionary<string, object[]>();

        if (http.Items.TryGetValue("FluentValidationFailures", out var raw) && raw is IDictionary<string, object[]> map)
        {
            foreach (var kv in simple)
            {
                var key = kv.Key;
                if (map.TryGetValue(key, out var arrObj))
                {
                    var list = new List<object>();
                    foreach (var o in arrObj)
                    {
                        var props = o.GetType().GetProperties();
                        var msg = "Invalid value";
                        string? code = null;

                        foreach (var p in props)
                        {
                            if (string.Equals(p.Name, "message", StringComparison.OrdinalIgnoreCase))
                            {
                                msg = p.GetValue(o)?.ToString() ?? "Invalid value";
                            }
                            else if (string.Equals(p.Name, "code", StringComparison.OrdinalIgnoreCase))
                            {
                                code = p.GetValue(o)?.ToString();
                            }
                        }

                        code ??= $"VAL_{Math.Abs((key + "|" + msg).GetHashCode()):D6}";
                        list.Add(new { message = msg, code });
                    }
                    detailed[key] = [.. list];
                }
                else
                {
                    detailed[key] = [.. kv.Value.Select(m => new { message = m, code = $"VAL_{Math.Abs((key + "|" + m).GetHashCode()):D6}" })];
                }
            }
        }
        else
        {
            foreach (var kv in simple)
            {
                var key = kv.Key;
                detailed[key] = [.. kv.Value.Select(m => new { message = m, code = $"VAL_{Math.Abs((key + "|" + m).GetHashCode()):D6}" })];
            }
        }

        return detailed;
    }
}