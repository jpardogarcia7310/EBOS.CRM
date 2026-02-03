using EBOS.CRM.Application.Contracts.Responses;
using EBOS.CRM.Application.Features.IdentificationType.Query.GetAllIdentificationType;
using IdentificationTypeEntity = EBOS.CRM.Domain.Entities.IdentificationType;
using EBOS.CRM.Domain.Interfaces.Repositories;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.IdentificationType.Query.GetAllIdentificationType;

public class GetAllIdentificationTypeQueryHandlerTest
{
    private readonly Mock<IIdentificationTypeRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetAllIdentificationTypeQueryHandler _handler;

    public GetAllIdentificationTypeQueryHandlerTest()
    {
        _repositoryMock = new Mock<IIdentificationTypeRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetAllIdentificationTypeQueryHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_EntitiesExist_ReturnsMappedDtos()
    {
        var entities = new List<IdentificationTypeEntity>
        {
            new() { Id = 1, Code = "DNI", Description = "Documento" }
        };
        var dtos = new List<IdentificationTypeResponse>
        {
            new(1, "DNI", "Documento")
        };

        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);
        _mapperMock.Setup(m => m.Map<IReadOnlyCollection<IdentificationTypeResponse>>(entities)).Returns(dtos);

        var query = new global::EBOS.CRM.Application.Features.IdentificationType.Query.GetAllIdentificationType.GetAllIdentificationTypeQuery();

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("DNI", result.First().Code);
        _repositoryMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mapperMock.Verify(m => m.Map<IReadOnlyCollection<IdentificationTypeResponse>>(entities), Times.Once);
    }

    [Fact]
    public async Task Handle_NoEntities_ReturnsEmptyEnumerable()
    {
        var entities = new List<IdentificationTypeEntity>();
        var dtos = new List<IdentificationTypeResponse>();

        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);
        _mapperMock.Setup(m => m.Map<IReadOnlyCollection<IdentificationTypeResponse>>(entities)).Returns(dtos);

        var query = new global::EBOS.CRM.Application.Features.IdentificationType.Query.GetAllIdentificationType.GetAllIdentificationTypeQuery();

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_RepositoryThrows_PropagatesException()
    {
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));
        var query = new global::EBOS.CRM.Application.Features.IdentificationType.Query.GetAllIdentificationType.GetAllIdentificationTypeQuery();

        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_CancellationRequested_ThrowsOperationCanceled()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        var query = new global::EBOS.CRM.Application.Features.IdentificationType.Query.GetAllIdentificationType.GetAllIdentificationTypeQuery();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _handler.Handle(query, cts.Token));
    }

    [Fact]
    public async Task Handle_MapperConfigurationInvalid_ThrowsMappingException()
    {
        var entities = new List<IdentificationTypeEntity> { new() { Id = 1, Code = "DNI" } };
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);
        _mapperMock.Setup(m => m.Map<IReadOnlyCollection<IdentificationTypeResponse>>(entities))
            .Throws(new InvalidOperationException("Mapping failed"));

        var query = new global::EBOS.CRM.Application.Features.IdentificationType.Query.GetAllIdentificationType.GetAllIdentificationTypeQuery();

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_NullEntityProperty_MapsGracefully()
    {
        var entities = new List<IdentificationTypeEntity> { new() { Id = 1, Code = null! } };
        var dtos = new List<IdentificationTypeResponse> { new(1, null!, "Documento") };

        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);
        _mapperMock.Setup(m => m.Map<IReadOnlyCollection<IdentificationTypeResponse>>(entities)).Returns(dtos);

        var query = new global::EBOS.CRM.Application.Features.IdentificationType.Query.GetAllIdentificationType.GetAllIdentificationTypeQuery();

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Null(result.First().Code);
    }

    [Fact]
    public async Task Handle_MapperCalledWithCorrectSourceType()
    {
        var entities = new List<IdentificationTypeEntity>();
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        var query = new global::EBOS.CRM.Application.Features.IdentificationType.Query.GetAllIdentificationType.GetAllIdentificationTypeQuery();

        await _handler.Handle(query, CancellationToken.None);

        _mapperMock.Verify(m => m.Map<IReadOnlyCollection<IdentificationTypeResponse>>(entities), Times.Once);
    }

    [Fact]
    public async Task Handle_RepositoryCalledOnce_WithCancellationToken()
    {
        var query = new global::EBOS.CRM.Application.Features.IdentificationType.Query.GetAllIdentificationType.GetAllIdentificationTypeQuery();
        var entities = new List<IdentificationTypeEntity>();
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        await _handler.Handle(query, CancellationToken.None);

        _repositoryMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}










