using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Application.Features.CRM.TaxInformationAddress.Queries.GetTaxInformationAddressById;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.TaxInformationAddress.Queries.GetTaxInformationAddressById;

public class GetTaxInformationAddressByIdQueryHandlerTest
{
    private readonly Mock<ITaxInformationAddressRepository> _repositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    [Fact]
    public async Task Handle_WhenFound_Maps()
    {
        var handler = new GetTaxInformationAddressByIdQueryHandler(_repositoryMock.Object, _mapperMock.Object);
        var entity = new EBOS.CRM.Domain.Entities.CRM.TaxInformationAddress();

        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);
        _mapperMock.Setup(m => m.Map<TaxInformationAddressResponse>(entity))
            .Returns((TaxInformationAddressResponse)null!);

        await handler.Handle(new GetTaxInformationAddressByIdQuery(1), CancellationToken.None);

        _mapperMock.Verify(m => m.Map<TaxInformationAddressResponse>(entity), Times.Once);
    }
}