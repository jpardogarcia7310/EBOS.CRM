using FluentValidation;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;

namespace EBOS.CRM.Api.Validation;

public class FluentValidationActionFilter : IAsyncActionFilter
{
    private const string Key = "FluentValidationFailures";

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var serviceProvider = context.HttpContext.RequestServices;
        var failuresMap = new Dictionary<string, List<object>>(StringComparer.OrdinalIgnoreCase);

        foreach (var arg in context.ActionArguments)
        {
            var argValue = arg.Value;
            if (argValue == null) continue;

            var argType = argValue.GetType();
            var validatorType = typeof(IValidator<>).MakeGenericType(argType);
            var validatorObj = serviceProvider.GetService(validatorType);
            if (validatorObj is not IValidator validator) continue;

            var validationContextType = typeof(ValidationContext<>).MakeGenericType(argType);
            var validationContext = Activator.CreateInstance(validationContextType, argValue) as IValidationContext;
            var result = validator.Validate(validationContext!);

            if (result != null && result.Errors?.Any() == true)
            {
                foreach (var f in result.Errors)
                {
                    var prop = NormalizePropertyName(f.PropertyName);
                    context.ModelState.AddModelError(prop, f.ErrorMessage ?? "Invalid value");

                    var code = string.IsNullOrWhiteSpace(f.ErrorCode) ? null : f.ErrorCode;
                    var entry = new { message = f.ErrorMessage ?? "Invalid value", code };

                    if (!failuresMap.TryGetValue(prop, out var list))
                    {
                        list = [];
                        failuresMap[prop] = list;
                    }
                    list.Add(entry);
                }
            }
        }

        if (failuresMap.Count > 0)
        {
            var dict = failuresMap.ToDictionary(k => k.Key, k => k.Value.ToArray());
            context.HttpContext.Items[Key] = dict;
        }

        if (!context.ModelState.IsValid)
        {
            // Let MVC handle the invalid model state via InvalidModelStateResponseFactory
            return;
        }

        await next();
    }

    private static string NormalizePropertyName(string propertyName)
    {
        if (string.IsNullOrWhiteSpace(propertyName)) 
            return propertyName;
        var last = propertyName.Split('.')[^1];
        return JsonNamingPolicy.CamelCase.ConvertName(last);
    }
}