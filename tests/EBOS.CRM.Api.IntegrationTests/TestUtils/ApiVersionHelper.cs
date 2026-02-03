using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;

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
            var versions = apiExplorer.ApiDescriptionGroups.Items
                .SelectMany(g => g.Items)
                .Where(d => string.Equals(d.ActionDescriptor.RouteValues["controller"], controller, StringComparison.OrdinalIgnoreCase))
                .Select(d => d.GroupName)
                .Where(g => !string.IsNullOrWhiteSpace(g))
                .Select(g =>
                {
                    var name = g!.TrimStart('v', 'V');
                    return ApiVersion.TryParse(name, out var v) ? v : null;
                })
                .Where(v => v != null)
                .Cast<ApiVersion>()
                .Distinct()
                .OrderByDescending(v => v);

            var latestForController = versions.FirstOrDefault();
            if (latestForController != null)
            {
                return latestForController.ToString("VVV");
            }
        }

        var latest = provider.ApiVersionDescriptions
            .OrderByDescending(d => d.ApiVersion)
            .First();

        return latest.ApiVersion.ToString("VVV");
    }
}
