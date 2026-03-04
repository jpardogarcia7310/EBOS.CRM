using System.Net;
using System.Text;
using System.Text.Json;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;

namespace EBOS.CRM.Api.IntegrationTests.Controllers;

public class EbosReadOnlyEndpointsTest(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory = factory;

    [Theory]
    [InlineData("AddressType")]
    [InlineData("Country")]
    [InlineData("IdentificationType")]
    [InlineData("Status")]
    public async Task Post_Is_Not_Available_For_Ebos_Endpoints(string resource)
    {
        var client = _factory.CreateClient();
        var version = ApiVersionHelper.GetLatestVersion(_factory, resource);
        using var content = new StringContent("{}", Encoding.UTF8, "application/json");

        var response = await client.PostAsync($"/api/v{version}/{resource}", content);

        response.StatusCode.Should().BeOneOf(HttpStatusCode.MethodNotAllowed, HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData("AddressType")]
    [InlineData("Country")]
    [InlineData("IdentificationType")]
    [InlineData("Status")]
    public async Task Put_Is_Not_Available_For_Ebos_Endpoints(string resource)
    {
        var client = _factory.CreateClient();
        var version = ApiVersionHelper.GetLatestVersion(_factory, resource);
        var id = await GetFirstIdAsync(client, version, resource);
        using var content = new StringContent("{}", Encoding.UTF8, "application/json");

        var response = await client.PutAsync($"/api/v{version}/{resource}/{id}", content);

        response.StatusCode.Should().BeOneOf(HttpStatusCode.MethodNotAllowed, HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData("AddressType")]
    [InlineData("Country")]
    [InlineData("IdentificationType")]
    [InlineData("Status")]
    public async Task Patch_Is_Not_Available_For_Ebos_Endpoints(string resource)
    {
        var client = _factory.CreateClient();
        var version = ApiVersionHelper.GetLatestVersion(_factory, resource);
        var id = await GetFirstIdAsync(client, version, resource);
        using var content = new StringContent("{}", Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage(HttpMethod.Patch, $"/api/v{version}/{resource}/{id}")
        {
            Content = content
        };

        var response = await client.SendAsync(request);

        response.StatusCode.Should().BeOneOf(HttpStatusCode.MethodNotAllowed, HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData("AddressType")]
    [InlineData("Country")]
    [InlineData("IdentificationType")]
    [InlineData("Status")]
    public async Task Delete_Is_Not_Available_For_Ebos_Endpoints(string resource)
    {
        var client = _factory.CreateClient();
        var version = ApiVersionHelper.GetLatestVersion(_factory, resource);
        var id = await GetFirstIdAsync(client, version, resource);

        var response = await client.DeleteAsync($"/api/v{version}/{resource}/{id}");

        response.StatusCode.Should().BeOneOf(HttpStatusCode.MethodNotAllowed, HttpStatusCode.NotFound);
    }

    private static async Task<long> GetFirstIdAsync(HttpClient client, string version, string resource)
    {
        var response = await client.GetAsync($"/api/v{version}/{resource}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var first = doc.RootElement.EnumerateArray().FirstOrDefault();
        first.TryGetProperty("id", out var idProp).Should().BeTrue();
        return idProp.GetInt64();
    }
}
