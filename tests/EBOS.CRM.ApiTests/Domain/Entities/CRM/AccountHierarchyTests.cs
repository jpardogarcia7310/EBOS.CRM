using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Services.CRM;
using Moq;

namespace EBOS.CRM.ApiTests.Domain.Entities.CRM;

public class AccountHierarchyTests
{
    [Fact]
    public void Create_WithSameParentChild_Throws()
    {
        Assert.ThrowsAny<DomainException>(() =>
            AccountHierarchy.Create(1, 10, 10, "GROUP_PARENT", DateTime.UtcNow));
    }

    [Fact]
    public void EndRelation_WithValidToBeforeValidFrom_Throws()
    {
        var validFrom = new DateTime(2026, 1, 10, 0, 0, 0, DateTimeKind.Utc);
        var entity = AccountHierarchy.Create(1, 10, 20, "GROUP_PARENT", validFrom);

        Assert.ThrowsAny<DomainException>(() => entity.EndRelation(validFrom.AddDays(-1)));
    }

    [Fact]
    public async Task AssignParentAsync_InvokesCycleInvariant()
    {
        var validFrom = DateTime.UtcNow;
        var entity = AccountHierarchy.Create(1, 10, 20, "GROUP_PARENT", validFrom);
        var invariant = new Mock<IAccountHierarchyAcyclicInvariant>(MockBehavior.Strict);
        invariant
            .Setup(x => x.EnsureNoCycleAsync(1, 11, 21, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        await entity.AssignParentAsync(1, 11, 21, "GROUP_PARENT", validFrom, invariant.Object, CancellationToken.None);

        invariant.Verify();
        Assert.Equal(11, entity.ParentCorporateCustomerId);
        Assert.Equal(21, entity.ChildCorporateCustomerId);
    }
}


