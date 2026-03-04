using System.Text.Json;
using EBOS.CRM.Contracts.Requests.CRM.CustomerMerge;
using EBOS.CRM.Contracts.Responses.CRM;

namespace EBOS.CRM.ApiTests.Contracts.CRM;

public class CustomerMergeContractsTest
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    [Fact]
    public void MergeRequest_RoundTrip_PreservesCollectionAndNullableReason()
    {
        var dto = new MergeCustomersRequest(1, 100, new List<long> { 200, 300 }, null);
        var payload = JsonSerializer.Serialize(dto, Json);
        var back = JsonSerializer.Deserialize<MergeCustomersRequest>(payload, Json);

        Assert.NotNull(back);
        Assert.Equal(1, back!.TenantId);
        Assert.Equal(100, back.WinnerCustomerId);
        Assert.Equal(2, back.MergeCustomerIds.Count);
        Assert.Contains(200, back.MergeCustomerIds);
        Assert.Contains(300, back.MergeCustomerIds);
        Assert.Null(back.Reason);
    }

    [Fact]
    public void FindDuplicatesRequest_RoundTrip_PreservesOptionalFilters()
    {
        var dto = new FindCustomerDuplicatesRequest(1, "a@b.com", null, "TAX-1", null);
        var payload = JsonSerializer.Serialize(dto, Json);
        var back = JsonSerializer.Deserialize<FindCustomerDuplicatesRequest>(payload, Json);

        Assert.NotNull(back);
        Assert.Equal(dto.Email, back!.Email);
        Assert.Equal(dto.Phone, back.Phone);
        Assert.Equal(dto.TaxId, back.TaxId);
        Assert.Equal(dto.IdentificationNumber, back.IdentificationNumber);
    }

    [Fact]
    public void MergeResponse_Deserialize_IgnoresUnknownFields_BackwardCompatible()
    {
        const string json = """
                            {
                              "winnerCustomerId": 7,
                              "mergedCustomerIds": [8,9],
                              "status": "Merged",
                              "futureField": { "x": 1 }
                            }
                            """;

        var dto = JsonSerializer.Deserialize<CustomerMergeResultResponse>(json, Json);
        Assert.NotNull(dto);
        Assert.Equal(7, dto!.WinnerCustomerId);
        Assert.Equal(2, dto.MergedCustomerIds.Count);
        Assert.Equal("Merged", dto.Status);
    }
}
