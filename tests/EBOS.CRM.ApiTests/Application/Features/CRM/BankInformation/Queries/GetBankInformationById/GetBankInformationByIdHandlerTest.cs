using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Application.Features.CRM.BankInformation.Queries.GetBankInformationById;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.BankInformation.Queries.GetBankInformationById;

public class GetBankInformationByIdQueryHandlerTest
{
    private readonly Mock<IBankInformationRepository> _repositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    [Fact]
    public async Task Handle_WhenFound_Maps()
    {
        var handler = new GetBankInformationByIdQueryHandler(_repositoryMock.Object, _mapperMock.Object);
        var entity = new EBOS.CRM.Domain.Entities.CRM.BankInformation();

        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);
        _mapperMock.Setup(m => m.Map<BankInformationResponse>(entity))
            .Returns((BankInformationResponse)null!);

        await handler.Handle(new GetBankInformationByIdQuery(1), CancellationToken.None);

        _mapperMock.Verify(m => m.Map<BankInformationResponse>(entity), Times.Once);
    }
}


