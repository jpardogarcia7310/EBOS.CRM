using EBOS.CRM.Application.Contracts.Responses;
using EBOS.CRM.Application.Features.Countries.Queries.GetAllCountries;
using EBOS.CRM.Domain.Entities;
using EBOS.CRM.Domain.Interfaces.Repositories;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.Countries.Queries.GetAllCountries;

public class GetAllCountriesQueryHandlerTest
{
    private readonly Mock<ICountryRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetAllCountriesQueryHandler _handler;

    public GetAllCountriesQueryHandlerTest()
    {
        _repositoryMock = new Mock<ICountryRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetAllCountriesQueryHandler(_repositoryMock.Object, _mapperMock.Object);

        _repositoryMock.Setup(r => r.GetAllPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Country>());
        _repositoryMock.Setup(r => r.CountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
    }

    [Fact]
    public async Task Handle_CountriesExist_ReturnsMappedDtos()
    {
        // Arrange
        var countries = new List<Country>
        {
            new() { Id = 1, Name = "Espa?a", Iso31661A2Code = "ES", Iso31661A3Code = "ESP", Iso31661NumCode = "724",
                Domain = ".es", Currency = "Euro", CurrencyCode = "EUR", InternationalPhoneCode = "34" }
        };
        var dtos = new List<CountryResponse>
        {
            new(1, "Espa?a", "ES", "ESP", "724", ".es", "Euro", "EUR", "34")
        };

        _repositoryMock.Setup(r => r.GetAllPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(countries);
        _repositoryMock.Setup(r => r.CountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(countries.Count);
        _mapperMock.Setup(m => m.Map<IReadOnlyCollection<CountryResponse>>(countries)).Returns(dtos);

        var query = new GetAllCountriesQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal("Espa?a", result.Items.First().Name);
        _repositoryMock.Verify(r => r.GetAllPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _mapperMock.Verify(m => m.Map<IReadOnlyCollection<CountryResponse>>(countries), Times.Once);
    }

    [Fact]
    public async Task Handle_NoCountries_ReturnsEmptyEnumerable()
    {
        // Arrange
        var countries = new List<Country>();
        var dtos = new List<CountryResponse>();

        _repositoryMock.Setup(r => r.GetAllPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(countries);
        _repositoryMock.Setup(r => r.CountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        _mapperMock.Setup(m => m.Map<IReadOnlyCollection<CountryResponse>>(countries)).Returns(dtos);

        var query = new GetAllCountriesQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Items);
    }

    [Fact]
    public async Task Handle_RepositoryThrows_PropagatesException()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetAllPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Fail"));
        var query = new GetAllCountriesQuery();

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_CancellationRequested_ThrowsOperationCanceled()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        var query = new GetAllCountriesQuery();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _handler.Handle(query, cts.Token));
    }

    [Fact]
    public async Task Handle_MapperConfigurationInvalid_ThrowsMappingException()
    {
        // Arrange
        var countries = new List<Country> { new() { Id = 1, Name = "Espa?a" } };

        _repositoryMock.Setup(r => r.GetAllPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(countries);
        _repositoryMock.Setup(r => r.CountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(countries.Count);
        _mapperMock.Setup(m => m.Map<IReadOnlyCollection<CountryResponse>>(countries))
                   .Throws(new InvalidOperationException("Mapping failed"));

        var query = new GetAllCountriesQuery();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_NullEntityProperty_MapsGracefully()
    {
        // Arrange
        var countries = new List<Country> { new() { Id = 1, Name = null! } };
        var dtos = new List<CountryResponse> { new(1, null!, "ES", "ESP", "724", ".es", "Euro", "EUR", "34") };

        _repositoryMock.Setup(r => r.GetAllPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(countries);
        _repositoryMock.Setup(r => r.CountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(countries.Count);
        _mapperMock.Setup(m => m.Map<IReadOnlyCollection<CountryResponse>>(countries)).Returns(dtos);

        var query = new GetAllCountriesQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Null(result.Items.First().Name);
    }

    [Fact]
    public async Task Handle_MapperCalledWithCorrectSourceType()
    {
        // Arrange
        var countries = new List<Country>();

        _repositoryMock.Setup(r => r.GetAllPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(countries);
        _repositoryMock.Setup(r => r.CountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var query = new GetAllCountriesQuery();

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _mapperMock.Verify(m => m.Map<IReadOnlyCollection<CountryResponse>>(countries), Times.Once);
    }

    [Fact]
    public async Task Handle_RepositoryCalledOnce_WithCancellationToken()
    {
        // Arrange
        var query = new GetAllCountriesQuery();

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.GetAllPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
