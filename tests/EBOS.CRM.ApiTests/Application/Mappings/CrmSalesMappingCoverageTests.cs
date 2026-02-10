using EBOS.CRM.ApiTests.Fixtures;
using EBOS.CRM.Application.Contracts.Requests.CRM.Lead;
using EBOS.CRM.Application.Contracts.Requests.CRM.Opportunity;
using EBOS.CRM.Application.Contracts.Requests.CRM.OpportunityStage;
using EBOS.CRM.Application.Contracts.Requests.CRM.Quote;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Entities.CRM;
using FluentAssertions;
using MapsterMapper;

namespace EBOS.CRM.ApiTests.Application.Mappings;

public class CrmSalesMappingCoverageTests(MapperFixture fixture) : IClassFixture<MapperFixture>
{
    private readonly IMapper _mapper = fixture.Mapper;

    [Fact]
    public void Lead_Mapping_Covers_All_Fields()
    {
        var request = new AddLeadRequest(1, "Web", "New", 10, "Acme", "Jane Doe",
            "lead@test.com", "123456", 1000m, "Notes");
        var entity = _mapper.Map<Lead>(request);
        entity.TenantId.Should().Be(1);
        entity.Source.Should().Be("Web");
        entity.Status.Should().Be("New");
        entity.OwnerUserId.Should().Be(10);
        entity.CompanyName.Should().Be("Acme");
        entity.ContactName.Should().Be("Jane Doe");
        entity.Email.Should().Be("lead@test.com");
        entity.Phone.Should().Be("123456");
        entity.EstimatedValue.Should().Be(1000m);
        entity.Notes.Should().Be("Notes");

        var response = _mapper.Map<LeadResponse>(entity);
        response.TenantId.Should().Be(1);
        response.CompanyName.Should().Be("Acme");
        response.Active.Should().BeTrue();
    }

    [Fact]
    public void Opportunity_Mapping_Covers_All_Fields()
    {
        var request = new AddOpportunityRequest(1, "Deal A", 3, 10, 20, DateTime.UtcNow.Date,
            5000m, 0.35m, "Referral", 11);
        var entity = _mapper.Map<Opportunity>(request);
        entity.TenantId.Should().Be(1);
        entity.Name.Should().Be("Deal A");
        entity.StageId.Should().Be(3);
        entity.OwnerUserId.Should().Be(10);
        entity.CustomerId.Should().Be(20);
        entity.Amount.Should().Be(5000m);
        entity.Probability.Should().Be(0.35m);
        entity.Source.Should().Be("Referral");
        entity.SourceLeadId.Should().Be(11);

        var response = _mapper.Map<OpportunityResponse>(entity);
        response.TenantId.Should().Be(1);
        response.Name.Should().Be("Deal A");
        response.Active.Should().BeTrue();
    }

    [Fact]
    public void OpportunityStage_Mapping_Covers_All_Fields()
    {
        var request = new AddOpportunityStageRequest(1, "Prospecting", 1, 0.1m, false, false);
        var entity = _mapper.Map<OpportunityStage>(request);
        entity.TenantId.Should().Be(1);
        entity.Name.Should().Be("Prospecting");
        entity.Order.Should().Be(1);
        entity.DefaultProbability.Should().Be(0.1m);
        entity.IsClosed.Should().BeFalse();
        entity.IsWon.Should().BeFalse();

        var response = _mapper.Map<OpportunityStageResponse>(entity);
        response.TenantId.Should().Be(1);
        response.Name.Should().Be("Prospecting");
        response.Active.Should().BeTrue();
    }

    [Fact]
    public void Quote_Mapping_Covers_All_Fields()
    {
        var request = new AddQuoteRequest(1, 10, "Draft", "Q-1001", 1000m, 50m, 950m, "Notes");
        var entity = _mapper.Map<Quote>(request);
        entity.TenantId.Should().Be(1);
        entity.OpportunityId.Should().Be(10);
        entity.Status.Should().Be("Draft");
        entity.ReferenceNumber.Should().Be("Q-1001");
        entity.SubtotalAmount.Should().Be(1000m);
        entity.DiscountAmount.Should().Be(50m);
        entity.TotalAmount.Should().Be(950m);
        entity.Notes.Should().Be("Notes");

        var response = _mapper.Map<QuoteResponse>(entity);
        response.TenantId.Should().Be(1);
        response.OpportunityId.Should().Be(10);
        response.Active.Should().BeTrue();
    }
}
