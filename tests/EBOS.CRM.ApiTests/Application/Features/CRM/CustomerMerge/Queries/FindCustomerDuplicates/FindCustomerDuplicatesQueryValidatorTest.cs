using EBOS.CRM.Application.Features.CRM.CustomerMerge.Queries.FindCustomerDuplicates;
using EBOS.CRM.Contracts.Requests.CRM.CustomerMerge;
using EBOS.CRM.Domain.Interfaces.Services.CRM;
using FluentAssertions;
using FluentValidation.TestHelper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CustomerMerge.Queries.FindCustomerDuplicates;

public class FindCustomerDuplicatesQueryValidatorTest
{
    private readonly FindCustomerDuplicatesQueryValidator _validator;

    public FindCustomerDuplicatesQueryValidatorTest()
    {
        var normalizationService = new Mock<ICustomerDedupeNormalizationService>();
        normalizationService
            .Setup(x => x.NormalizePhone(It.IsAny<string?>()))
            .Returns((string? value) =>
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    return null;
                }

                return new string(value.Where(char.IsDigit).ToArray());
            });

        _validator = new FindCustomerDuplicatesQueryValidator(normalizationService.Object);
    }

    [Fact]
    public void Validate_ValidRequest_Passes()
    {
        var query = new FindCustomerDuplicatesQuery(BuildRequest() with { Phone = "123456" });

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_NullRequest_Fails()
    {
        var query = new FindCustomerDuplicatesQuery(null!);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.Request);
    }

    [Fact]
    public void Validate_MissingAllMatchingFields_Fails()
    {
        var query = new FindCustomerDuplicatesQuery(BuildRequest() with
        {
            Email = null,
            Phone = null,
            TaxId = null,
            IdentificationNumber = null
        });

        var result = _validator.TestValidate(query);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(x => x.ErrorMessage == "At least one matching field is required.");
    }

    [Fact]
    public void Validate_EmailWhitespaceOnly_Fails()
    {
        var query = new FindCustomerDuplicatesQuery(new FindCustomerDuplicatesRequest(
            TenantId: 1,
            Email: "   ",
            Phone: null,
            TaxId: null,
            IdentificationNumber: null));

        var result = _validator.TestValidate(query);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(x => x.ErrorMessage == "At least one matching field is required.");
    }

    [Fact]
    public void Validate_InvalidEmail_Fails()
    {
        var query = new FindCustomerDuplicatesQuery(BuildRequest() with { Email = "invalid" });

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.Request.Email);
    }

    [Fact]
    public void Validate_PhoneNotNormalized_Fails()
    {
        var query = new FindCustomerDuplicatesQuery(BuildRequest() with { Phone = "123-456" });

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.Request.Phone);
    }

    [Fact]
    public void Validate_PhoneMinimumDigits_Passes()
    {
        var query = new FindCustomerDuplicatesQuery(BuildRequest() with { Phone = "1" });

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_PhoneTooLong_Fails()
    {
        var query = new FindCustomerDuplicatesQuery(BuildRequest() with { Phone = "1234567890123" });

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.Request.Phone);
    }

    [Fact]
    public void Validate_PhoneAtMaxLength_Passes()
    {
        var query = new FindCustomerDuplicatesQuery(BuildRequest() with { Phone = "123456789012" });

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_TaxIdTooLong_Fails()
    {
        var query = new FindCustomerDuplicatesQuery(BuildRequest() with { TaxId = Long(21) });

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.Request.TaxId);
    }

    [Fact]
    public void Validate_TaxIdAtMaxLength_Passes()
    {
        var query = new FindCustomerDuplicatesQuery(BuildRequest() with { TaxId = Long(20) });

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_TaxIdNonAlphanumeric_Fails()
    {
        var query = new FindCustomerDuplicatesQuery(BuildRequest() with { TaxId = "ABC-123" });

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.Request.TaxId);
    }

    [Fact]
    public void Validate_IdentificationNumberAtMaxLength_Passes()
    {
        var query = new FindCustomerDuplicatesQuery(BuildRequest() with { IdentificationNumber = Long(10) });

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_IdentificationNumberTooLong_Fails()
    {
        var query = new FindCustomerDuplicatesQuery(BuildRequest() with { IdentificationNumber = Long(11) });

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.Request.IdentificationNumber);
    }

    [Fact]
    public void Validate_IdentificationNumberNonAlphanumeric_Fails()
    {
        var query = new FindCustomerDuplicatesQuery(BuildRequest() with { IdentificationNumber = "12-3" });

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.Request.IdentificationNumber);
    }

    [Fact]
    public void Validate_IdentificationNumberMinimumLength_Passes()
    {
        var query = new FindCustomerDuplicatesQuery(BuildRequest() with { IdentificationNumber = "A" });

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_OnlyEmailProvided_Passes()
    {
        var query = new FindCustomerDuplicatesQuery(new FindCustomerDuplicatesRequest(
            TenantId: 1,
            Email: "valid@example.com",
            Phone: null,
            TaxId: null,
            IdentificationNumber: null));

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_OnlyPhoneProvided_Passes()
    {
        var query = new FindCustomerDuplicatesQuery(new FindCustomerDuplicatesRequest(
            TenantId: 1,
            Email: null,
            Phone: "123456",
            TaxId: null,
            IdentificationNumber: null));

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_OnlyTaxIdProvided_Passes()
    {
        var query = new FindCustomerDuplicatesQuery(new FindCustomerDuplicatesRequest(
            TenantId: 1,
            Email: null,
            Phone: null,
            TaxId: "TAX123",
            IdentificationNumber: null));

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_OnlyIdentificationNumberProvided_Passes()
    {
        var query = new FindCustomerDuplicatesQuery(new FindCustomerDuplicatesRequest(
            TenantId: 1,
            Email: null,
            Phone: null,
            TaxId: null,
            IdentificationNumber: "ID123"));

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_MixedFieldsWithOneInvalid_Fails()
    {
        var query = new FindCustomerDuplicatesQuery(new FindCustomerDuplicatesRequest(
            TenantId: 1,
            Email: "valid@example.com",
            Phone: "123-456",
            TaxId: null,
            IdentificationNumber: null));

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.Request.Phone);
    }

    [Fact]
    public void Validate_MultipleValidFields_Passes()
    {
        var query = new FindCustomerDuplicatesQuery(new FindCustomerDuplicatesRequest(
            TenantId: 1,
            Email: "valid@example.com",
            Phone: "123456",
            TaxId: "TAX123",
            IdentificationNumber: "ID123"));

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_MixedMaxLimits_Passes()
    {
        var query = new FindCustomerDuplicatesQuery(new FindCustomerDuplicatesRequest(
            TenantId: 1,
            Email: BuildEmailWithLength(100),
            Phone: "123456789012",
            TaxId: Long(20),
            IdentificationNumber: Long(10)));

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_MixedMaxEmailWithInvalidPhone_Fails()
    {
        var query = new FindCustomerDuplicatesQuery(new FindCustomerDuplicatesRequest(
            TenantId: 1,
            Email: BuildEmailWithLength(100),
            Phone: "123-456",
            TaxId: Long(20),
            IdentificationNumber: Long(10)));

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.Request.Phone);
    }

    [Fact]
    public void Validate_MixedMaxPhoneWithInvalidTaxId_Fails()
    {
        var query = new FindCustomerDuplicatesQuery(new FindCustomerDuplicatesRequest(
            TenantId: 1,
            Email: BuildEmailWithLength(100),
            Phone: "123456789012",
            TaxId: "ABC-123",
            IdentificationNumber: Long(10)));

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.Request.TaxId);
    }

    [Fact]
    public void Validate_MixedMaxTaxIdWithInvalidIdNumber_Fails()
    {
        var query = new FindCustomerDuplicatesQuery(new FindCustomerDuplicatesRequest(
            TenantId: 1,
            Email: BuildEmailWithLength(100),
            Phone: "123456789012",
            TaxId: Long(20),
            IdentificationNumber: "12-3"));

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.Request.IdentificationNumber);
    }

    [Fact]
    public void Validate_EmailAtMaxLength_Passes()
    {
        var email = BuildEmailWithLength(100);
        var query = new FindCustomerDuplicatesQuery(BuildRequest() with { Email = email });

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmailTooLong_Fails()
    {
        var email = BuildEmailWithLength(101);
        var query = new FindCustomerDuplicatesQuery(BuildRequest() with { Email = email });

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.Request.Email);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_InvalidTenantId_Fails(long tenantId)
    {
        var query = new FindCustomerDuplicatesQuery(BuildRequest() with { TenantId = tenantId });

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.Request.TenantId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_InvalidPageNumber_Fails(int pageNumber)
    {
        var query = new FindCustomerDuplicatesQuery(BuildRequest(), pageNumber);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.PageNumber);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_InvalidPageSize_Fails(int pageSize)
    {
        var query = new FindCustomerDuplicatesQuery(BuildRequest(), 1, pageSize);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }

    private static FindCustomerDuplicatesRequest BuildRequest() =>
        new(
            TenantId: 1,
            Email: "valid@example.com",
            Phone: "123456",
            TaxId: "TAX123",
            IdentificationNumber: "ID123"
        );

    private static string Long(int length) => new('X', length);

    private static string BuildEmailWithLength(int totalLength)
    {
        const string prefix = "a@";
        const string suffix = ".com";
        var middleLength = totalLength - prefix.Length - suffix.Length;
        middleLength.Should().BeGreaterThan(0, "email length should allow a valid local+domain");
        return prefix + new string('b', middleLength) + suffix;
    }
}
