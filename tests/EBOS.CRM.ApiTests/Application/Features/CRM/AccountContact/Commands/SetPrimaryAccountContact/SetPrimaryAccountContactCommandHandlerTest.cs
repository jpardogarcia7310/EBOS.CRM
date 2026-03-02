using EBOS.CRM.Application.Features.CRM.AccountContact.Commands.SetPrimaryAccountContact;
using EBOS.CRM.Application.Shared.Audit;
using EBOS.CRM.Contracts.Requests.CRM.AccountContact;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Contracts.Responses.Services;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using EBOS.CRM.Domain.Interfaces.Services.CRM;
using MapsterMapper;
using Moq;
using CRMAccountContact = EBOS.CRM.Domain.Entities.CRM.AccountContact;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.AccountContact.Commands.SetPrimaryAccountContact;

public class SetPrimaryAccountContactCommandHandlerTest
{
    private readonly Mock<IAccountContactRepository> _repositoryMock;
    private readonly Mock<IAccountContactPrimaryGuard> _primaryGuardMock;
    private readonly Mock<IAuditService> _auditServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly SetPrimaryAccountContactCommandHandler _handler;

    public SetPrimaryAccountContactCommandHandlerTest()
    {
        _repositoryMock = new Mock<IAccountContactRepository>();
        _primaryGuardMock = new Mock<IAccountContactPrimaryGuard>();
        _auditServiceMock = new Mock<IAuditService>();
        var currentUserMock = new Mock<ICurrentUserContext>();
        _mapperMock = new Mock<IMapper>();

        currentUserMock.SetupGet(x => x.UserId).Returns(1);
        currentUserMock.SetupGet(x => x.CorrelationId).Returns("corr-1");

        _auditServiceMock.Setup(a => a.InsertAuditAsync(
                It.IsAny<AuditInsertRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuditInsertResponse(true, 1));

        _handler = new SetPrimaryAccountContactCommandHandler(
            _repositoryMock.Object,
            _primaryGuardMock.Object,
            _auditServiceMock.Object,
            currentUserMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_WhenPrimaryTrue_DisablesOtherPrimariesAndAudits()
    {
        var request = new SetPrimaryAccountContactRequest(1, true);
        var now = DateTime.UtcNow;
        var entity = CRMAccountContact.Create(1, 20, 30, false, now, null, 1);
        entity.Id = 10;
        var otherPrimary = CRMAccountContact.Create(1, 20, 31, true, now, null, 1);
        otherPrimary.Id = 11;

        _repositoryMock.Setup(r => r.GetByIdAsync(entity.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);
        _primaryGuardMock
            .Setup(g => g.GetOtherPrimariesAsync(1, 20, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CRMAccountContact> { otherPrimary });

        _mapperMock.Setup(m => m.Map<AccountContactResponse>(entity))
            .Returns(new AccountContactResponse(entity.Id, entity.TenantId, entity.CorporateCustomerId,
                entity.IndividualCustomerId, true, entity.StartAt, entity.EndAt, true));

        var result = await _handler.Handle(new SetPrimaryAccountContactCommand(entity.Id, request),
            CancellationToken.None);

        Assert.NotNull(result);
        Assert.False(otherPrimary.IsPrimary);
        _repositoryMock.Verify(r => r.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.UpdateAsync(otherPrimary, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _auditServiceMock.Verify(a => a.InsertAuditAsync(
            It.IsAny<AuditInsertRequest>(),
            It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
