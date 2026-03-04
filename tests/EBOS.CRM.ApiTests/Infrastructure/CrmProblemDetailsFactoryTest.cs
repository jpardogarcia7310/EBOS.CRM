using EBOS.CRM.Api.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EBOS.CRM.ApiTests.Infrastructure;

public class CrmProblemDetailsFactoryTest
{
    [Fact]
    public void CreateProblemDetails_AppliesDefaultsAndTraceId()
    {
        var options = Microsoft.Extensions.Options.Options.Create(new ApiBehaviorOptions());
        options.Value.ClientErrorMapping[404] = new ClientErrorData { Title = "Not found", Link = "https://rfc" };

        var factory = new CrmProblemDetailsFactory(options);
        var http = new DefaultHttpContext
        {
            TraceIdentifier = "trace-1",
            Request =
            {
                Path = "/x"
            }
        };

        var result = factory.CreateProblemDetails(http, 404);

        Assert.Equal("Not found", result.Title);
        Assert.Equal("https://rfc", result.Type);
        Assert.Equal("/x", result.Instance);
        Assert.Equal("trace-1", result.Extensions["traceId"]);
    }

    [Fact]
    public void CreateValidationProblemDetails_SetsStatusAndErrors()
    {
        var options = Microsoft.Extensions.Options.Options.Create(new ApiBehaviorOptions());
        var factory = new CrmProblemDetailsFactory(options);
        var http = new DefaultHttpContext();
        var ms = new ModelStateDictionary();
        ms.AddModelError("Name", "Required");

        var result = factory.CreateValidationProblemDetails(http, ms);

        Assert.Equal(400, result.Status);
        Assert.True(result.Errors.ContainsKey("Name"));
    }
}
