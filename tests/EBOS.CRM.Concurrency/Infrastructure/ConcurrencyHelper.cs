using System.Net;

namespace EBOS.CRM.Concurrency.Infrastructure;

public static class ConcurrencyHelper
{
    public const int Parallelism = 100;

    public static async Task AssertReadConcurrencyAsync(HttpClient client, string baseUrl, long id)
    {
        var tasks = Enumerable.Range(0, Parallelism)
            .SelectMany(_ => new[]
            {
                client.GetAsync(baseUrl),
                client.GetAsync($"{baseUrl}/{id}")
            })
            .ToArray();

        var responses = await Task.WhenAll(tasks);

        Assert.All(responses, response => Assert.Equal(HttpStatusCode.OK, response.StatusCode));
    }

    public static async Task AssertWriteConcurrencyAsync(
        HttpClient client,
        string baseUrl,
        long id,
        ConcurrencyPayloadFactories payloads)
    {
        if (payloads.UseIsolatedWrite)
        {
            var tasks = Enumerable.Range(0, Parallelism)
                .Select(_ => ExecuteIsolatedWriteAsync(client, baseUrl, payloads));

            var batches = await Task.WhenAll(tasks);
            var writeResponses = batches.SelectMany(batch => batch).ToArray();

            Assert.All(writeResponses, response => Assert.True((int)response.StatusCode < 500));
            return;
        }

        var postTasks = payloads.Post is null
            ? Enumerable.Empty<Task<HttpResponseMessage>>()
            : Enumerable.Range(0, Parallelism)
                .Select(_ => client.PostAsync(baseUrl, payloads.Post()));
        var putTasks = payloads.Put is null
            ? Enumerable.Empty<Task<HttpResponseMessage>>()
            : Enumerable.Range(0, Parallelism)
                .Select(_ => client.PutAsync($"{baseUrl}/{id}", payloads.Put(id)));
        var patchTasks = payloads.Patch is null
            ? Enumerable.Empty<Task<HttpResponseMessage>>()
            : Enumerable.Range(0, Parallelism)
                .Select(_ => client.SendAsync(new HttpRequestMessage(new HttpMethod("PATCH"), $"{baseUrl}/{id}")
                {
                    Content = payloads.Patch(id)
                }));
        var deleteTasks = payloads.AllowDelete
            ? Enumerable.Range(0, Parallelism)
                .Select(_ => client.DeleteAsync($"{baseUrl}/{id}"))
            : Enumerable.Empty<Task<HttpResponseMessage>>();

        var responses = await Task.WhenAll(postTasks.Concat(putTasks).Concat(patchTasks).Concat(deleteTasks));

        Assert.All(responses, response => Assert.True((int)response.StatusCode < 500));
    }

    private static StringContent CreateJsonContent()
        => new("{}", System.Text.Encoding.UTF8, "application/json");

    private static async Task<IReadOnlyList<HttpResponseMessage>> ExecuteIsolatedWriteAsync(
        HttpClient client,
        string baseUrl,
        ConcurrencyPayloadFactories payloads)
    {
        var responses = new List<HttpResponseMessage>();

        if (payloads.Post is null)
        {
            return responses;
        }

        var postResponse = await client.PostAsync(baseUrl, payloads.Post());
        responses.Add(postResponse);

        var id = await TryReadIdAsync(postResponse);
        if (id <= 0)
        {
            return responses;
        }

        if (payloads.Put is not null)
        {
            responses.Add(await client.PutAsync($"{baseUrl}/{id}", payloads.Put(id)));
        }

        if (payloads.Patch is not null)
        {
            responses.Add(await client.SendAsync(new HttpRequestMessage(new HttpMethod("PATCH"), $"{baseUrl}/{id}")
            {
                Content = payloads.Patch(id)
            }));
        }

        if (payloads.AllowDelete)
        {
            responses.Add(await client.DeleteAsync($"{baseUrl}/{id}"));
        }

        return responses;
    }

    private static async Task<long> TryReadIdAsync(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            return 0;
        }

        var payload = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(payload))
        {
            return 0;
        }

        try
        {
            using var document = System.Text.Json.JsonDocument.Parse(payload);
            if (document.RootElement.ValueKind != System.Text.Json.JsonValueKind.Object)
            {
                return 0;
            }

            foreach (var property in document.RootElement.EnumerateObject())
            {
                if (!string.Equals(property.Name, "id", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (property.Value.ValueKind == System.Text.Json.JsonValueKind.Number &&
                    property.Value.TryGetInt64(out var id))
                {
                    return id;
                }
            }
        }
        catch
        {
            return 0;
        }

        return 0;
    }
}
