using EBOS.CRM.Api.IntegrationTests;

namespace EBOS.CRM.ApiTests;

[CollectionDefinition("IntegrationTestsCollection")]
public class IntegrationTestsCollection : ICollectionFixture<CustomWebApplicationFactory>
{
    // xUnit uses this class to share the CustomWebApplicationFactory instance across tests.
}
