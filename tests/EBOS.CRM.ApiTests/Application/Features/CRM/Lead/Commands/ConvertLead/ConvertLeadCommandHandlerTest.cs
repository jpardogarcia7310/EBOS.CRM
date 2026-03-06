using EBOS.CRM.Application.Features.CRM.Lead.Commands.ConvertLead;
using EBOS.CRM.Application.Shared.Audit;
using EBOS.CRM.Contracts.Requests.CRM.Lead;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Contracts.Responses.Services;
using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using EBOS.CRM.Domain.Interfaces.Services.CRM;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Lead.Commands.ConvertLead;

public class ConvertLeadCommandHandlerTest
{
    [Fact]
    public async Task Handle_WhenLeadNotFound_ReturnsNull()
    {
        var leadRepo = new Mock<ILeadRepository>();
        var opportunityRepo = new Mock<IOpportunityRepository>();
        var audit = new Mock<IAuditService>();
        var currentUser = new Mock<ICurrentUserContext>();
        var mapper = new Mock<IMapper>();
        var conversionValidation = new Mock<ILeadConversionValidationService>();
        leadRepo.Setup(x => x.GetByIdAsync(404, It.IsAny<CancellationToken>()))
            .ReturnsAsync((global::EBOS.CRM.Domain.Entities.CRM.Lead?)null);

        var handler = new ConvertLeadCommandHandler(leadRepo.Object, opportunityRepo.Object, audit.Object, currentUser.Object, mapper.Object, conversionValidation.Object);
        var result = await handler.Handle(new ConvertLeadCommand(404, new ConvertLeadRequest(1, 2, 3, "Opp", 100m, 0.5m, null, null)), CancellationToken.None);
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WhenValid_ConvertsLead()
    {
        var leadRepo = new Mock<ILeadRepository>();
        var opportunityRepo = new Mock<IOpportunityRepository>();
        var audit = new Mock<IAuditService>();
        var currentUser = new Mock<ICurrentUserContext>();
        var mapper = new Mock<IMapper>();
        var conversionValidation = new Mock<ILeadConversionValidationService>();

        currentUser.SetupGet(x => x.UserId).Returns(1);
        currentUser.SetupGet(x => x.CorrelationId).Returns("corr-1");
        audit.Setup(x => x.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuditInsertResponse(true, 1));

        var lead = new global::EBOS.CRM.Domain.Entities.CRM.Lead
        {
            Id = 1, TenantId = 1, Source = "WEB", Status = "NEW", OwnerUserId = 5, CompanyName = "ACME", ContactName = "John", Email = "john@acme.com", Phone = "111"
        };
        leadRepo.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(lead);
        conversionValidation.Setup(x => x.EnsureDependenciesAvailableAsync(1, 2, 3, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        mapper.Setup(x => x.Map<OpportunityResponse>(It.IsAny<global::EBOS.CRM.Domain.Entities.CRM.Opportunity>()))
            .Returns(new OpportunityResponse(10, 1, "Opp", 3, 5, 2, null, 100m, 0.5m, "WEB", 1, null, true));

        var handler = new ConvertLeadCommandHandler(leadRepo.Object, opportunityRepo.Object, audit.Object, currentUser.Object, mapper.Object, conversionValidation.Object);
        var result = await handler.Handle(new ConvertLeadCommand(1, new ConvertLeadRequest(1, 2, 3, "Opp", 100m, 0.5m, null, null)), CancellationToken.None);

        Assert.NotNull(result);
        opportunityRepo.Verify(x => x.AddAsync(It.IsAny<global::EBOS.CRM.Domain.Entities.CRM.Opportunity>(), It.IsAny<CancellationToken>()), Times.Once);
        leadRepo.Verify(x => x.UpdateAsync(lead, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenDependencyResolutionIsTransient_ThrowsTransientDomainFailure()
    {
        var leadRepo = new Mock<ILeadRepository>();
        var opportunityRepo = new Mock<IOpportunityRepository>();
        var audit = new Mock<IAuditService>();
        var currentUser = new Mock<ICurrentUserContext>();
        var mapper = new Mock<IMapper>();
        var conversionValidation = new Mock<ILeadConversionValidationService>();

        var lead = new global::EBOS.CRM.Domain.Entities.CRM.Lead
        {
            Id = 1, TenantId = 1, Source = "WEB", Status = "NEW", OwnerUserId = 5, CompanyName = "ACME", ContactName = "John", Email = "john@acme.com", Phone = "111"
        };
        leadRepo.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(lead);
        conversionValidation.Setup(x => x.EnsureDependenciesAvailableAsync(1, 2, 3, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new TransientDomainFailureException(
                "Transient failure while resolving lead conversion dependencies.",
                "DOMAIN_TRANSIENT_LEAD_CONVERSION_DEPENDENCY_RESOLUTION"));

        var handler = new ConvertLeadCommandHandler(leadRepo.Object, opportunityRepo.Object, audit.Object, currentUser.Object, mapper.Object, conversionValidation.Object);
        var command = new ConvertLeadCommand(1, new ConvertLeadRequest(1, 2, 3, "Opp", 100m, 0.5m, null, null));

        var ex = await Assert.ThrowsAsync<TransientDomainFailureException>(() => handler.Handle(command, CancellationToken.None));
        Assert.Equal("DOMAIN_TRANSIENT_LEAD_CONVERSION_DEPENDENCY_RESOLUTION", ex.Code);
    }
}
