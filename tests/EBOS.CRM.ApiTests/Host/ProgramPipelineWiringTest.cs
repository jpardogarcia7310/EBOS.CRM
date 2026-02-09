using System.Net;
using EBOS.CRM.Api.Constants;
using EBOS.CRM.ApiTests.Fixtures;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EBOS.CRM.ApiTests.Host;

public class ProgramPipelineWiringTest(ProgramPipelineWiringTest.PipelineWebApplicationFactory factory)
    : IClassFixture<ProgramPipelineWiringTest.PipelineWebApplicationFactory>
{
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "Country");

    [Fact]
    public async Task TenantRequirement_Returns_400_When_No_Header_And_No_Subdomain()
    {
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Remove(HeaderNames.TenantId);

        var response = await client.GetAsync($"/api/v{_version}/Country");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task TenantRequirement_Allows_When_Header_Present()
    {
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Remove(HeaderNames.TenantId);
        client.DefaultRequestHeaders.Add(HeaderNames.TenantId, "7");

        var response = await client.GetAsync($"/api/v{_version}/Country");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task TenantResolution_Allows_When_Subdomain_Present()
    {
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Remove(HeaderNames.TenantId);

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v{_version}/Country");
        request.Headers.Host = "tenant7.api.domain";

        var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task TenantResolution_Uses_Subdomain_When_Header_Invalid()
    {
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Remove(HeaderNames.TenantId);

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v{_version}/Country");
        request.Headers.Add(HeaderNames.TenantId, "invalid");
        request.Headers.Host = "tenant7.api.domain";

        var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    public sealed class PipelineWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<CrmDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                var contextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(CrmDbContext));
                if (contextDescriptor != null)
                {
                    services.Remove(contextDescriptor);
                }

                var dbName = $"ApiTestsDb_{Guid.NewGuid()}";
                services.AddDbContext<CrmDbContext>(options =>
                {
                    options.UseInMemoryDatabase(dbName);
                });

                using var scope = services.BuildServiceProvider().CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<CrmDbContext>();
                db.Database.EnsureCreated();
                IntegrationTestCrmDataSeeder.Seed(db);
            });
        }
    }
}
