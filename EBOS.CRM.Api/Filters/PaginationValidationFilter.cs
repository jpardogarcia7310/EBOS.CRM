using EBOS.CRM.Api.Helpers;
using EBOS.CRM.Api.Options;
using EBOS.CRM.Api.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace EBOS.CRM.Api.Filters;

public sealed class PaginationValidationFilter(IOptions<PaginationOptions> paginationOptions,
    IStringLocalizer<SharedResource> localizer, ProblemDetailsFactory problemDetailsFactory) : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ActionArguments.TryGetValue("pageSize", out var pageSizeObj) ||
            pageSizeObj is not int pageSize)
        {
            return;
        }

        var settings = paginationOptions.Value;
        var safePageSize = pageSize <= 0 ? settings.DefaultPageSize : pageSize;
        if (safePageSize <= settings.MaxPageSize)
        {
            return;
        }

        var details = problemDetailsFactory.CreateProblemDetails(
            context.HttpContext,
            StatusCodes.Status400BadRequest,
            title: ProblemDetailsDefaults.InvalidPageSizeTitle,
            detail: localizer["InvalidPageSize", settings.MaxPageSize]);

        context.Result = new BadRequestObjectResult(details);
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}
