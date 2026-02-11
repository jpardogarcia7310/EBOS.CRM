using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Application.Contracts.Requests.CRM.BranchOffice;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EBOS.CRM.IntegrationTests.Controllers.CRM.BranchOffice;

public class BranchOfficeErrorHandlingTest(BranchOfficeErrorHandlingTest.FailingBranchOfficeFactory factory)
    : IClassFixture<BranchOfficeErrorHandlingTest.FailingBranchOfficeFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "BranchOffice");

    [Fact]
    public async Task GetAll_Returns_500_WhenRepositoryFails()
    {
        var response = await _client.GetAsync($"/api/v{_version}/BranchOffice");
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem!.Status.Should().Be(500);
    }

    [Fact]
    public async Task Add_Returns_500_WhenRepositoryFails()
    {
        var request = new AddBranchOfficeRequest(
            TenantId: 1,
            Name: "Branch A",
            PhoneNumber: "+1 555 0101",
            CorporateCustomerId: 1);

        var response = await _client.PostAsJsonAsync($"/api/v{_version}/BranchOffice", request);
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Update_Returns_500_WhenRepositoryFails()
    {
        var request = new UpdateBranchOfficeRequest(
            Id: 1,
            TenantId: 1,
            Name: "Branch A",
            PhoneNumber: "+1 555 0101",
            CorporateCustomerId: 1);

        var response = await _client.PutAsJsonAsync($"/api/v{_version}/BranchOffice/1", request);
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Delete_Returns_500_WhenRepositoryFails()
    {
        var response = await _client.DeleteAsync($"/api/v{_version}/BranchOffice/1");
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    public sealed class FailingBranchOfficeFactory : CustomWebApplicationFactory
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<IBranchOfficeRepository>();

                services.AddScoped<IBranchOfficeRepository, FailingBranchOfficeRepository>();
            });
        }
    }
}



