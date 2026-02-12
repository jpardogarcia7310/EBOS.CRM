using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;
using EBOS.CRM.Domain.Interfaces.Services;

namespace EBOS.CRM.Api.Security;

public sealed class PolicyAuthorizationFilter(ICurrentUserContext currentUser, IPolicyService policyService)
    : IAsyncActionFilter
{
    private readonly ICurrentUserContext _currentUser = currentUser
        ?? throw new ArgumentNullException(nameof(currentUser));
    private readonly IPolicyService _policyService = policyService
        ?? throw new ArgumentNullException(nameof(policyService));

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var descriptor = context.ActionDescriptor as ControllerActionDescriptor;
        if (descriptor is null)
        {
            await next();
            return;
        }

        var policyCode = PolicyCodeBuilder.Build(descriptor.ControllerName, descriptor.ActionName);
        if (!string.IsNullOrWhiteSpace(policyCode) && _currentUser.UserId > 0)
        {
            await _policyService.EnsureAuthorizedAsync(_currentUser.UserId, policyCode,
                context.HttpContext.RequestAborted);
        }

        await next();
    }

    private static class PolicyCodeBuilder
    {
        public static string Build(string controllerName, string actionName)
        {
            var action = ResolveAction(actionName);
            var resource = ToKebabCase(Singularize(controllerName));
            return $"crm.{resource}.{action}";
        }

        private static string ResolveAction(string actionName)
        {
            if (actionName.StartsWith("Add", StringComparison.Ordinal))
            {
                return "create";
            }

            if (actionName.StartsWith("Update", StringComparison.Ordinal) ||
                actionName.StartsWith("Patch", StringComparison.Ordinal))
            {
                return "update";
            }

            if (actionName.StartsWith("Delete", StringComparison.Ordinal))
            {
                return "delete";
            }

            return "read";
        }

        private static string Singularize(string value)
        {
            if (value.EndsWith("ies", StringComparison.OrdinalIgnoreCase) && value.Length > 3)
            {
                return value[..^3] + "y";
            }

            if (value.EndsWith("ses", StringComparison.OrdinalIgnoreCase) && value.Length > 3)
            {
                return value[..^2];
            }

            if (value.EndsWith("ss", StringComparison.OrdinalIgnoreCase))
            {
                return value;
            }

            if (value.EndsWith("s", StringComparison.OrdinalIgnoreCase) && value.Length > 1)
            {
                return value[..^1];
            }

            return value;
        }

        private static string ToKebabCase(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            if (value.All(char.IsUpper))
            {
                return value.ToLowerInvariant();
            }

            var sb = new StringBuilder(value.Length + 8);
            for (var i = 0; i < value.Length; i++)
            {
                var c = value[i];
                if (char.IsUpper(c))
                {
                    var prevIsLower = i > 0 && char.IsLower(value[i - 1]);
                    var nextIsLower = i + 1 < value.Length && char.IsLower(value[i + 1]);

                    if (i > 0 && (prevIsLower || nextIsLower))
                    {
                        sb.Append('-');
                    }

                    sb.Append(char.ToLowerInvariant(c));
                }
                else
                {
                    sb.Append(char.ToLowerInvariant(c));
                }
            }

            return sb.ToString();
        }
    }
}
