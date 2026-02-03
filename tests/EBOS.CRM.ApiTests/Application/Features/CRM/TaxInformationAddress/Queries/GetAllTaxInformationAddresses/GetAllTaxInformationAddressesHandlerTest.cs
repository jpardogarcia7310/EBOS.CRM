using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Application.Features.CRM.TaxInformationAddress.Queries.GetAllTaxInformationAddresses;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.TaxInformationAddress.Queries.GetAllTaxInformationAddresses;

public class GetAllTaxInformationAddressesQueryHandlerTest
{
    private readonly Mock<ITaxInformationAddressRepository> _repositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    [Fact]
    public async Task Handle_ReturnsList()
    {
        var handler = new GetAllTaxInformationAddressesQueryHandler(_repositoryMock.Object, _mapperMock.Object);
        var entities = new List<EBOS.CRM.Domain.Entities.CRM.TaxInformationAddress> { new() };
        var dtos = new List<TaxInformationAddressResponse>();

        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);
        _mapperMock.Setup(m => m.Map<IReadOnlyCollection<TaxInformationAddressResponse>>(entities))
            .Returns(dtos);

        var result = await handler.Handle(new GetAllTaxInformationAddressesQuery(), CancellationToken.None);

        Assert.NotNull(result);
    }
}







