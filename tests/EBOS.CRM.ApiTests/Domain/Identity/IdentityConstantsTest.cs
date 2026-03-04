using EBOS.CRM.Domain.Identity;

namespace EBOS.CRM.ApiTests.Domain.Identity;

public class IdentityConstantsTest
{
    [Fact]
    public void PermissionKeys_Crm_HasExpectedCoreValues()
    {
        Assert.Equal("Crm.Customer.Read", PermissionKeys.Crm.CustomerRead);
        Assert.Equal("Crm.Customer.Create", PermissionKeys.Crm.CustomerCreate);
        Assert.Equal("Crm.CaseActivity.Update", PermissionKeys.Crm.CaseActivityUpdate);
    }

    [Fact]
    public void PolicyKeys_HasExpectedCoreValues()
    {
        Assert.Equal("Policy.Operations.Observability.Read", PolicyKeys.Operations.ObservabilityRead);
        Assert.Equal("Policy.Crm.Customer.Pii.Read", PolicyKeys.Crm.CustomerPiiRead);
        Assert.Equal("Policy.Crm.Quote.Delete", PolicyKeys.Crm.QuoteDelete);
    }

    [Fact]
    public void RoleNames_AreExpected()
    {
        Assert.Equal("crm.readonly", RoleNames.CrmReadOnly);
        Assert.Equal("crm.editor", RoleNames.CrmEditor);
        Assert.Equal("crm.admin", RoleNames.CrmAdmin);
    }
}
