namespace EBOS.CRM.StressTests.Infrastructure;

public static class StressHelper
{
    private const int Parallelism = 100;

    public static async Task AssertReadStressAsync(HttpClient client, string baseUrl, long id)
    {
        var tasks = Enumerable.Range(0, Parallelism)
            .SelectMany(_ => new[]
            {
                client.GetAsync(baseUrl),
                client.GetAsync($"{baseUrl}/{id}")
            });

        var responses = await Task.WhenAll(tasks);

        Assert.All(responses, response => Assert.True((int)response.StatusCode < 500));
    }

    public static async Task AssertWriteStressAsync(HttpClient client, string baseUrl, long id,
        StressPayloadFactories payloads)
    {
        var postTasks = Enumerable.Range(0, Parallelism)
            .Select(_ => client.PostAsync(baseUrl, payloads.Post()));
        var putTasks = Enumerable.Range(0, Parallelism)
            .Select(_ => client.PutAsync($"{baseUrl}/{id}", payloads.Put(id)));
        var patchTasks = Enumerable.Range(0, Parallelism)
            .Select(_ => client.SendAsync(new HttpRequestMessage(new HttpMethod("PATCH"), $"{baseUrl}/{id}")
            {
                Content = payloads.Patch(id)
            }));
        var deleteTasks = Enumerable.Range(0, Parallelism)
            .Select(_ => client.DeleteAsync($"{baseUrl}/{id}"));

        var responses = await Task.WhenAll(postTasks.Concat(putTasks).Concat(patchTasks).Concat(deleteTasks));

        Assert.All(responses, response => Assert.True((int)response.StatusCode < 600));
    }

    public static async Task AssertNegativeStressAsync(HttpClient client, string baseUrl, long id)
    {
        var invalidId = id <= 0 ? long.MaxValue : -1;

        var getResponse = await client.GetAsync($"{baseUrl}/{invalidId}");
        Assert.True(getResponse.StatusCode is System.Net.HttpStatusCode.NotFound
            or System.Net.HttpStatusCode.BadRequest);

        var postResponse = await client.PostAsync(baseUrl, CreateJsonContent());
        Assert.True(postResponse.StatusCode is System.Net.HttpStatusCode.BadRequest
            or System.Net.HttpStatusCode.MethodNotAllowed);

        var putResponse = await client.PutAsync($"{baseUrl}/{invalidId}", CreateJsonContent());
        Assert.True(putResponse.StatusCode is System.Net.HttpStatusCode.BadRequest
            or System.Net.HttpStatusCode.MethodNotAllowed);

        var patchResponse = await client.SendAsync(new HttpRequestMessage(new HttpMethod("PATCH"), $"{baseUrl}/{invalidId}")
        {
            Content = CreateJsonContent()
        });
        Assert.True(patchResponse.StatusCode is System.Net.HttpStatusCode.BadRequest
            or System.Net.HttpStatusCode.MethodNotAllowed);

        var deleteResponse = await client.DeleteAsync($"{baseUrl}/{invalidId}");
        Assert.True(deleteResponse.StatusCode is System.Net.HttpStatusCode.NotFound
            or System.Net.HttpStatusCode.BadRequest
            or System.Net.HttpStatusCode.MethodNotAllowed);
    }

    private static StringContent CreateJsonContent()
        => new("{}", System.Text.Encoding.UTF8, "application/json");
}
