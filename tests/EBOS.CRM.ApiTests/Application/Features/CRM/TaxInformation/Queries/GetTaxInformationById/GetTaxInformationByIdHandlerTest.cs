using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Application.Features.CRM.TaxInformation.Queries.GetTaxInformationById;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.TaxInformation.Queries.GetTaxInformationById;

public class GetTaxInformationByIdQueryHandlerTest
{
    private readonly Mock<ITaxInformationRepository> _repositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    [Fact]
    public async Task Handle_WhenFound_Maps()
    {
        var handler = new GetTaxInformationByIdQueryHandler(_repositoryMock.Object, _mapperMock.Object);
        var entity = new EBOS.CRM.Domain.Entities.CRM.TaxInformation();

        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);
        _mapperMock.Setup(m => m.Map<TaxInformationResponse>(entity))
            .Returns((TaxInformationResponse)null!);

        await handler.Handle(new GetTaxInformationByIdQuery(1), CancellationToken.None);

        _mapperMock.Verify(m => m.Map<TaxInformationResponse>(entity), Times.Once);
    }
}


