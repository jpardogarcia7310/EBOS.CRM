using System.ComponentModel.DataAnnotations;
using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.ApiTests.Domain.Entities.CRM;

public class CorporateCustomerTests
{
    [Fact]
    public void CorporateCustomer_Collections_AreInitialized()
    {
        var entity = new CorporateCustomer();
        Assert.NotNull(entity.BranchOffices);
        Assert.NotNull(entity.ParentRelationships);
        Assert.NotNull(entity.ChildRelationships);
        Assert.Empty(entity.BranchOffices);
        Assert.Empty(entity.ParentRelationships);
        Assert.Empty(entity.ChildRelationships);
    }

    [Fact]
    public void LegalName_AndTaxIdentification_HaveAnnotations()
    {
        var legalName = typeof(CorporateCustomer).GetProperty(nameof(CorporateCustomer.LegalName));
        var taxId = typeof(CorporateCustomer).GetProperty(nameof(CorporateCustomer.TaxIdentification));

        Assert.NotNull(legalName!.GetCustomAttributes(typeof(RequiredAttribute), true).SingleOrDefault());
        Assert.Equal(200, legalName.GetCustomAttributes(typeof(MaxLengthAttribute), true)
            .Cast<MaxLengthAttribute>().Single().Length);

        Assert.NotNull(taxId!.GetCustomAttributes(typeof(RequiredAttribute), true).SingleOrDefault());
        Assert.Equal(20, taxId.GetCustomAttributes(typeof(MaxLengthAttribute), true)
            .Cast<MaxLengthAttribute>().Single().Length);
    }
}
