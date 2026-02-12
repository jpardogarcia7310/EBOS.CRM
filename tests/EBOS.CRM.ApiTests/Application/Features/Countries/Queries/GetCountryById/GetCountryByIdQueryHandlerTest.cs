using EBOS.CRM.Contracts.Responses.EBOS;
using EBOS.CRM.Application.Features.EBOS.Countries.Queries.GetCountryById;
using EBOS.CRM.Domain.Entities.EBOS;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.Countries.Queries.GetCountryById;

public class GetCountryByIdQueryHandlerTest
{
    private readonly Mock<ICountryRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetCountryByIdQueryHandler _handler;

    public GetCountryByIdQueryHandlerTest()
    {
        _repositoryMock = new Mock<ICountryRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetCountryByIdQueryHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingId_ReturnsMappedDto()
    {
        // Arrange
        var country = new Country
        {
            Id = 1,
            Name = "España",
            Iso31661A2Code = "ES",
            Iso31661A3Code = "ESP",
            Iso31661NumCode = "724",
            Domain = ".es",
            Currency = "Euro",
            CurrencyCode = "EUR",
            InternationalPhoneCode = "34"
        };
        var dto = new CountryResponse(country.Id, country.Name, country.Iso31661A2Code, country.Iso31661A3Code,
            country.Iso31661NumCode, country.Domain, country.Currency, country.CurrencyCode,
            country.InternationalPhoneCode);

        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(country);
        _mapperMock.Setup(m => m.Map<CountryResponse>(country)).Returns(dto);

        var query = new GetCountryByIdQuery(1);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dto.Id, result.Id);
        Assert.Equal(dto.Name, result.Name);
        _repositoryMock.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()),
            Times.Once);
        _mapperMock.Verify(m => m.Map<CountryResponse>(country), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingId_ReturnsNull()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Country?)null);
        var query = new GetCountryByIdQuery(99);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
        _mapperMock.Verify(m => m.Map<CountryResponse>(It.IsAny<Country>()), Times.Never);
    }

    [Fact]
    public async Task Handle_RepositoryThrows_PropagatesException()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(new Exception("DB error"));
        var query = new GetCountryByIdQuery(1);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_CancellationRequested_ThrowsOperationCanceled()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        var query = new GetCountryByIdQuery(1);

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _handler.Handle(query, cts.Token));
    }

    [Fact]
    public async Task Handle_MapperConfigurationInvalid_ThrowsMappingException()
    {
        // Arrange
        var country = new Country { Id = 1, Name = "España" };
        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(country);

        // We simulate that the Mapster mapper fails
        _mapperMock.Setup(m => m.Map<CountryResponse>(country))
                   .Throws(new InvalidOperationException("Mapping failed"));

        var query = new GetCountryByIdQuery(1);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_RepositoryCalledOnce_WithCorrectIdAndToken()
    {
        // Arrange
        var query = new GetCountryByIdQuery(42);
        _repositoryMock.Setup(r => r.GetByIdAsync(42, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Country?)null);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.GetByIdAsync(42, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_NullEntity_DoesNotCallMapper()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Country?)null);
        var query = new GetCountryByIdQuery(1);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
        _mapperMock.Verify(m => m.Map<CountryResponse>(It.IsAny<Country>()), Times.Never);
    }
}



