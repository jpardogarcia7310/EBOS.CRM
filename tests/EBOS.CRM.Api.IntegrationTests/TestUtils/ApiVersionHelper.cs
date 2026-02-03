using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace EBOS.CRM.Api.IntegrationTests.TestUtils;

public static class ApiVersionHelper
{
    public static string GetLatestVersion(WebApplicationFactory<Program> factory, string? controller = null)
    {
        using var scope = factory.Services.CreateScope();
        var provider = scope.ServiceProvider.GetRequiredService<IApiVersionDescriptionProvider>();
        var apiExplorer = scope.ServiceProvider.GetRequiredService<IApiDescriptionGroupCollectionProvider>();

        if (!string.IsNullOrWhiteSpace(controller))
        {
            var latestGroup = apiExplorer.ApiDescriptionGroups.Items
                .SelectMany(g => g.Items)
                .Where(d => string.Equals(d.ActionDescriptor.RouteValues["controller"], controller, StringComparison.OrdinalIgnoreCase))
                .Select(d => d.GroupName)
                .Where(g => !string.IsNullOrWhiteSpace(g))
                .OrderByDescending(g => g)
                .FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(latestGroup))
            {
                return latestGroup!.TrimStart('v', 'V');
            }
        }

        var latest = provider.ApiVersionDescriptions
            .OrderByDescending(d => d.ApiVersion)
            .First();

        return latest.GroupName.TrimStart('v', 'V');
    }
}
