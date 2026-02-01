using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Application.Features.CRM.CreditAccount.Queries.GetCreditAccountById;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CreditAccount.Queries.GetCreditAccountById;

public class GetCreditAccountByIdQueryHandlerTest
{
    private readonly Mock<ICreditAccountRepository> _repositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    [Fact]
    public async Task Handle_WhenFound_Maps()
    {
        var handler = new GetCreditAccountByIdQueryHandler(_repositoryMock.Object, _mapperMock.Object);
        var entity = new EBOS.CRM.Domain.Entities.CRM.CreditAccount();

        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);
        _mapperMock.Setup(m => m.Map<CreditAccountResponse>(entity))
            .Returns((CreditAccountResponse)null!);

        await handler.Handle(new GetCreditAccountByIdQuery(1), CancellationToken.None);

        _mapperMock.Verify(m => m.Map<CreditAccountResponse>(entity), Times.Once);
    }
}