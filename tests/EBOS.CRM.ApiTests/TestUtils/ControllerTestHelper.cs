using System.Linq;
using System.Net.Http.Json;
using EBOS.CRM.Application.Contracts.Responses.Common;

namespace EBOS.CRM.ApiTests.TestUtils;

public static class ControllerTestHelper
{
    public static async Task<long> GetFirstIdAsync<T>(HttpClient client, string url, Func<T, long> selector)
    {
        var response = await client.GetFromJsonAsync<PagedResponse<T>>(url);
        var items = response?.Items;
        if (items == null || items.Count == 0)
            throw new InvalidOperationException($"No items returned from {url}");

        return selector(items.First());
    }
}
