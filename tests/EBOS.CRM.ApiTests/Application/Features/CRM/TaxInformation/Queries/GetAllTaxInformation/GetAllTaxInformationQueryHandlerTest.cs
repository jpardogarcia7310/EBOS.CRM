using EBOS.CRM.Application.Features.CRM.TaxInformation.Queries.GetAllTaxInformation;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.TaxInformation.Queries.GetAllTaxInformation;

public class GetAllTaxInformationQueryHandlerTest
{
    private readonly Mock<ITaxInformationRepository> _repositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    [Fact]
    public async Task Handle_ReturnsPagedResult()
    {
        var handler = new GetAllTaxInformationQueryHandler(_repositoryMock.Object, _mapperMock.Object);
        var entities = new List<global::EBOS.CRM.Domain.Entities.CRM.TaxInformation> { new() };

        _repositoryMock.Setup(r => r.GetAllPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);
        _repositoryMock.Setup(r => r.CountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities.Count);
        _mapperMock.Setup(m => m.Map<IReadOnlyCollection<TaxInformationResponse>>(entities))
            .Returns(new List<TaxInformationResponse>());

        var result = await handler.Handle(new GetAllTaxInformationQuery(), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(entities.Count, result.Total);
    }
}
