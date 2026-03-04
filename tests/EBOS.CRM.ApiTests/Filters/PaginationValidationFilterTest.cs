using EBOS.CRM.Api.Filters;
using EBOS.CRM.Api.Options;
using EBOS.CRM.Api.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Moq;

namespace EBOS.CRM.ApiTests.Filters;

public class PaginationValidationFilterTest
{
    [Fact]
    public void OnActionExecuting_WhenPageSizeExceedsMax_ReturnsBadRequest()
    {
        var options = global::Microsoft.Extensions.Options.Options.Create(
            new PaginationOptions { DefaultPageSize = 10, MaxPageSize = 50 });

        var localizer = new Mock<IStringLocalizer<SharedResource>>();
        localizer.Setup(x => x["InvalidPageSize", It.IsAny<object[]>()])
            .Returns(new LocalizedString("InvalidPageSize", "Max page size is 50"));

        var problemFactory = new Mock<ProblemDetailsFactory>();
        problemFactory.Setup(x => x.CreateProblemDetails(
                It.IsAny<HttpContext>(),
                StatusCodes.Status400BadRequest,
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .Returns(new ProblemDetails { Status = StatusCodes.Status400BadRequest, Title = "Invalid" });

        var filter = new PaginationValidationFilter(options, localizer.Object, problemFactory.Object);

        var http = new DefaultHttpContext();
        var actionContext = new ActionContext(http, new RouteData(), new ActionDescriptor(), new ModelStateDictionary());
        var ctx = new ActionExecutingContext(actionContext, new List<IFilterMetadata>(),
            new Dictionary<string, object?> { ["pageSize"] = 100 }, new object());

        filter.OnActionExecuting(ctx);

        Assert.IsType<BadRequestObjectResult>(ctx.Result);
    }

    [Fact]
    public void OnActionExecuting_WhenPageSizeWithinMax_DoesNothing()
    {
        var options = global::Microsoft.Extensions.Options.Options.Create(
            new PaginationOptions { DefaultPageSize = 10, MaxPageSize = 50 });
        var localizer = new Mock<IStringLocalizer<SharedResource>>();
        var problemFactory = new Mock<ProblemDetailsFactory>();
        var filter = new PaginationValidationFilter(options, localizer.Object, problemFactory.Object);

        var http = new DefaultHttpContext();
        var actionContext = new ActionContext(http, new RouteData(), new ActionDescriptor(), new ModelStateDictionary());
        var ctx = new ActionExecutingContext(actionContext, new List<IFilterMetadata>(),
            new Dictionary<string, object?> { ["pageSize"] = 25 }, new object());

        filter.OnActionExecuting(ctx);

        Assert.Null(ctx.Result);
    }
}
