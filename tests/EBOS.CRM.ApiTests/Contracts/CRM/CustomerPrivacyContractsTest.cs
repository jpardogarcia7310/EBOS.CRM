using System.Text.Json;
using EBOS.CRM.Contracts.Requests.CRM.CustomerPrivacy;
using EBOS.CRM.Contracts.Responses.CRM;

namespace EBOS.CRM.ApiTests.Contracts.CRM;

public class CustomerPrivacyContractsTest
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    [Fact]
    public void RegisterRequest_Defaults_AndRoundTrip_AreStable()
    {
        var dto = new RegisterCustomerPrivacyRequestRequest(1, 10, "ANONYMIZE", "gdpr");
        Assert.False(dto.ExecuteNow);

        var payload = JsonSerializer.Serialize(dto, Json);
        var back = JsonSerializer.Deserialize<RegisterCustomerPrivacyRequestRequest>(payload, Json);

        Assert.NotNull(back);
        Assert.Equal(dto.TenantId, back!.TenantId);
        Assert.Equal(dto.CustomerId, back.CustomerId);
        Assert.Equal(dto.RequestType, back.RequestType);
        Assert.Equal(dto.Reason, back.Reason);
        Assert.Equal(dto.ExecuteNow, back.ExecuteNow);
    }

    [Fact]
    public void RetryRequest_DefaultReason_IsNull()
    {
        var dto = new RetryCustomerPrivacyRequestRequest(3);
        Assert.Null(dto.Reason);
    }

    [Fact]
    public void PrivacyResponse_Deserialize_IgnoresUnknownFields_BackwardCompatible()
    {
        const string json = """
                            {
                              "id": 1,
                              "tenantId": 2,
                              "customerId": 3,
                              "requestType": "ANONYMIZE",
                              "status": "COMPLETED",
                              "reason": "gdpr",
                              "requestedBy": 9,
                              "requestedAt": "2026-03-04T10:00:00Z",
                              "processedBy": 10,
                              "processedAt": "2026-03-04T10:01:00Z",
                              "failureCode": null,
                              "failureReason": null,
                              "correlationId": "corr-1",
                              "futureField": "must-be-ignored"
                            }
                            """;

        var dto = JsonSerializer.Deserialize<CustomerPrivacyRequestResponse>(json, Json);
        Assert.NotNull(dto);
        Assert.Equal(1, dto!.Id);
        Assert.Equal("COMPLETED", dto.Status);
        Assert.Equal("corr-1", dto.CorrelationId);
    }
}
