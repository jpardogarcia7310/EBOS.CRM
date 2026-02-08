using EBOS.CRM.Application.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EBOS.CRM.ApiTests.Options;

public class TenantIsolationOptionsValidationTests
{
    [Fact]
    public void Startup_Throws_WhenTraversalDepthOutOfRange()
    {
        var configuration = BuildConfiguration();
        var optionsSection = configuration.GetSection(TenantIsolationOptions.SectionName);
        var minDepth = optionsSection.GetValue<int>(nameof(TenantIsolationOptions.MinTraversalDepth));
        var maxDepth = optionsSection.GetValue<int>(nameof(TenantIsolationOptions.MaxTraversalDepth));
        var invalidDepth = maxDepth + 1;

        var services = new ServiceCollection();
        ConfigureTenantIsolation(services, BuildConfiguration(invalidDepth));

        using var provider = services.BuildServiceProvider();

        var ex = Assert.Throws<OptionsValidationException>(
            () => provider.GetRequiredService<IOptions<TenantIsolationOptions>>().Value);

        Assert.Contains(nameof(TenantIsolationOptions.TraversalDepth), ex.Message);
    }

    [Fact]
    public void Startup_Allows_WhenTraversalDepthWithinRange()
    {
        var configuration = BuildConfiguration();
        var optionsSection = configuration.GetSection(TenantIsolationOptions.SectionName);
        var minDepth = optionsSection.GetValue<int>(nameof(TenantIsolationOptions.MinTraversalDepth));

        var services = new ServiceCollection();
        ConfigureTenantIsolation(services, BuildConfiguration(minDepth));

        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<TenantIsolationOptions>>().Value;

        Assert.Equal(minDepth, options.TraversalDepth);
    }

    private static IConfiguration BuildConfiguration(int? traversalDepthOverride = null)
    {
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "..", "EBOS.CRM.Api");
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        var builder = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false);

        if (!string.IsNullOrWhiteSpace(environment))
        {
            builder.AddJsonFile($"appsettings.{environment}.json", optional: true);
        }

        if (traversalDepthOverride.HasValue)
        {
            builder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                [$"{TenantIsolationOptions.SectionName}:{nameof(TenantIsolationOptions.TraversalDepth)}"] =
                    traversalDepthOverride.Value.ToString()
            });
        }

        return builder.Build();
    }

    private static void ConfigureTenantIsolation(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<TenantIsolationOptions>()
            .Bind(configuration.GetSection(TenantIsolationOptions.SectionName))
            .Validate(options => options.MinTraversalDepth is >= 1 and <= 50,
                "TenantIsolation:MinTraversalDepth must be between 1 and 50.")
            .Validate(options => options.MaxTraversalDepth is >= 1 and <= 50,
                "TenantIsolation:MaxTraversalDepth must be between 1 and 50.")
            .Validate(options => options.MinTraversalDepth <= options.MaxTraversalDepth,
                "TenantIsolation:MinTraversalDepth must be <= TenantIsolation:MaxTraversalDepth.")
            .Validate(options =>
                    options.TraversalDepth >= options.MinTraversalDepth &&
                    options.TraversalDepth <= options.MaxTraversalDepth,
                "TenantIsolation:TraversalDepth must be within the configured min/max range.")
            .ValidateOnStart();
    }
}
