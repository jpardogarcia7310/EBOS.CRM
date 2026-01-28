using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Api.IntegrationTests.Infrastructure;
using EBOS.CRM.Domain.Interfaces.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace EBOS.CRM.Api.IntegrationTests.Controllers.IdentificationType;

public class IdentificationTypeErrorHandlingTests : IClassFixture<IdentificationTypeErrorHandlingTests.FailingIdentificationTypeFactory>
{
    private readonly HttpClient _client;

    public IdentificationTypeErrorHandlingTests(FailingIdentificationTypeFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_Returns_500_WhenRepositoryFails()
    {
        var response = await _client.GetAsync("/api/v2/IdentificationType");
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
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IIdentificationTypeRepository));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddScoped<IIdentificationTypeRepository, FailingIdentificationTypeRepository>();
            });
        }
    }
}
