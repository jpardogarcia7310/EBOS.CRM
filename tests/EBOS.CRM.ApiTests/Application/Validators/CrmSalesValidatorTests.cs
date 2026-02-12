using EBOS.CRM.Contracts.Requests.CRM.Lead;
using EBOS.CRM.Contracts.Requests.CRM.Opportunity;
using EBOS.CRM.Contracts.Requests.CRM.OpportunityStage;
using EBOS.CRM.Contracts.Requests.CRM.Quote;
using EBOS.CRM.Application.Features.CRM.Lead.Commands.AddLead;
using EBOS.CRM.Application.Features.CRM.Lead.Commands.ConvertLead;
using EBOS.CRM.Application.Features.CRM.Lead.Commands.DisqualifyLead;
using EBOS.CRM.Application.Features.CRM.Lead.Commands.QualifyLead;
using EBOS.CRM.Application.Features.CRM.Lead.Commands.UpdateLead;
using EBOS.CRM.Application.Features.CRM.Opportunity.Commands.AddOpportunity;
using EBOS.CRM.Application.Features.CRM.Opportunity.Commands.CloseOpportunity;
using EBOS.CRM.Application.Features.CRM.Opportunity.Commands.PatchOpportunityStage;
using EBOS.CRM.Application.Features.CRM.Opportunity.Commands.UpdateOpportunity;
using EBOS.CRM.Application.Features.CRM.OpportunityStage.Commands.AddOpportunityStage;
using EBOS.CRM.Application.Features.CRM.OpportunityStage.Commands.UpdateOpportunityStage;
using EBOS.CRM.Application.Features.CRM.Quote.Commands.AddQuote;
using EBOS.CRM.Application.Features.CRM.Quote.Commands.UpdateQuote;
using FluentAssertions;

namespace EBOS.CRM.ApiTests.Application.Validators;

public class CrmSalesValidatorTests
{
    [Fact]
    public void AddLead_Validates_All_Fields()
    {
        var validator = new AddLeadCommandValidator();
        var request = new AddLeadRequest(1, "Web", "New", 10, "Acme", "Jane Doe",
            "lead@test.com", "123456", 100m, "Notes");
        validator.Validate(new AddLeadCommand(request)).IsValid.Should().BeTrue();

        validator.Validate(new AddLeadCommand(request with { Source = "" })).IsValid.Should().BeFalse();
        validator.Validate(new AddLeadCommand(request with { Status = "" })).IsValid.Should().BeFalse();
        validator.Validate(new AddLeadCommand(request with { OwnerUserId = 0 })).IsValid.Should().BeFalse();
        validator.Validate(new AddLeadCommand(request with { CompanyName = "" })).IsValid.Should().BeFalse();
        validator.Validate(new AddLeadCommand(request with { ContactName = "" })).IsValid.Should().BeFalse();
        validator.Validate(new AddLeadCommand(request with { Email = "" })).IsValid.Should().BeFalse();
        validator.Validate(new AddLeadCommand(request with { Phone = "" })).IsValid.Should().BeFalse();
        validator.Validate(new AddLeadCommand(request with { EstimatedValue = -1m })).IsValid.Should().BeFalse();
        validator.Validate(new AddLeadCommand(request with { Notes = Long(2001) })).IsValid.Should().BeFalse();
        validator.Validate(new AddLeadCommand(request with { Source = Long(101) })).IsValid.Should().BeFalse();
        validator.Validate(new AddLeadCommand(request with { Status = Long(51) })).IsValid.Should().BeFalse();
        validator.Validate(new AddLeadCommand(request with { CompanyName = Long(201) })).IsValid.Should().BeFalse();
        validator.Validate(new AddLeadCommand(request with { ContactName = Long(151) })).IsValid.Should().BeFalse();
        validator.Validate(new AddLeadCommand(request with { Email = Long(101) })).IsValid.Should().BeFalse();
        validator.Validate(new AddLeadCommand(request with { Phone = Long(21) })).IsValid.Should().BeFalse();
    }

    [Fact]
    public void UpdateLead_Validates_All_Fields()
    {
        var validator = new UpdateLeadCommandValidator();
        var request = new UpdateLeadRequest(5, 1, "Web", "New", 10, "Acme", "Jane Doe",
            "lead@test.com", "123456", 100m, "Notes");
        validator.Validate(new UpdateLeadCommand(5, request)).IsValid.Should().BeTrue();

        validator.Validate(new UpdateLeadCommand(0, request)).IsValid.Should().BeFalse();
        validator.Validate(new UpdateLeadCommand(5, request with { Id = 0 })).IsValid.Should().BeFalse();
        validator.Validate(new UpdateLeadCommand(5, request with { Source = "" })).IsValid.Should().BeFalse();
        validator.Validate(new UpdateLeadCommand(5, request with { Status = "" })).IsValid.Should().BeFalse();
        validator.Validate(new UpdateLeadCommand(5, request with { OwnerUserId = 0 })).IsValid.Should().BeFalse();
        validator.Validate(new UpdateLeadCommand(5, request with { CompanyName = "" })).IsValid.Should().BeFalse();
        validator.Validate(new UpdateLeadCommand(5, request with { ContactName = "" })).IsValid.Should().BeFalse();
        validator.Validate(new UpdateLeadCommand(5, request with { Email = "" })).IsValid.Should().BeFalse();
        validator.Validate(new UpdateLeadCommand(5, request with { Phone = "" })).IsValid.Should().BeFalse();
        validator.Validate(new UpdateLeadCommand(5, request with { EstimatedValue = -1m })).IsValid.Should().BeFalse();
        validator.Validate(new UpdateLeadCommand(5, request with { Notes = Long(2001) })).IsValid.Should().BeFalse();
    }

    [Fact]
    public void QualifyLead_Validates_All_Fields()
    {
        var validator = new QualifyLeadCommandValidator();
        var request = new QualifyLeadRequest(1, "Notes");
        validator.Validate(new QualifyLeadCommand(5, request)).IsValid.Should().BeTrue();

        validator.Validate(new QualifyLeadCommand(0, request)).IsValid.Should().BeFalse();
        validator.Validate(new QualifyLeadCommand(5, request with { Notes = Long(2001) }))
            .IsValid.Should().BeFalse();
    }

    [Fact]
    public void DisqualifyLead_Validates_All_Fields()
    {
        var validator = new DisqualifyLeadCommandValidator();
        var request = new DisqualifyLeadRequest(1, "Reason");
        validator.Validate(new DisqualifyLeadCommand(5, request)).IsValid.Should().BeTrue();

        validator.Validate(new DisqualifyLeadCommand(0, request)).IsValid.Should().BeFalse();
        validator.Validate(new DisqualifyLeadCommand(5, request with { Reason = "" })).IsValid.Should().BeFalse();
        validator.Validate(new DisqualifyLeadCommand(5, request with { Reason = Long(2001) }))
            .IsValid.Should().BeFalse();
    }

    [Fact]
    public void ConvertLead_Validates_All_Fields()
    {
        var validator = new ConvertLeadCommandValidator();
        var request = new ConvertLeadRequest(1, 10, 2, "Deal", 1000m, 0.5m, DateTime.UtcNow, "Notes");
        validator.Validate(new ConvertLeadCommand(5, request)).IsValid.Should().BeTrue();

        validator.Validate(new ConvertLeadCommand(0, request)).IsValid.Should().BeFalse();
        validator.Validate(new ConvertLeadCommand(5, request with { CustomerId = 0 })).IsValid.Should().BeFalse();
        validator.Validate(new ConvertLeadCommand(5, request with { StageId = 0 })).IsValid.Should().BeFalse();
        validator.Validate(new ConvertLeadCommand(5, request with { OpportunityName = "" })).IsValid.Should().BeFalse();
        validator.Validate(new ConvertLeadCommand(5, request with { Amount = -1m })).IsValid.Should().BeFalse();
        validator.Validate(new ConvertLeadCommand(5, request with { Probability = 2m })).IsValid.Should().BeFalse();
        validator.Validate(new ConvertLeadCommand(5, request with { Notes = Long(2001) }))
            .IsValid.Should().BeFalse();
        validator.Validate(new ConvertLeadCommand(5, request with { OpportunityName = Long(201) }))
            .IsValid.Should().BeFalse();
    }

    [Fact]
    public void AddOpportunity_Validates_All_Fields()
    {
        var validator = new AddOpportunityCommandValidator();
        var request = new AddOpportunityRequest(1, "Deal", 2, 10, 20, DateTime.UtcNow, 1000m, 0.25m, "Web", null);
        validator.Validate(new AddOpportunityCommand(request)).IsValid.Should().BeTrue();

        validator.Validate(new AddOpportunityCommand(request with { Name = "" })).IsValid.Should().BeFalse();
        validator.Validate(new AddOpportunityCommand(request with { StageId = 0 })).IsValid.Should().BeFalse();
        validator.Validate(new AddOpportunityCommand(request with { OwnerUserId = 0 })).IsValid.Should().BeFalse();
        validator.Validate(new AddOpportunityCommand(request with { CustomerId = 0 })).IsValid.Should().BeFalse();
        validator.Validate(new AddOpportunityCommand(request with { Amount = -1m })).IsValid.Should().BeFalse();
        validator.Validate(new AddOpportunityCommand(request with { Probability = 2m })).IsValid.Should().BeFalse();
        validator.Validate(new AddOpportunityCommand(request with { Name = Long(201) })).IsValid.Should().BeFalse();
        validator.Validate(new AddOpportunityCommand(request with { Source = Long(101) })).IsValid.Should().BeFalse();
    }

    [Fact]
    public void UpdateOpportunity_Validates_All_Fields()
    {
        var validator = new UpdateOpportunityCommandValidator();
        var request = new UpdateOpportunityRequest(5, 1, "Deal", 2, 10, 20, DateTime.UtcNow, 1000m, 0.25m, "Web", null, "Reason");
        validator.Validate(new UpdateOpportunityCommand(5, request)).IsValid.Should().BeTrue();

        validator.Validate(new UpdateOpportunityCommand(0, request)).IsValid.Should().BeFalse();
        validator.Validate(new UpdateOpportunityCommand(5, request with { Id = 0 })).IsValid.Should().BeFalse();
        validator.Validate(new UpdateOpportunityCommand(5, request with { Name = "" })).IsValid.Should().BeFalse();
        validator.Validate(new UpdateOpportunityCommand(5, request with { StageId = 0 })).IsValid.Should().BeFalse();
        validator.Validate(new UpdateOpportunityCommand(5, request with { OwnerUserId = 0 })).IsValid.Should().BeFalse();
        validator.Validate(new UpdateOpportunityCommand(5, request with { CustomerId = 0 })).IsValid.Should().BeFalse();
        validator.Validate(new UpdateOpportunityCommand(5, request with { Amount = -1m })).IsValid.Should().BeFalse();
        validator.Validate(new UpdateOpportunityCommand(5, request with { Probability = 2m })).IsValid.Should().BeFalse();
        validator.Validate(new UpdateOpportunityCommand(5, request with { CloseReason = Long(501) }))
            .IsValid.Should().BeFalse();
    }

    [Fact]
    public void CloseOpportunity_Validates_All_Fields()
    {
        var validator = new CloseOpportunityCommandValidator();
        var request = new CloseOpportunityRequest(1, 2, true, "Reason");
        validator.Validate(new CloseOpportunityCommand(5, request)).IsValid.Should().BeTrue();

        validator.Validate(new CloseOpportunityCommand(0, request)).IsValid.Should().BeFalse();
        validator.Validate(new CloseOpportunityCommand(5, request with { StageId = 0 })).IsValid.Should().BeFalse();
        validator.Validate(new CloseOpportunityCommand(5, request with { CloseReason = Long(501) }))
            .IsValid.Should().BeFalse();
    }

    [Fact]
    public void PatchOpportunityStage_Validates_All_Fields()
    {
        var validator = new PatchOpportunityStageCommandValidator();
        var request = new PatchOpportunityStageRequest(1, 2, 0.5m);
        validator.Validate(new PatchOpportunityStageCommand(5, request)).IsValid.Should().BeTrue();

        validator.Validate(new PatchOpportunityStageCommand(0, request)).IsValid.Should().BeFalse();
        validator.Validate(new PatchOpportunityStageCommand(5, request with { StageId = 0 })).IsValid.Should().BeFalse();
        validator.Validate(new PatchOpportunityStageCommand(5, request with { Probability = 2m }))
            .IsValid.Should().BeFalse();
    }

    [Fact]
    public void AddOpportunityStage_Validates_All_Fields()
    {
        var validator = new AddOpportunityStageCommandValidator();
        var request = new AddOpportunityStageRequest(1, "Prospecting", 1, 0.1m, false, false);
        validator.Validate(new AddOpportunityStageCommand(request)).IsValid.Should().BeTrue();

        validator.Validate(new AddOpportunityStageCommand(request with { Name = "" })).IsValid.Should().BeFalse();
        validator.Validate(new AddOpportunityStageCommand(request with { Order = -1 })).IsValid.Should().BeFalse();
        validator.Validate(new AddOpportunityStageCommand(request with { DefaultProbability = 2m }))
            .IsValid.Should().BeFalse();
        validator.Validate(new AddOpportunityStageCommand(request with { Name = Long(101) }))
            .IsValid.Should().BeFalse();
    }

    [Fact]
    public void UpdateOpportunityStage_Validates_All_Fields()
    {
        var validator = new UpdateOpportunityStageCommandValidator();
        var request = new UpdateOpportunityStageRequest(5, 1, "Prospecting", 1, 0.1m, false, false);
        validator.Validate(new UpdateOpportunityStageCommand(5, request)).IsValid.Should().BeTrue();

        validator.Validate(new UpdateOpportunityStageCommand(0, request)).IsValid.Should().BeFalse();
        validator.Validate(new UpdateOpportunityStageCommand(5, request with { Id = 0 })).IsValid.Should().BeFalse();
        validator.Validate(new UpdateOpportunityStageCommand(5, request with { Name = "" })).IsValid.Should().BeFalse();
        validator.Validate(new UpdateOpportunityStageCommand(5, request with { Order = -1 })).IsValid.Should().BeFalse();
        validator.Validate(new UpdateOpportunityStageCommand(5, request with { DefaultProbability = 2m }))
            .IsValid.Should().BeFalse();
        validator.Validate(new UpdateOpportunityStageCommand(5, request with { Name = Long(101) }))
            .IsValid.Should().BeFalse();
    }

    [Fact]
    public void AddQuote_Validates_All_Fields()
    {
        var validator = new AddQuoteCommandValidator();
        var request = new AddQuoteRequest(1, 10, "Draft", "Q-1001", 1000m, 50m, 950m, null, "Notes");
        validator.Validate(new AddQuoteCommand(request)).IsValid.Should().BeTrue();

        validator.Validate(new AddQuoteCommand(request with { OpportunityId = 0 })).IsValid.Should().BeFalse();
        validator.Validate(new AddQuoteCommand(request with { Status = "" })).IsValid.Should().BeFalse();
        validator.Validate(new AddQuoteCommand(request with { Status = Long(51) })).IsValid.Should().BeFalse();
        validator.Validate(new AddQuoteCommand(request with { ReferenceNumber = Long(51) }))
            .IsValid.Should().BeFalse();
        validator.Validate(new AddQuoteCommand(request with { SubtotalAmount = -1m })).IsValid.Should().BeFalse();
        validator.Validate(new AddQuoteCommand(request with { DiscountAmount = -1m })).IsValid.Should().BeFalse();
        validator.Validate(new AddQuoteCommand(request with { TotalAmount = -1m })).IsValid.Should().BeFalse();
        validator.Validate(new AddQuoteCommand(request with { Notes = Long(2001) })).IsValid.Should().BeFalse();
        validator.Validate(new AddQuoteCommand(request with { DiscountAmount = 2000m })).IsValid.Should().BeFalse();
    }

    [Fact]
    public void UpdateQuote_Validates_All_Fields()
    {
        var validator = new UpdateQuoteCommandValidator();
        var request = new UpdateQuoteRequest(5, 1, 10, "Draft", "Q-1001", 1000m, 50m, 950m, null, "Notes");
        validator.Validate(new UpdateQuoteCommand(5, request)).IsValid.Should().BeTrue();

        validator.Validate(new UpdateQuoteCommand(0, request)).IsValid.Should().BeFalse();
        validator.Validate(new UpdateQuoteCommand(5, request with { Id = 0 })).IsValid.Should().BeFalse();
        validator.Validate(new UpdateQuoteCommand(5, request with { OpportunityId = 0 })).IsValid.Should().BeFalse();
        validator.Validate(new UpdateQuoteCommand(5, request with { Status = "" })).IsValid.Should().BeFalse();
        validator.Validate(new UpdateQuoteCommand(5, request with { Status = Long(51) })).IsValid.Should().BeFalse();
        validator.Validate(new UpdateQuoteCommand(5, request with { ReferenceNumber = Long(51) }))
            .IsValid.Should().BeFalse();
        validator.Validate(new UpdateQuoteCommand(5, request with { SubtotalAmount = -1m })).IsValid.Should().BeFalse();
        validator.Validate(new UpdateQuoteCommand(5, request with { DiscountAmount = -1m })).IsValid.Should().BeFalse();
        validator.Validate(new UpdateQuoteCommand(5, request with { TotalAmount = -1m })).IsValid.Should().BeFalse();
        validator.Validate(new UpdateQuoteCommand(5, request with { Notes = Long(2001) })).IsValid.Should().BeFalse();
        validator.Validate(new UpdateQuoteCommand(5, request with { DiscountAmount = 2000m })).IsValid.Should().BeFalse();
    }

    private static string Long(int length) => new('X', length);
}
