using EBOS.CRM.Contracts.Requests.CRM.CustomerPrivacy;
using EBOS.CRM.Contracts.Responses.CRM;

namespace EBOS.CRM.ApiTests.Contracts.CRM;

public class Customer360ContractsCoverageTest
{
    [Fact]
    public void CustomerDuplicateCandidateResponse_StoresValues()
    {
        var dto = new CustomerDuplicateCandidateResponse(1, "EMAIL", 90);
        Assert.Equal(1, dto.CustomerId);
        Assert.Equal("EMAIL", dto.MatchReason);
        Assert.Equal(90, dto.Score);
    }

    [Fact]
    public void CustomerMergeHistoryResponse_StoresValues()
    {
        var dto = new CustomerMergeHistoryResponse(1, 2, 3, 4, "dedupe");
        Assert.Equal(1, dto.Id);
        Assert.Equal(2, dto.TenantId);
        Assert.Equal(3, dto.WinnerCustomerId);
        Assert.Equal(4, dto.MergedCustomerId);
        Assert.Equal("dedupe", dto.Reason);
    }

    [Fact]
    public void CustomerMergeResultResponse_StoresValues()
    {
        var dto = new CustomerMergeResultResponse(7, new List<long> { 8, 9 }, "Merged");
        Assert.Equal(7, dto.WinnerCustomerId);
        Assert.Equal(2, dto.MergedCustomerIds.Count);
        Assert.Equal("Merged", dto.Status);
    }

    [Fact]
    public void CustomerPrivacyRetentionRunResponse_StoresValues()
    {
        var cutoff = DateTime.UtcNow;
        var dto = new CustomerPrivacyRetentionRunResponse(1, true, 90, 500, cutoff, 10, 0);
        Assert.Equal(1, dto.TenantId);
        Assert.True(dto.DryRun);
        Assert.Equal(90, dto.RetentionDays);
        Assert.Equal(500, dto.BatchSize);
        Assert.Equal(cutoff, dto.CutoffUtc);
        Assert.Equal(10, dto.Candidates);
        Assert.Equal(0, dto.Affected);
    }

    [Fact]
    public void ForecastStageSummaryResponse_StoresValues()
    {
        var dto = new ForecastStageSummaryResponse(1, "Prospecting", 5, 1000m, 250m);
        Assert.Equal(1, dto.StageId);
        Assert.Equal("Prospecting", dto.StageName);
        Assert.Equal(5, dto.OpportunityCount);
        Assert.Equal(1000m, dto.TotalAmount);
        Assert.Equal(250m, dto.WeightedAmount);
    }

    [Fact]
    public void RunCustomerPrivacyRetentionRequest_StoresValues()
    {
        var dto = new RunCustomerPrivacyRetentionRequest(2, false, 60, 1000);
        Assert.Equal(2, dto.TenantId);
        Assert.False(dto.DryRun);
        Assert.Equal(60, dto.RetentionDays);
        Assert.Equal(1000, dto.BatchSize);
    }

    [Fact]
    public void SlaCheckResponse_StoresValues()
    {
        var due = DateTime.UtcNow;
        var dto = new SlaCheckResponse(1, 2, due, true, true);
        Assert.Equal(1, dto.CaseId);
        Assert.Equal(2, dto.SlaId);
        Assert.Equal(due, dto.DueAt);
        Assert.True(dto.IsBreached);
        Assert.True(dto.IsActive);
    }
}
