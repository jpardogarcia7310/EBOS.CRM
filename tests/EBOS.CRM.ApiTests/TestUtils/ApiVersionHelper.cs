using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace EBOS.CRM.ApiTests.TestUtils;

public static class ApiVersionHelper
{
    public static string GetLatestVersion(WebApplicationFactory<Program> factory)
    {
        using var scope = factory.Services.CreateScope();
        var provider = scope.ServiceProvider.GetRequiredService<IApiVersionDescriptionProvider>();

        var latest = provider.ApiVersionDescriptions
            .OrderByDescending(d => d.ApiVersion)
            .First();

        return latest.GroupName;
    }
}
