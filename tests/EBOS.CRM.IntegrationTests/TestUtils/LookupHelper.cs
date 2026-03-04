using EBOS.CRM.Contracts.Responses.EBOS;

namespace EBOS.CRM.IntegrationTests.TestUtils;

public static class LookupHelper
{
    public static async Task<long> GetStatusIdAsync(HttpClient client, string version)
    {
        var response = await client.GetAsync($"/api/v{version}/Status");
        response.EnsureSuccessStatusCode();

        var items = await response.Content.ReadItemsAsync<StatusResponse>();
        return items.First().Id;
    }

    public static async Task<long> GetCountryIdAsync(HttpClient client, string version)
    {
        var response = await client.GetAsync($"/api/v{version}/Country");
        response.EnsureSuccessStatusCode();

        var items = await response.Content.ReadItemsAsync<CountryResponse>();
        return items.First().Id;
    }

    public static async Task<long> GetAddressTypeIdAsync(HttpClient client, string version)
    {
        var response = await client.GetAsync($"/api/v{version}/AddressType");
        response.EnsureSuccessStatusCode();

        var items = await response.Content.ReadItemsAsync<AddressTypeResponse>();
        return items.First().Id;
    }

    public static async Task<long> GetIdentificationTypeIdAsync(HttpClient client, string version)
    {
        var response = await client.GetAsync($"/api/v{version}/IdentificationType");
        response.EnsureSuccessStatusCode();

        var items = await response.Content.ReadItemsAsync<IdentificationTypeResponse>();
        return items.First().Id;
    }
}
