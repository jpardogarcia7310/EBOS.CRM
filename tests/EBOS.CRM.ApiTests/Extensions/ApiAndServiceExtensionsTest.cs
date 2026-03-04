using EBOS.CRM.Api.Extensions;
using EBOS.CRM.Api.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EBOS.CRM.ApiTests.Extensions;

public class ApiAndServiceExtensionsTest
{
    [Fact]
    public void ApiBehaviorConfig_Configure_SetsInvalidModelFactory()
    {
        var options = new ApiBehaviorOptions();
        ApiBehaviorConfig.Configure(options);

        var http = new DefaultHttpContext
        {
            Request =
            {
                Path = "/api/v1/test"
            }
        };
        var actionContext = new ActionContext(http, new RouteData(), new ActionDescriptor(), new ModelStateDictionary());
        actionContext.ModelState.AddModelError("Name", "required");

        var result = options.InvalidModelStateResponseFactory(actionContext);
        var bad = Assert.IsType<BadRequestObjectResult>(result);
        
        Assert.IsType<ValidationProblemDetails>(bad.Value);
    }

    [Fact]
    public void ServiceCollectionExtensions_AddErrorResponses_RegistersFilter()
    {
        var options = new SwaggerGenOptions();

        options.AddErrorResponses();
        Assert.Contains(options.OperationFilterDescriptors, x => x.Type == typeof(ErrorResponsesOperationFilter));
    }

    [Fact]
    public void MiddlewareExtensions_ReturnSameBuilder()
    {
        var app = new ApplicationBuilder(new ServiceCollection().BuildServiceProvider());

        var afterError = app.UseApiErrorHandling();
        var afterCorr = app.UseCorrelationId();
        var afterTenantReq = app.UseTenantRequirement();
        var afterTenantRes = app.UseTenantResolution();

        Assert.Same(app, afterError);
        Assert.Same(app, afterCorr);
        Assert.Same(app, afterTenantReq);
        Assert.Same(app, afterTenantRes);
    }

    [Fact]
    public void SwaggerConfig_ApiVersioning_RegistersServices()
    {
        var services = new ServiceCollection();

        SwaggerConfig.ApiVersioning(services);

        Assert.Contains(services, s => s.ServiceType.FullName!.Contains("IApiVersionDescriptionProvider"));
    }
}
