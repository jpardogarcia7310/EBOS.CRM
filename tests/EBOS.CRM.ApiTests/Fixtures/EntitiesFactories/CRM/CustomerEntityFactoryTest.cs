using EBOS.CRM.ApiTests.Fixtures.EntitiesFactories.CRM;
using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.ApiTests.Fixtures.EntitiesFactories.CRM;

public class CustomerEntityFactoryTest
{
    public static Customer CreateValidCustomer(
        string code = "C001",
        string email = "a@b.com",
        string phone = "123",
        DateTime? createdAt = null,
        long statusId = 1)
    {
        return new Customer
        {
            Code = code,
            Email = email,
            Phone = phone,
            CreatedAt = createdAt ?? DateTime.UtcNow,
            StatusId = statusId
        };
    }

    [Fact]
    public void CreateValidCustomer_Defaults_AreSet()
    {
        var entity = CreateValidCustomer();

        Assert.NotNull(entity);
        Assert.Equal("C001", entity.Code);
        Assert.Equal("a@b.com", entity.Email);
        Assert.Equal("123", entity.Phone);
        Assert.Equal(1, entity.StatusId);
    }

    [Fact]
    public void CreateValidCustomer_CustomValues_AreApplied()
    {
        var date = new DateTime(2024, 1, 1);
        var entity = CreateValidCustomer(
            code: "C999",
            email: "x@y.com",
            phone: "999",
            createdAt: date,
            statusId: 2);

        Assert.Equal("C999", entity.Code);
        Assert.Equal("x@y.com", entity.Email);
        Assert.Equal("999", entity.Phone);
        Assert.Equal(date, entity.CreatedAt);
        Assert.Equal(2, entity.StatusId);
    }
}
