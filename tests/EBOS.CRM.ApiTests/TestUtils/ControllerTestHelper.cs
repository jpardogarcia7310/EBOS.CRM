using System.Net.Http.Json;

namespace EBOS.CRM.ApiTests.TestUtils;

public static class ControllerTestHelper
{
    public static async Task<long> GetFirstIdAsync<T>(HttpClient client, string url, Func<T, long> selector)
    {
        var items = await client.GetFromJsonAsync<IReadOnlyCollection<T>>(url);
        if (items == null || items.Count == 0)
            throw new InvalidOperationException($"No items returned from {url}");

        return selector(items.First());
    }
}


