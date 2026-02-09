using EBOS.CRM.Application.Contracts.Responses;
using EBOS.CRM.Application.Features.EBOS.IdentificationType.Query.GetIdentificationTypeByIdQuery;
using IdentificationTypeEntity = EBOS.CRM.Domain.Entities.IdentificationType;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.IdentificationType.Query.GetIdentificationTypeByIdQuery;

public class GetIdentificationTypeByIdQueryHandlerTest
{
    private readonly Mock<IIdentificationTypeRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetIdentificationTypeByIdQueryHandler _handler;

    public GetIdentificationTypeByIdQueryHandlerTest()
    {
        _repositoryMock = new Mock<IIdentificationTypeRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetIdentificationTypeByIdQueryHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingId_ReturnsMappedDto()
    {
        var entity = new IdentificationTypeEntity { Id = 1, Code = "DNI", Description = "Documento" };
        var dto = new IdentificationTypeResponse(1, "DNI", "Documento");

        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);
        _mapperMock.Setup(m => m.Map<IdentificationTypeResponse>(entity)).Returns(dto);

        var query = new global::EBOS.CRM.Application.Features.EBOS.IdentificationType.Query.GetIdentificationTypeByIdQuery.GetIdentificationTypeByIdQuery(1);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(dto.Id, result.Id);
        Assert.Equal(dto.Code, result.Code);
        _repositoryMock.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        _mapperMock.Verify(m => m.Map<IdentificationTypeResponse>(entity), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingId_ReturnsNull()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((IdentificationTypeEntity?)null);
        var query = new global::EBOS.CRM.Application.Features.EBOS.IdentificationType.Query.GetIdentificationTypeByIdQuery.GetIdentificationTypeByIdQuery(99);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Null(result);
        _mapperMock.Verify(m => m.Map<IdentificationTypeResponse>(It.IsAny<IdentificationTypeEntity>()), Times.Never);
    }

    [Fact]
    public async Task Handle_RepositoryThrows_PropagatesException()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));
        var query = new global::EBOS.CRM.Application.Features.EBOS.IdentificationType.Query.GetIdentificationTypeByIdQuery.GetIdentificationTypeByIdQuery(1);

        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_CancellationRequested_ThrowsOperationCanceled()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        var query = new global::EBOS.CRM.Application.Features.EBOS.IdentificationType.Query.GetIdentificationTypeByIdQuery.GetIdentificationTypeByIdQuery(1);

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _handler.Handle(query, cts.Token));
    }

    [Fact]
    public async Task Handle_MapperConfigurationInvalid_ThrowsMappingException()
    {
        var entity = new IdentificationTypeEntity { Id = 1, Code = "DNI" };
        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);
        _mapperMock.Setup(m => m.Map<IdentificationTypeResponse>(entity))
            .Throws(new InvalidOperationException("Mapping failed"));

        var query = new global::EBOS.CRM.Application.Features.EBOS.IdentificationType.Query.GetIdentificationTypeByIdQuery.GetIdentificationTypeByIdQuery(1);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_RepositoryCalledOnce_WithCorrectIdAndToken()
    {
        var query = new global::EBOS.CRM.Application.Features.EBOS.IdentificationType.Query.GetIdentificationTypeByIdQuery.GetIdentificationTypeByIdQuery(42);
        _repositoryMock.Setup(r => r.GetByIdAsync(42, It.IsAny<CancellationToken>()))
            .ReturnsAsync((IdentificationTypeEntity?)null);

        await _handler.Handle(query, CancellationToken.None);

        _repositoryMock.Verify(r => r.GetByIdAsync(42, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NullEntity_DoesNotCallMapper()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((IdentificationTypeEntity?)null);
        var query = new global::EBOS.CRM.Application.Features.EBOS.IdentificationType.Query.GetIdentificationTypeByIdQuery.GetIdentificationTypeByIdQuery(1);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Null(result);
        _mapperMock.Verify(m => m.Map<IdentificationTypeResponse>(It.IsAny<IdentificationTypeEntity>()), Times.Never);
    }
}


