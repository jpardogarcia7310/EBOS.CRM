using System.Text.Json;
using EBOS.CRM.Contracts.Requests.CRM.CustomerConsent;
using EBOS.CRM.Contracts.Responses.CRM;

namespace EBOS.CRM.ApiTests.Contracts.CRM;

public class CustomerConsentContractsTest
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    [Fact]
    public void AddConsentRequest_RoundTrip_PreservesDateAndOptionalExpiry()
    {
        var now = DateTime.UtcNow;
        var expires = now.AddDays(30);
        var dto = new AddCustomerConsentRequest(1, 2, "MARKETING_EMAIL", true, now, "web", expires);

        var payload = JsonSerializer.Serialize(dto, Json);
        var back = JsonSerializer.Deserialize<AddCustomerConsentRequest>(payload, Json);

        Assert.NotNull(back);
        Assert.Equal(dto.TenantId, back!.TenantId);
        Assert.Equal(dto.CustomerId, back.CustomerId);
        Assert.Equal(dto.ConsentType, back.ConsentType);
        Assert.Equal(dto.Granted, back.Granted);
        Assert.Equal(dto.Source, back.Source);
        Assert.Equal(dto.ExpiresAt, back.ExpiresAt);
    }

    [Fact]
    public void RevokeConsentRequest_RoundTrip_PreservesRequiredFields()
    {
        var dto = new RevokeCustomerConsentRequest(9, DateTime.UtcNow);
        var payload = JsonSerializer.Serialize(dto, Json);
        var back = JsonSerializer.Deserialize<RevokeCustomerConsentRequest>(payload, Json);

        Assert.NotNull(back);
        Assert.Equal(dto.TenantId, back!.TenantId);
        Assert.Equal(dto.RevokedAt, back.RevokedAt);
    }

    [Fact]
    public void ConsentResponse_Deserialize_IgnoresUnknownFields_BackwardCompatible()
    {
        const string json = """
                            {
                              "id": 1,
                              "tenantId": 2,
                              "customerId": 3,
                              "consentType": "MARKETING_EMAIL",
                              "granted": true,
                              "grantedAt": "2026-03-04T10:00:00Z",
                              "source": "web",
                              "expiresAt": null,
                              "revokedAt": null,
                              "active": true,
                              "futureField": "ignored"
                            }
                            """;

        var dto = JsonSerializer.Deserialize<CustomerConsentResponse>(json, Json);
        Assert.NotNull(dto);
        Assert.Equal(1, dto!.Id);
        Assert.True(dto.Active);
    }
}
