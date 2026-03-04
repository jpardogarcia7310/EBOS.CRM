using EBOS.CRM.Application.Features.CRM.Lead.Commands.DisqualifyLead;
using EBOS.CRM.Contracts.Requests.CRM.Lead;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Contracts.Responses.Services;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Lead.Commands.DisqualifyLead;

public class DisqualifyLeadCommandHandlerTest
{
    [Fact]
    public async Task Handle_WhenNotFound_ReturnsNull()
    {
        var repository = new Mock<ILeadRepository>();
        var audit = new Mock<IAuditService>();
        var currentUser = new Mock<ICurrentUserContext>();
        repository.Setup(x => x.GetByIdAsync(404, It.IsAny<CancellationToken>()))
            .ReturnsAsync((global::EBOS.CRM.Domain.Entities.CRM.Lead?)null);

        var handler = new DisqualifyLeadCommandHandler(repository.Object, audit.Object, currentUser.Object);
        var result = await handler.Handle(new DisqualifyLeadCommand(404, new DisqualifyLeadRequest(1, "reason")), CancellationToken.None);
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WhenValid_UpdatesStatus()
    {
        var repository = new Mock<ILeadRepository>();
        var audit = new Mock<IAuditService>();
        var currentUser = new Mock<ICurrentUserContext>();
        currentUser.SetupGet(x => x.UserId).Returns(1);
        currentUser.SetupGet(x => x.CorrelationId).Returns("corr-1");
        audit.Setup(x => x.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuditInsertResponse(true, 1));

        var lead = new global::EBOS.CRM.Domain.Entities.CRM.Lead { Id = 1, TenantId = 1, Source = "WEB", Status = "NEW", OwnerUserId = 2, CompanyName = "ACME", ContactName = "John", Email = "john@acme.com", Phone = "111" };
        repository.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(lead);

        var handler = new DisqualifyLeadCommandHandler(repository.Object, audit.Object, currentUser.Object);
        var result = await handler.Handle(new DisqualifyLeadCommand(1, new DisqualifyLeadRequest(1, "bad lead")), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("Disqualified", result.Status);
        repository.Verify(x => x.UpdateAsync(lead, It.IsAny<CancellationToken>()), Times.Once);
    }
}
