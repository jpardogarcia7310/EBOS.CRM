using System.Net.Http.Json;
using EBOS.CRM.Application.Contracts.Responses.Common;

namespace EBOS.CRM.ApiTests.TestUtils;

public static class HttpContentExtensions
{
    public static async Task<IReadOnlyCollection<T>> ReadPagedItemsAsync<T>(this HttpContent content)
    {
        var response = await content.ReadFromJsonAsync<PagedResponse<T>>();
        return response?.Items ?? Array.Empty<T>();
    }
}
