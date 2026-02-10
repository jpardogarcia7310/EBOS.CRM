using EBOS.CRM.Application.Services.Interfaces;

namespace EBOS.CRM.Api.IntegrationTests.Infrastructure;

public sealed class TestCurrentUserContext : ICurrentUserContext
{
    public long UserId => 1;
    public string CorrelationId => "integration-test";
}
