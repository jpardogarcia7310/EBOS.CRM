using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Api.IntegrationTests.Infrastructure;
using EBOS.CRM.Api.IntegrationTests.TestUtils;
using EBOS.CRM.Application.Contracts.Requests.CRM.BranchOfficeAddress;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace EBOS.CRM.Api.IntegrationTests.Controllers.CRM.BranchOfficeAddress;

public class BranchOfficeAddressErrorHandlingTest(
    BranchOfficeAddressErrorHandlingTest.FailingBranchOfficeAddressFactory factory)
    : IClassFixture<BranchOfficeAddressErrorHandlingTest.FailingBranchOfficeAddressFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "BranchOfficeAddress");

    [Fact]
    public async Task GetAll_Returns_500_WhenRepositoryFails()
    {
        var response = await _client.GetAsync($"/api/v{_version}/BranchOfficeAddress");
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem!.Status.Should().Be(500);
    }

    [Fact]
    public async Task Add_Returns_500_WhenRepositoryFails()
    {
        var request = new AddBranchOfficeAddressRequest(
            TenantId: 1,
            BranchOfficeId: 1,
            AddressId: 1,
            IsPrimary: true,
            ValidFrom: new DateTime(2024, 1, 1),
            ValidTo: null,
            IsCurrent: true);

        var response = await _client.PostAsJsonAsync($"/api/v{_version}/BranchOfficeAddress", request);
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Update_Returns_500_WhenRepositoryFails()
    {
        var request = new UpdateBranchOfficeAddressRequest(
            TenantId: 1,
            BranchOfficeId: 1,
            AddressId: 1,
            IsPrimary: true,
            ValidFrom: new DateTime(2024, 1, 1),
            ValidTo: null,
            IsCurrent: true);

        var response = await _client.PutAsJsonAsync($"/api/v{_version}/BranchOfficeAddress/1", request);
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Delete_Returns_500_WhenRepositoryFails()
    {
        var response = await _client.DeleteAsync($"/api/v{_version}/BranchOfficeAddress/1");
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    public sealed class FailingBranchOfficeAddressFactory : CustomWebApplicationFactory
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IBranchOfficeAddressRepository));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddScoped<IBranchOfficeAddressRepository, FailingBranchOfficeAddressRepository>();
            });
        }
    }
}



