using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Contracts.Requests.CRM.IndividualCustomer;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EBOS.CRM.IntegrationTests.Controllers.CRM.IndividualCustomer;

public class IndividualCustomerErrorHandlingTest(
    IndividualCustomerErrorHandlingTest.FailingIndividualCustomerFactory factory)
    : IClassFixture<IndividualCustomerErrorHandlingTest.FailingIndividualCustomerFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "IndividualCustomer");

    [Fact]
    public async Task GetAll_Returns_500_WhenRepositoryFails()
    {
        var response = await _client.GetAsync($"/api/v{_version}/IndividualCustomer");
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem.Status.Should().Be(500);
    }

    [Fact]
    public async Task Add_Returns_500_WhenRepositoryFails()
    {
        var request = new AddIndividualCustomerRequest(
            TenantId: 1,
            Code: "IND-001",
            Email: "ind@example.com",
            Phone: "+1 555 1111",
            StatusId: 1,
            FirstName: "Ana",
            LastName: "Garcia",
            BirthDate: new DateTime(1990, 1, 1),
            IdentificationNumber: "ID123",
            IdentificationTypeId: 1);

        var response = await _client.PostAsJsonAsync($"/api/v{_version}/IndividualCustomer", request);
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Update_Returns_500_WhenRepositoryFails()
    {
        var request = new UpdateIndividualCustomerRequest(
            TenantId: 1,
            Code: "IND-001",
            Email: "ind@example.com",
            Phone: "+1 555 1111",
            StatusId: 1,
            FirstName: "Ana",
            LastName: "Garcia",
            BirthDate: new DateTime(1990, 1, 1),
            IdentificationNumber: "ID123",
            IdentificationTypeId: 1);

        var response = await _client.PutAsJsonAsync($"/api/v{_version}/IndividualCustomer/1", request);
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Delete_Returns_500_WhenRepositoryFails()
    {
        var response = await _client.DeleteAsync($"/api/v{_version}/IndividualCustomer/1");
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    public sealed class FailingIndividualCustomerFactory : CustomWebApplicationFactory
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<IIndividualCustomerRepository>();

                services.AddScoped<IIndividualCustomerRepository, FailingIndividualCustomerRepository>();
            });
        }
    }
}



