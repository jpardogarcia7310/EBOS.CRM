using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EBOS.CRM.IntegrationTests.Controllers.EBOS.AddressType;

public class AddressTypeErrorHandlingTest(AddressTypeErrorHandlingTest.FailingAddressTypeFactory factory)
    : IClassFixture<AddressTypeErrorHandlingTest.FailingAddressTypeFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory);

    [Fact]
    public async Task GetAll_Returns_500_WhenRepositoryFails()
    {
        var response = await _client.GetAsync($"/api/v{_version}/AddressType");
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem!.Status.Should().Be(500);
    }

    [Fact]
    public async Task GetById_Returns_500_WhenRepositoryFails()
    {
        var response = await _client.GetAsync($"/api/v{_version}/AddressType/1");
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem!.Status.Should().Be(500);
    }

    public sealed class FailingAddressTypeFactory : CustomWebApplicationFactory
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<IAddressTypeRepository>();

                services.AddScoped<IAddressTypeRepository, FailingAddressTypeRepository>();
            });
        }
    }
}








