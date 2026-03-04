using EBOS.CRM.Application.Features.CRM.CustomerPreference.Commands.UpsertCustomerPreference;
using EBOS.CRM.Application.Shared.Audit;
using EBOS.CRM.Contracts.Requests.CRM.CustomerPreference;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Contracts.Responses.Services;
using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Entities.EBOS;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using EBOS.CRM.Domain.Interfaces.Services;
using MapsterMapper;
using Moq;
using CustomerEntity = EBOS.CRM.Domain.Entities.CRM.Customer;
using CustomerPreferenceEntity = EBOS.CRM.Domain.Entities.CRM.CustomerPreference;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CustomerPreference.Commands.UpsertCustomerPreference;

public class UpsertCustomerPreferenceCommandHandlerTest
{
    [Fact]
    public async Task Handle_NewPreference_AddsAndReturnsResponse()
    {
        var repository = new Mock<ICustomerPreferenceRepository>();
        var customerRepo = new Mock<ICustomerRepository>();
        var channelRepo = new Mock<IChannelTypeRepository>();
        var auditService = new Mock<IAuditService>();
        var currentUser = new Mock<ICurrentUserContext>();
        var mapper = new Mock<IMapper>();

        currentUser.SetupGet(x => x.UserId).Returns(1);
        currentUser.SetupGet(x => x.CorrelationId).Returns("corr-1");
        auditService.Setup(x => x.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuditInsertResponse(true, 1));
        customerRepo.Setup(x => x.GetByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CustomerEntity { Id = 2, TenantId = 1 });
        channelRepo.Setup(x => x.GetByIdAsync(3, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ChannelType { Id = 3, IsActive = true });
        repository.Setup(x => x.GetByCustomerAndChannelAsync(1, 2, 3, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CustomerPreferenceEntity?)null);
        mapper.Setup(x => x.Map<CustomerPreferenceResponse>(It.IsAny<CustomerPreferenceEntity>()))
            .Returns(new CustomerPreferenceResponse(1, 1, 2, 3, true, true));

        var handler = new UpsertCustomerPreferenceCommandHandler(
            repository.Object, customerRepo.Object, channelRepo.Object, auditService.Object, currentUser.Object, mapper.Object);

        var result = await handler.Handle(new UpsertCustomerPreferenceCommand(new UpsertCustomerPreferenceRequest(1, 2, 3, true)),
            CancellationToken.None);

        Assert.NotNull(result);
        repository.Verify(x => x.AddAsync(It.IsAny<CustomerPreferenceEntity>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
