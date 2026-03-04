using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.ApiTests.Domain.Entities.CRM;

public class IndividualCustomerTests
{
    [Fact]
    public void IndividualCustomer_InheritsCustomerCoreFields()
    {
        var entity = new IndividualCustomer
        {
            TenantId = 1,
            Code = "C-1",
            Email = "i@example.com",
            Phone = "3000000000",
            StatusId = 1,
            FirstName = "Jane",
            LastName = "Doe",
            BirthDate = new DateTime(1990, 1, 1),
            IdentificationNumber = "12345678",
            IdentificationTypeId = 10
        };

        Assert.Equal("C-1", entity.Code);
        Assert.Equal("Jane", entity.FirstName);
        Assert.Equal(10, entity.IdentificationTypeId);
    }
}
