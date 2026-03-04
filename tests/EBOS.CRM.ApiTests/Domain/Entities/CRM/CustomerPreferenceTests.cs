using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.ApiTests.Domain.Entities.CRM;

public class CustomerPreferenceTests
{
    [Fact]
    public void Create_WithInvalidIds_Throws()
    {
        Assert.Throws<InvalidOperationException>(() =>
            CustomerPreference.Create(0, 1, 1, true, DateTime.UtcNow, 1));
        Assert.Throws<InvalidOperationException>(() =>
            CustomerPreference.Create(1, 0, 1, true, DateTime.UtcNow, 1));
        Assert.Throws<InvalidOperationException>(() =>
            CustomerPreference.Create(1, 1, 0, true, DateTime.UtcNow, 1));
    }

    [Fact]
    public void UpdatePreference_WithInvalidUpdatedBy_Throws()
    {
        var entity = CustomerPreference.Create(1, 10, 20, true, DateTime.UtcNow, 1);
        Assert.Throws<InvalidOperationException>(() => entity.UpdatePreference(false, DateTime.UtcNow, 0));
    }

    [Fact]
    public void ReassignCustomer_WithSameId_DoesNotChange()
    {
        var entity = CustomerPreference.Create(1, 10, 20, true, DateTime.UtcNow, 1);
        entity.ReassignCustomer(10);
        Assert.Equal(10, entity.CustomerId);
    }

    [Fact]
    public void MergeFrom_WithNull_Throws()
    {
        var entity = CustomerPreference.Create(1, 10, 20, true, DateTime.UtcNow, 1);
        Assert.Throws<ArgumentNullException>(() => entity.MergeFrom(null!));
    }
}
