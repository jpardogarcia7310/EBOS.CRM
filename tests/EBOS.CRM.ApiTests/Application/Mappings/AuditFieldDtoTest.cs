using EBOS.CRM.Contracts.Requests.CRM.Customer;
using FluentAssertions;

namespace EBOS.CRM.ApiTests.Application.Mappings;

public class AuditFieldDtoTest
{
    private static readonly string[] AuditFields = ["CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy"];

    [Fact]
    public void Requests_DoNotExpose_AuditFields()
    {
        var assembly = typeof(PatchCustomerRequest).Assembly;
        var requestTypes = assembly.GetTypes()
            .Where(t => t.IsPublic && t.Name.EndsWith("Request", StringComparison.Ordinal));

        foreach (var type in requestTypes)
        {
            var properties = type.GetProperties().Select(p => p.Name).ToList();
            properties.Should().NotContain(AuditFields, $"request {type.FullName} must not expose audit fields");
        }
    }

    [Fact]
    public void Responses_DoNotExpose_AuditFields()
    {
        var assembly = typeof(PatchCustomerRequest).Assembly;
        var responseTypes = assembly.GetTypes()
            .Where(t => t.IsPublic && t.Name.EndsWith("Response", StringComparison.Ordinal));

        foreach (var type in responseTypes)
        {
            var properties = type.GetProperties().Select(p => p.Name).ToList();
            properties.Should().NotContain(AuditFields, $"response {type.FullName} must not expose audit fields");
        }
    }
}
