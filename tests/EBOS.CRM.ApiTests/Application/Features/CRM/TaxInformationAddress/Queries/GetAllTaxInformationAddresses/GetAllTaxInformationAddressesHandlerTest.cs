using EBOS.CRM.Contracts.Responses.CRM;
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

        _repositoryMock.Setup(r => r.GetAllPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);
        _repositoryMock.Setup(r => r.CountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities.Count);
        _mapperMock.Setup(m => m.Map<IReadOnlyCollection<TaxInformationAddressResponse>>(entities))
            .Returns(new List<TaxInformationAddressResponse>());

        var result = await handler.Handle(new GetAllTaxInformationAddressesQuery(), CancellationToken.None);

        Assert.NotNull(result);
    }
}

