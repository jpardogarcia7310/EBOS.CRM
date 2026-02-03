using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Application.Features.CRM.TaxInformation.Queries.GetAllTaxInformations;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.TaxInformation.Queries.GetAllTaxInformations;

public class GetAllTaxInformationsQueryHandlerTest
{
    private readonly Mock<ITaxInformationRepository> _repositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    [Fact]
    public async Task Handle_ReturnsList()
    {
        var handler = new GetAllTaxInformationsQueryHandler(_repositoryMock.Object, _mapperMock.Object);
        var entities = new List<EBOS.CRM.Domain.Entities.CRM.TaxInformation> { new() };
        var dtos = new List<TaxInformationResponse>();

        _repositoryMock.Setup(r => r.GetAllPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);
        _repositoryMock.Setup(r => r.CountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities.Count);
        _mapperMock.Setup(m => m.Map<IReadOnlyCollection<TaxInformationResponse>>(entities))
            .Returns(dtos);

        var result = await handler.Handle(new GetAllTaxInformationsQuery(), CancellationToken.None);

        Assert.NotNull(result);
    }
}
