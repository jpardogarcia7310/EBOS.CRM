using EBOS.CRM.Application.Features.CRM.Lead.Commands.UpdateLead;
using EBOS.CRM.Application.Shared.Audit;
using EBOS.CRM.Contracts.Requests.CRM.Lead;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Contracts.Responses.Services;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Lead.Commands.UpdateLead;

public class UpdateLeadCommandHandlerTest
{
    [Fact]
    public async Task Handle_WhenNotFound_ReturnsNull()
    {
        var repository = new Mock<ILeadRepository>();
        var audit = new Mock<IAuditService>();
        var currentUser = new Mock<ICurrentUserContext>();
        var mapper = new Mock<IMapper>();
        repository.Setup(x => x.GetByIdAsync(404, It.IsAny<CancellationToken>()))
            .ReturnsAsync((global::EBOS.CRM.Domain.Entities.CRM.Lead?)null);

        var handler = new UpdateLeadCommandHandler(repository.Object, audit.Object, currentUser.Object, mapper.Object);
        var req = new UpdateLeadRequest(404, 1, "WEB", "NEW", 2, "ACME", "John", "john@acme.com", "111", null, null);
        var result = await handler.Handle(new UpdateLeadCommand(404, req), CancellationToken.None);
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WhenValid_UpdatesAndReturnsDto()
    {
        var repository = new Mock<ILeadRepository>();
        var audit = new Mock<IAuditService>();
        var currentUser = new Mock<ICurrentUserContext>();
        var mapper = new Mock<IMapper>();
        currentUser.SetupGet(x => x.UserId).Returns(1);
        currentUser.SetupGet(x => x.CorrelationId).Returns("corr-1");
        audit.Setup(x => x.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuditInsertResponse(true, 1));

        var lead = new global::EBOS.CRM.Domain.Entities.CRM.Lead { Id = 1, TenantId = 1, Source = "WEB", Status = "NEW", OwnerUserId = 2, CompanyName = "ACME", ContactName = "John", Email = "john@acme.com", Phone = "111" };
        repository.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(lead);
        mapper.Setup(x => x.Map(It.IsAny<UpdateLeadRequest>(), lead)).Callback(() => lead.Status = "WORKING");
        mapper.Setup(x => x.Map<LeadResponse>(lead))
            .Returns(new LeadResponse(1, 1, "WEB", "WORKING", 2, "ACME", "John", "john@acme.com", "111", null, null, null, true));

        var handler = new UpdateLeadCommandHandler(repository.Object, audit.Object, currentUser.Object, mapper.Object);
        var req = new UpdateLeadRequest(1, 1, "WEB", "WORKING", 2, "ACME", "John", "john@acme.com", "111", null, null);
        var result = await handler.Handle(new UpdateLeadCommand(1, req), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("WORKING", result!.Status);
        repository.Verify(x => x.UpdateAsync(lead, It.IsAny<CancellationToken>()), Times.Once);
    }
}
