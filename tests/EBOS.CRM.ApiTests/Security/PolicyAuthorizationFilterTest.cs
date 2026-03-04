using System.Reflection;
using EBOS.CRM.Api.Security;
using EBOS.CRM.Domain.Interfaces.Services;
using EBOS.CRM.Domain.Interfaces.Services.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Moq;

namespace EBOS.CRM.ApiTests.Security;

public class PolicyAuthorizationFilterTest
{
    [Fact]
    public async Task OnActionExecutionAsync_WhenUserAuthenticated_EnsuresAuthorization()
    {
        var currentUser = new Mock<ICurrentUserContext>();
        currentUser.SetupGet(x => x.UserId).Returns(10);

        var policyService = new Mock<IPolicyService>();
        var filter = new PolicyAuthorizationFilter(currentUser.Object, policyService.Object);

        var http = new DefaultHttpContext();
        var action = new ControllerActionDescriptor
        {
            ControllerName = "Customers",
            ActionName = "AddCustomer"
        };
        var actionContext = new ActionContext(http, new RouteData(), action);
        var exec = new ActionExecutingContext(actionContext, new List<IFilterMetadata>(),
            new Dictionary<string, object?>(), new object());
        var executed = new ActionExecutedContext(actionContext, new List<IFilterMetadata>(), new object());

        await filter.OnActionExecutionAsync(exec, () => Task.FromResult(executed));

        policyService.Verify(x => x.EnsureAuthorizedAsync(10, "crm.customer.create", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void PolicyCodeBuilder_Build_ComposesExpectedCode()
    {
        var nestedType = typeof(PolicyAuthorizationFilter).GetNestedType("PolicyCodeBuilder", BindingFlags.NonPublic);
        Assert.NotNull(nestedType);

        var buildMethod = nestedType!.GetMethod("Build", BindingFlags.Public | BindingFlags.Static);
        Assert.NotNull(buildMethod);

        var code = (string)buildMethod!.Invoke(null, new object[] { "Opportunities", "PatchOpportunityStage" })!;

        Assert.Equal("crm.opportunity.update", code);
    }
}
