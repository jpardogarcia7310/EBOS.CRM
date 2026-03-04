using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Application.Features.CRM.BranchOffice.Queries.GetBranchOfficeById;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.BranchOffice.Queries.GetBranchOfficeById;

public class GetBranchOfficeByIdQueryHandlerTest
{
    private readonly Mock<IBranchOfficeRepository> _repositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    [Fact]
    public async Task Handle_WhenFound_Maps()
    {
        var handler = new GetBranchOfficeByIdQueryHandler(_repositoryMock.Object, _mapperMock.Object);
        var entity = new global::EBOS.CRM.Domain.Entities.CRM.BranchOffice();

        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);
        _mapperMock.Setup(m => m.Map<BranchOfficeResponse>(entity))
            .Returns((BranchOfficeResponse)null!);

        await handler.Handle(new GetBranchOfficeByIdQuery(1), CancellationToken.None);

        _mapperMock.Verify(m => m.Map<BranchOfficeResponse>(entity), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenNotFound_ReturnsNull()
    {
        var handler = new GetBranchOfficeByIdQueryHandler(_repositoryMock.Object, _mapperMock.Object);
        _repositoryMock.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((global::EBOS.CRM.Domain.Entities.CRM.BranchOffice?)null);

        var result = await handler.Handle(new GetBranchOfficeByIdQuery(99), CancellationToken.None);

        Assert.Null(result);
        _mapperMock.Verify(m => m.Map<BranchOfficeResponse>(It.IsAny<global::EBOS.CRM.Domain.Entities.CRM.BranchOffice>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenCanceled_ThrowsOperationCanceled()
    {
        var handler = new GetBranchOfficeByIdQueryHandler(_repositoryMock.Object, _mapperMock.Object);
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            handler.Handle(new GetBranchOfficeByIdQuery(1), cts.Token));
    }
}


