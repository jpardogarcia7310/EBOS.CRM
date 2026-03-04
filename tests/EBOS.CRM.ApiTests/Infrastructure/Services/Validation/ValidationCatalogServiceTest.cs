using EBOS.CRM.Domain.Entities.EBOS;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using EBOS.CRM.Infrastructure.Services.Validation;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace EBOS.CRM.ApiTests.Infrastructure.Services.Validation;

public class ValidationCatalogServiceTest
{
    [Fact]
    public async Task GetPatternAsync_WhenKeyExists_ReturnsPattern_AndCaches()
    {
        var repo = new Mock<IValidationRuleRepository>(MockBehavior.Strict);
        repo.Setup(x => x.GetByKeysAsync(It.IsAny<IReadOnlyCollection<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ValidationRule> { new() { Key = "postal_code:ES", Pattern = "^[0-9]{5}$", IsActive = true } });

        using var cache = new MemoryCache(new MemoryCacheOptions());
        var sut = new ValidationCatalogService(repo.Object, cache);

        var first = await sut.GetPatternAsync("postal_code:ES");
        var second = await sut.GetPatternAsync("postal_code:ES");

        Assert.Equal("^[0-9]{5}$", first);
        Assert.Equal("^[0-9]{5}$", second);
        repo.Verify(x => x.GetByKeysAsync(It.IsAny<IReadOnlyCollection<string>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetPatternAsync_WhenKeyIsEmpty_ReturnsNull()
    {
        var repo = new Mock<IValidationRuleRepository>(MockBehavior.Strict);
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var sut = new ValidationCatalogService(repo.Object, cache);

        var result = await sut.GetPatternAsync(" ");

        Assert.Null(result);
    }
}
