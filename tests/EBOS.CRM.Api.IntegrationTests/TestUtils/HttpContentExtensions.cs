using System.Net.Http.Json;

namespace EBOS.CRM.Api.IntegrationTests.TestUtils;

public static class HttpContentExtensions
{
    public static async Task<IReadOnlyCollection<T>> ReadItemsAsync<T>(this HttpContent content)
    {
        var response = await content.ReadFromJsonAsync<IReadOnlyCollection<T>>();
        return response ?? Array.Empty<T>();
    }
}


