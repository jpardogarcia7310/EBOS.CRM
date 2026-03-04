using System.Text.Json;
using EBOS.CRM.Contracts.Requests.CRM.CustomerPreference;
using EBOS.CRM.Contracts.Responses.CRM;

namespace EBOS.CRM.ApiTests.Contracts.CRM;

public class CustomerPreferenceContractsTest
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    [Fact]
    public void UpsertPreferenceRequest_DefaultCountryId_IsNull()
    {
        var dto = new UpsertCustomerPreferenceRequest(1, 2, 3, true);
        Assert.Null(dto.CountryId);
    }

    [Fact]
    public void UpsertPreferenceRequest_RoundTrip_PreservesAllFields()
    {
        var dto = new UpsertCustomerPreferenceRequest(1, 2, 3, false, 44);
        var payload = JsonSerializer.Serialize(dto, Json);
        var back = JsonSerializer.Deserialize<UpsertCustomerPreferenceRequest>(payload, Json);

        Assert.NotNull(back);
        Assert.Equal(dto.TenantId, back!.TenantId);
        Assert.Equal(dto.CustomerId, back.CustomerId);
        Assert.Equal(dto.ChannelId, back.ChannelId);
        Assert.Equal(dto.Preferred, back.Preferred);
        Assert.Equal(dto.CountryId, back.CountryId);
    }

    [Fact]
    public void PreferenceResponse_Deserialize_IgnoresUnknownFields_BackwardCompatible()
    {
        const string json = """
                            {
                              "id": 10,
                              "tenantId": 1,
                              "customerId": 2,
                              "channelId": 3,
                              "preferred": true,
                              "active": true,
                              "futureField": "ignored"
                            }
                            """;

        var dto = JsonSerializer.Deserialize<CustomerPreferenceResponse>(json, Json);
        Assert.NotNull(dto);
        Assert.Equal(10, dto!.Id);
        Assert.True(dto.Preferred);
        Assert.True(dto.Active);
    }
}
