using EBOS.CRM.Application.Contracts.Requests.CRM.BankInformation;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.Application.Contracts.Responses.Services;
using EBOS.CRM.Application.Features.CRM.BankInformation.Commands.AddBankInformation;
using EBOS.CRM.Application.Services.Interfaces;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using Moq;
using CRMBankInformation = EBOS.CRM.Domain.Entities.CRM.BankInformation;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.BankInformation.Commands.AddBankInformation;

public class AddBankInformationCommandHandlerTest
{
    private readonly Mock<IBankInformationRepository> _repositoryMock;
    private readonly Mock<IAuditService> _auditServiceMock;
    private readonly Mock<ICurrentUserContext> _currentUserMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly AddBankInformationCommandHandler _handler;

    public AddBankInformationCommandHandlerTest()
    {
        _repositoryMock = new Mock<IBankInformationRepository>();
        _auditServiceMock = new Mock<IAuditService>();
        _currentUserMock = new Mock<ICurrentUserContext>();
        _mapperMock = new Mock<IMapper>();

        _currentUserMock.SetupGet(x => x.UserId).Returns(1);
        _currentUserMock.SetupGet(x => x.CorrelationId).Returns("corr-1");

        _auditServiceMock.Setup(a => a.InsertAuditAsync(
                It.IsAny<EBOS.CRM.Application.Contracts.Requests.Services.AuditInsertRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuditInsertResponse(true, 1));

        _handler = new AddBankInformationCommandHandler(
            _repositoryMock.Object,
            _auditServiceMock.Object,
            _currentUserMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_WhenRequestIsValid_AddsAndAudits()
    {
        var request = BuildAddRequest();
        var entity = new CRMBankInformation();

        _mapperMock.Setup(m => m.Map<CRMBankInformation>(request)).Returns(entity);
        _mapperMock.Setup(m => m.Map<BankInformationResponse>(entity)).Returns(TestResponseFactory.Create<BankInformationResponse>());

        var result = await _handler.Handle(new AddBankInformationCommand(request), CancellationToken.None);

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
        var entity = new CRMBankInformation();

        _mapperMock.Setup(m => m.Map<CRMBankInformation>(request)).Returns(entity);
        _repositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("db error"));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.Handle(new AddBankInformationCommand(request), CancellationToken.None));

        _repositoryMock.Verify(r => r.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

private static AddBankInformationRequest BuildAddRequest() => new(
        Iban: "ES1200000000000000000000",
        Bic: "BANKESMM",
        BankName: "Bank",
        CustomerId: 1
    );
}