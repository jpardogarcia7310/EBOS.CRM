using EBOS.CRM.Application.Features.CRM.Lead.Commands.AddLead;
using EBOS.CRM.Contracts.Requests.CRM.Lead;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Contracts.Responses.Services;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Lead.Commands.AddLead;

public class AddLeadCommandHandlerTest
{
    [Fact]
    public async Task Handle_ValidRequest_PersistsAndReturnsResponse()
    {
        var repository = new Mock<ILeadRepository>();
        var auditService = new Mock<IAuditService>();
        var currentUser = new Mock<ICurrentUserContext>();
        var mapper = new Mock<IMapper>();

        currentUser.SetupGet(x => x.UserId).Returns(1);
        currentUser.SetupGet(x => x.CorrelationId).Returns("corr-1");
        auditService.Setup(x => x.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuditInsertResponse(true, 1));
        mapper.Setup(x => x.Map<global::EBOS.CRM.Domain.Entities.CRM.Lead>(It.IsAny<AddLeadRequest>()))
            .Returns(new global::EBOS.CRM.Domain.Entities.CRM.Lead { Id = 10, TenantId = 1, Source = "WEB", Status = "NEW", OwnerUserId = 5, CompanyName = "ACME", ContactName = "John", Email = "john@acme.com", Phone = "111" });
        mapper.Setup(x => x.Map<LeadResponse>(It.IsAny<global::EBOS.CRM.Domain.Entities.CRM.Lead>()))
            .Returns(new LeadResponse(10, 1, "WEB", "NEW", 5, "ACME", "John", "john@acme.com", "111", null, null, null, true));

        var handler = new AddLeadCommandHandler(repository.Object, auditService.Object, currentUser.Object, mapper.Object);
        var result = await handler.Handle(new AddLeadCommand(new AddLeadRequest(1, "WEB", "NEW", 5, "ACME", "John", "john@acme.com", "111", null, null)), CancellationToken.None);

        Assert.NotNull(result);
        repository.Verify(x => x.AddAsync(It.IsAny<global::EBOS.CRM.Domain.Entities.CRM.Lead>(), It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        auditService.Verify(x => x.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
