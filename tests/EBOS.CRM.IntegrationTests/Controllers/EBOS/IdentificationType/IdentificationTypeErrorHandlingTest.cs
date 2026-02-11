using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace EBOS.CRM.IntegrationTests.Controllers.EBOS.IdentificationType;

public class IdentificationTypeErrorHandlingTest(
    IdentificationTypeErrorHandlingTest.FailingIdentificationTypeFactory factory)
    : IClassFixture<IdentificationTypeErrorHandlingTest.FailingIdentificationTypeFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory);

    [Fact]
    public async Task GetAll_Returns_500_WhenRepositoryFails()
    {
        var response = await _client.GetAsync($"/api/v{_version}/IdentificationType");
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem!.Status.Should().Be(500);
    }

    [Fact]
    public async Task GetById_Returns_500_WhenRepositoryFails()
    {
        var response = await _client.GetAsync($"/api/v{_version}/IdentificationType/1");
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem!.Status.Should().Be(500);
    }

    public sealed class FailingIdentificationTypeFactory : CustomWebApplicationFactory
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
            builder.ConfigureServices(services =>
            {
                var descriptors = services
                    .Where(d => d.ServiceType == typeof(IIdentificationTypeRepository))
                    .ToList();
                foreach (var descriptor in descriptors)
                {
                    services.Remove(descriptor);
                }

                services.AddScoped<IIdentificationTypeRepository, FailingIdentificationTypeRepository>();
            });
        }
    }
}







