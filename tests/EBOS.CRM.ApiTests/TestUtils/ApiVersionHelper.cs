using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace EBOS.CRM.ApiTests.TestUtils;

public static class ApiVersionHelper
{
    public static string GetLatestVersion(WebApplicationFactory<Program> factory, string? controller = null)
    {
        using var scope = factory.Services.CreateScope();
        var provider = scope.ServiceProvider.GetRequiredService<IApiVersionDescriptionProvider>();
        var apiExplorer = scope.ServiceProvider.GetRequiredService<IApiDescriptionGroupCollectionProvider>();

        if (!string.IsNullOrWhiteSpace(controller))
        {
            var groupName = apiExplorer.ApiDescriptionGroups.Items
                .SelectMany(g => g.Items)
                .Where(d => string.Equals(d.ActionDescriptor.RouteValues["controller"], controller, StringComparison.OrdinalIgnoreCase))
                .Select(d => d.GroupName)
                .FirstOrDefault(g => !string.IsNullOrWhiteSpace(g));

            if (!string.IsNullOrWhiteSpace(groupName))
            {
                return groupName!.TrimStart('v', 'V');
            }
        }

        var latest = provider.ApiVersionDescriptions
            .OrderByDescending(d => d.ApiVersion)
            .First();

        return latest.GroupName.TrimStart('v', 'V');
    }
}


