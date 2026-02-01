using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Application.Features.CRM.BankInformation.Queries.GetAllBankInformations;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using Moq;
using EBOS.CRM.Domain.Primitives.Paging;
using EBOS.CRM.Application.Contracts.Requests.Common;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.BankInformation.Queries.GetAllBankInformations;

public class GetAllBankInformationsQueryHandlerTest
{
    private readonly Mock<IBankInformationRepository> _repositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    [Fact]
    public async Task Handle_ReturnsList()
    {
        var handler = new GetAllBankInformationsQueryHandler(_repositoryMock.Object, _mapperMock.Object);
        var entities = new List<EBOS.CRM.Domain.Entities.CRM.BankInformation> { new() };
        var dtos = new List<BankInformationResponse>();

        _repositoryMock.Setup(r => r.GetPagedAsync(It.IsAny<PagedQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<EBOS.CRM.Domain.Entities.CRM.BankInformation>(entities, 1, 50, entities.Count, entities.Count == 0 ? 0 : 1, null, null, null));
        _mapperMock.Setup(m => m.Map<IReadOnlyCollection<BankInformationResponse>>(entities))
            .Returns(dtos);

        var result = await handler.Handle(new GetAllBankInformationsQuery(new PagedQueryRequest()), CancellationToken.None);

        Assert.NotNull(result);
    }
}




