using EBOS.CRM.Application.Features.CRM.CustomerConsent.Commands.RevokeCustomerConsent;
using EBOS.CRM.Application.Shared.Audit;
using EBOS.CRM.Contracts.Requests.CRM.CustomerConsent;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Contracts.Responses.Services;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using EBOS.CRM.Domain.Interfaces.Services.CRM;
using MapsterMapper;
using Moq;
using CRMConsent = EBOS.CRM.Domain.Entities.CRM.CustomerConsent;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CustomerConsent.Commands.RevokeCustomerConsent;

public class RevokeCustomerConsentCommandHandlerTest
{
    [Fact]
    public async Task Handle_WhenFound_CreatesRevocationEvent()
    {
        var repository = new Mock<ICustomerConsentRepository>();
        var auditService = new Mock<IAuditService>();
        var currentUser = new Mock<ICurrentUserContext>();
        var metrics = new Mock<ICustomer360Metrics>();
        var mapper = new Mock<IMapper>();

        currentUser.SetupGet(x => x.UserId).Returns(1);
        currentUser.SetupGet(x => x.CorrelationId).Returns("corr-1");
        auditService.Setup(x => x.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuditInsertResponse(true, 1));

        var existing = CRMConsent.Create(1, 2, "EMAIL", true, DateTime.UtcNow.AddDays(-5), "api", null);
        existing.Id = 10;
        repository.Setup(x => x.GetByIdAsync(10, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
        mapper.Setup(x => x.Map<CustomerConsentResponse>(It.IsAny<CRMConsent>()))
            .Returns(new CustomerConsentResponse(11, 1, 2, "EMAIL", false, DateTime.UtcNow, "api", null, DateTime.UtcNow, false));

        var handler = new RevokeCustomerConsentCommandHandler(
            repository.Object, auditService.Object, currentUser.Object, metrics.Object, mapper.Object);

        var result = await handler.Handle(
            new RevokeCustomerConsentCommand(10, new RevokeCustomerConsentRequest(1, DateTime.UtcNow)),
            CancellationToken.None);

        Assert.NotNull(result);
        repository.Verify(x => x.AddAsync(It.IsAny<CRMConsent>(), It.IsAny<CancellationToken>()), Times.Once);
        metrics.Verify(x => x.RecordConsentEvent(1, "EMAIL", false), Times.Once);
    }
}
