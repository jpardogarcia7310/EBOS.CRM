using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.ApiTests.Domain.Entities.CRM;

public class CustomerTests
{
    [Fact]
    public void Customer_Collections_AreInitialized()
    {
        var customer = new Customer();

        Assert.NotNull(customer.Addresses);
        Assert.NotNull(customer.CustomerAddresses);
        Assert.NotNull(customer.AccountContacts);
        Assert.NotNull(customer.Preferences);
        Assert.NotNull(customer.Consents);
        Assert.Empty(customer.Addresses);
        Assert.Empty(customer.CustomerAddresses);
        Assert.Empty(customer.AccountContacts);
        Assert.Empty(customer.Preferences);
        Assert.Empty(customer.Consents);
    }
}
