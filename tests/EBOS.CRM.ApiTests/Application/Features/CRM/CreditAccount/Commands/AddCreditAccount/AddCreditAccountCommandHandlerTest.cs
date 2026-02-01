using EBOS.CRM.Application.Contracts.Requests.CRM.CreditAccount;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.Application.Contracts.Responses.Services;
using EBOS.CRM.Application.Features.CRM.CreditAccount.Commands.AddCreditAccount;
using EBOS.CRM.Application.Services.Interfaces;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using Moq;
using CRMCreditAccount = EBOS.CRM.Domain.Entities.CRM.CreditAccount;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CreditAccount.Commands.AddCreditAccount;

public class AddCreditAccountCommandHandlerTest
{
    private readonly Mock<ICreditAccountRepository> _repositoryMock;
    private readonly Mock<IAuditService> _auditServiceMock;
    private readonly Mock<ICurrentUserContext> _currentUserMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly AddCreditAccountCommandHandler _handler;

    public AddCreditAccountCommandHandlerTest()
    {
        _repositoryMock = new Mock<ICreditAccountRepository>();
        _auditServiceMock = new Mock<IAuditService>();
        _currentUserMock = new Mock<ICurrentUserContext>();
        _mapperMock = new Mock<IMapper>();

        _currentUserMock.SetupGet(x => x.UserId).Returns(1);
        _currentUserMock.SetupGet(x => x.CorrelationId).Returns("corr-1");

        _auditServiceMock.Setup(a => a.InsertAuditAsync(
                It.IsAny<EBOS.CRM.Application.Contracts.Requests.Services.AuditInsertRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuditInsertResponse(true, 1));

        _handler = new AddCreditAccountCommandHandler(
            _repositoryMock.Object,
            _auditServiceMock.Object,
            _currentUserMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_WhenRequestIsValid_AddsAndAudits()
    {
        var request = BuildAddRequest();
        var entity = new CRMCreditAccount();

        _mapperMock.Setup(m => m.Map<CRMCreditAccount>(request)).Returns(entity);
        _mapperMock.Setup(m => m.Map<CreditAccountResponse>(entity)).Returns(TestResponseFactory.Create<CreditAccountResponse>());

        var result = await _handler.Handle(new AddCreditAccountCommand(request), CancellationToken.None);

        Assert.NotNull(result);
        _repositoryMock.Verify(r => r.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.AddAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _auditServiceMock.Verify(a => a.InsertAuditAsync(
            It.IsAny<EBOS.CRM.Application.Contracts.Requests.Services.AuditInsertRequest>(),
            It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenSaveChangesThrows_RollsBack()
    {
        var request = BuildAddRequest();
        var entity = new CRMCreditAccount();

        _mapperMock.Setup(m => m.Map<CRMCreditAccount>(request)).Returns(entity);
        _repositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("db error"));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.Handle(new AddCreditAccountCommand(request), CancellationToken.None));

        _repositoryMock.Verify(r => r.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

private static AddCreditAccountRequest BuildAddRequest() => new(
        MaxAmount: 1000m,
        UsedAmount: 100m,
        CustomerId: 1
    );
}