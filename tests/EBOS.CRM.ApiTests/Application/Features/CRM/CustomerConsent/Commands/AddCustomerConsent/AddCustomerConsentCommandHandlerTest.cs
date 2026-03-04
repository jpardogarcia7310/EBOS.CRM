using EBOS.CRM.Application.Features.CRM.CustomerConsent.Commands.AddCustomerConsent;
using EBOS.CRM.Contracts.Requests.CRM.CustomerConsent;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Contracts.Responses.Services;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using EBOS.CRM.Domain.Interfaces.Services.CRM;
using MapsterMapper;
using Moq;
using CustomerEntity = EBOS.CRM.Domain.Entities.CRM.Customer;
using CustomerConsentEntity = EBOS.CRM.Domain.Entities.CRM.CustomerConsent;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CustomerConsent.Commands.AddCustomerConsent;

public class AddCustomerConsentCommandHandlerTest
{
    [Fact]
    public async Task Handle_ValidRequest_PersistsAndReturnsResponse()
    {
        var repository = new Mock<ICustomerConsentRepository>();
        var customerRepository = new Mock<ICustomerRepository>();
        var auditService = new Mock<IAuditService>();
        var currentUser = new Mock<ICurrentUserContext>();
        var metrics = new Mock<ICustomer360Metrics>();
        var mapper = new Mock<IMapper>();

        currentUser.SetupGet(x => x.UserId).Returns(1);
        currentUser.SetupGet(x => x.CorrelationId).Returns("corr-1");
        auditService.Setup(x => x.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuditInsertResponse(true, 1));
        customerRepository.Setup(x => x.GetByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CustomerEntity { Id = 2, TenantId = 1 });
        mapper.Setup(x => x.Map<CustomerConsentResponse>(It.IsAny<CustomerConsentEntity>()))
            .Returns(new CustomerConsentResponse(1, 1, 2, "EMAIL", true, DateTime.UtcNow, "api", null, null, true));

        var handler = new AddCustomerConsentCommandHandler(
            repository.Object, customerRepository.Object, auditService.Object, currentUser.Object, metrics.Object, mapper.Object);

        var cmd = new AddCustomerConsentCommand(new AddCustomerConsentRequest(1, 2, "EMAIL", true, DateTime.UtcNow, "api", null));
        var result = await handler.Handle(cmd, CancellationToken.None);

        Assert.NotNull(result);
        repository.Verify(x => x.AddAsync(It.IsAny<CustomerConsentEntity>(), It.IsAny<CancellationToken>()), Times.Once);
        metrics.Verify(x => x.RecordConsentEvent(1, "EMAIL", true), Times.Once);
    }
}
