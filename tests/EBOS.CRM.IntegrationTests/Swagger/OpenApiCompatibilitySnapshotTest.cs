using System.Text.Json;
using System.Text.Json.Nodes;
using EBOS.CRM.IntegrationTests.Infrastructure;
using FluentAssertions;

namespace EBOS.CRM.IntegrationTests.Swagger;

public class OpenApiCompatibilitySnapshotTest(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task OpenApiV1_ShouldMatchSnapshot_EnterpriseGate()
    {
        var response = await _client.GetAsync("/swagger/v1/swagger.json");
        response.EnsureSuccessStatusCode();
        var currentJson = await response.Content.ReadAsStringAsync();
        var currentCanonical = Canonicalize(currentJson);

        var snapshotPath = ResolveSnapshotPath();
        var updateSnapshot = string.Equals(
            Environment.GetEnvironmentVariable("UPDATE_OPENAPI_SNAPSHOT"),
            "true",
            StringComparison.OrdinalIgnoreCase);

        if (updateSnapshot)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(snapshotPath)!);
            await File.WriteAllTextAsync(snapshotPath, currentCanonical);
            return;
        }

        File.Exists(snapshotPath)
            .Should()
            .BeTrue($"OpenAPI snapshot not found: {snapshotPath}. Run with UPDATE_OPENAPI_SNAPSHOT=true to create/update.");

        var snapshotCanonical = await File.ReadAllTextAsync(snapshotPath);
        currentCanonical.Should().Be(snapshotCanonical,
            "OpenAPI contract changed. If intentional and non-breaking, update snapshot via UPDATE_OPENAPI_SNAPSHOT=true");
    }

    private static string ResolveSnapshotPath()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current != null &&
               !Directory.Exists(Path.Combine(current.FullName, "tests", "EBOS.CRM.IntegrationTests")))
        {
            current = current.Parent;
        }

        if (current is null)
        {
            throw new InvalidOperationException("Could not locate repository root (tests/EBOS.CRM.IntegrationTests).");
        }

        return Path.Combine(current.FullName, "tests", "EBOS.CRM.IntegrationTests", "Swagger", "openapi.v1.snapshot.json");
    }

    private static string Canonicalize(string json)
    {
        var node = JsonNode.Parse(json) ?? throw new InvalidOperationException("Invalid OpenAPI JSON.");
        var canonical = CanonicalizeNode(node);
        return canonical.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
    }

    private static JsonNode CanonicalizeNode(JsonNode node)
    {
        if (node is JsonObject obj)
        {
            var sorted = new JsonObject();
            foreach (var kvp in obj.OrderBy(k => k.Key, StringComparer.Ordinal))
            {
                sorted[kvp.Key] = kvp.Value is null ? null : CanonicalizeNode(kvp.Value);
            }

            return sorted;
        }

        if (node is JsonArray arr)
        {
            var normalized = new JsonArray();
            foreach (var item in arr)
            {
                normalized.Add(item is null ? null : CanonicalizeNode(item));
            }

            return normalized;
        }

        return node.DeepClone();
    }
}
