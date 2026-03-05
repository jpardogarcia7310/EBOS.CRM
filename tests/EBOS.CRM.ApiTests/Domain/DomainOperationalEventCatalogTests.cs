using EBOS.CRM.Domain.Events;

namespace EBOS.CRM.ApiTests.Domain;

public class DomainOperationalEventCatalogTests
{
    [Fact]
    public void Concept_IsClassificationModel_NotDomainEntities()
    {
        Assert.False(DomainOperationalEventCatalog.RepresentsDomainEntities);
        Assert.Contains("classifies domain events", DomainOperationalEventCatalog.Concept, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("CustomerPrivacyRequestRegistered", DomainOperationalEventCategory.Business)]
    [InlineData("DomainCommandDeduplicated", DomainOperationalEventCategory.Technical)]
    [InlineData("DomainInvariantBreachDetected", DomainOperationalEventCategory.Anomaly)]
    public void Classify_ReturnsDeterministicCategory(string eventName, DomainOperationalEventCategory expectedCategory)
    {
        var category = DomainOperationalEventCatalog.Classify(eventName);

        Assert.Equal(expectedCategory, category);
    }

    [Fact]
    public void TryClassify_ReturnsFalse_ForUnknownEvent()
    {
        var found = DomainOperationalEventCatalog.TryClassify("UnknownDomainEvent", out var category);

        Assert.False(found);
        Assert.Equal(default, category);
    }

    [Fact]
    public void All_ContainsAllEnterpriseOperationalCategories()
    {
        var categories = DomainOperationalEventCatalog.All
            .Select(x => x.Category)
            .Distinct()
            .ToHashSet();

        Assert.Contains(DomainOperationalEventCategory.Business, categories);
        Assert.Contains(DomainOperationalEventCategory.Technical, categories);
        Assert.Contains(DomainOperationalEventCategory.Anomaly, categories);
    }
}
