using EBOS.CRM.ApiTests.Fixtures.EntitiesFactories.CRM;
using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.ApiTests.Fixtures.EntitiesFactories.CRM;

public class IndividualCustomerEntityFactoryTest
{
    public static IndividualCustomer CreateValidIndividualCustomer(
        string code = "IND-001",
        string email = "ind@example.com",
        string phone = "123",
        DateTime? createdAt = null,
        long statusId = 1,
        string firstName = "John",
        string lastName = "Doe",
        DateTime? birthDate = null,
        string? identificationNumber = "1234567890",
        long identificationTypeId = 1)
    {
        return new IndividualCustomer
        {
            Code = code,
            Email = email,
            Phone = phone,
            CreatedAt = createdAt ?? DateTime.UtcNow,
            StatusId = statusId,
            FirstName = firstName,
            LastName = lastName,
            BirthDate = birthDate ?? new DateTime(1990, 1, 1),
            IdentificationNumber = identificationNumber,
            IdentificationTypeId = identificationTypeId
        };
    }

    [Fact]
    public void CreateValidIndividualCustomer_Defaults_AreSet()
    {
        var entity = CreateValidIndividualCustomer();

        Assert.NotNull(entity);
        Assert.Equal("IND-001", entity.Code);
        Assert.Equal("ind@example.com", entity.Email);
        Assert.Equal("123", entity.Phone);
        Assert.Equal(1, entity.StatusId);
        Assert.Equal("John", entity.FirstName);
        Assert.Equal("Doe", entity.LastName);
        Assert.Equal("1234567890", entity.IdentificationNumber);
        Assert.Equal(1, entity.IdentificationTypeId);
    }

    [Fact]
    public void CreateValidIndividualCustomer_CustomValues_AreApplied()
    {
        var date = new DateTime(2000, 1, 1);
        var entity = CreateValidIndividualCustomer(
            code: "IND-999",
            email: "x@y.com",
            phone: "999",
            createdAt: date,
            statusId: 2,
            firstName: "Jane",
            lastName: "Smith",
            birthDate: date,
            identificationNumber: "ABC",
            identificationTypeId: 3);

        Assert.Equal("IND-999", entity.Code);
        Assert.Equal("x@y.com", entity.Email);
        Assert.Equal("999", entity.Phone);
        Assert.Equal(date, entity.CreatedAt);
        Assert.Equal(2, entity.StatusId);
        Assert.Equal("Jane", entity.FirstName);
        Assert.Equal("Smith", entity.LastName);
        Assert.Equal(date, entity.BirthDate);
        Assert.Equal("ABC", entity.IdentificationNumber);
        Assert.Equal(3, entity.IdentificationTypeId);
    }
}


