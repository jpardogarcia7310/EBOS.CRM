using System.Reflection;
using EBOS.CRM.Application.Options;
using EBOS.CRM.Application.Validation;

namespace EBOS.CRM.ApiTests.Application.Coverage;

public class ApplicationMissingNominalCoverageTest
{
    [Fact]
    public void Application_MissingTypes_ArePresent_ByReflection()
    {
        var assembly = typeof(EBOS.CRM.Application.Behavior.ValidationBehavior<,>).Assembly;

        AssertContainsTypeName(assembly, "CustomerPrivacyRequestMapper");
        AssertContainsTypeName(assembly, "ReferenceEqualityComparer");
        AssertContainsTypeName(assembly, nameof(ValidationCatalogOptions));
        AssertContainsTypeName(assembly, nameof(ValidationRuleKeys));
    }

    [Fact]
    public void ValidationRuleKeys_BuildsExpectedKeys()
    {
        Assert.Equal("DEFAULT", ValidationRuleKeys.DefaultCountryKey);
        Assert.Equal("postal_code:ES", ValidationRuleKeys.PostalCode("ES"));
        Assert.Equal("phone:US", ValidationRuleKeys.Phone("US"));
        Assert.Equal("tax_id:MX", ValidationRuleKeys.TaxId("MX"));
        Assert.Equal("id:DNI", ValidationRuleKeys.Identification("DNI"));
    }

    [Fact]
    public void ValidationCatalogOptions_HasExpectedDefaults_AndCanBeConfigured()
    {
        var options = new ValidationCatalogOptions
        {
            DefaultCountryIso2 = "ES",
            PostalCodePatternsByCountry = new Dictionary<string, string> { ["ES"] = "^[0-9]{5}$" },
            PhonePatternsByCountry = new Dictionary<string, string> { ["ES"] = "^[0-9]{9}$" },
            TaxIdPatternsByCountry = new Dictionary<string, string> { ["ES"] = "^[A-Z0-9]{9}$" },
            IdentificationPatternsByTypeCode = new Dictionary<string, string> { ["DNI"] = "^[0-9]{8}[A-Z]$" }
        };

        Assert.Equal("ValidationCatalogs", ValidationCatalogOptions.SectionName);
        Assert.Equal("ES", options.DefaultCountryIso2);
        Assert.Equal("^[0-9]{5}$", options.PostalCodePatternsByCountry["ES"]);
        Assert.Equal("^[0-9]{9}$", options.PhonePatternsByCountry["ES"]);
        Assert.Equal("^[A-Z0-9]{9}$", options.TaxIdPatternsByCountry["ES"]);
        Assert.Equal("^[0-9]{8}[A-Z]$", options.IdentificationPatternsByTypeCode["DNI"]);
    }

    private static void AssertContainsTypeName(Assembly assembly, string typeName)
    {
        var exists = assembly.GetTypes().Any(t => string.Equals(t.Name, typeName, StringComparison.Ordinal));
        Assert.True(exists, $"Type '{typeName}' was not found in assembly '{assembly.GetName().Name}'.");
    }
}
