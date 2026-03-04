using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Infrastructure.Services.CRM;
using Moq;

namespace EBOS.CRM.ApiTests.Infrastructure.Services.CRM;

public class AccountHierarchyCycleGuardTest
{
    [Fact]
    public async Task CreatesCycleAsync_WhenParentEqualsChild_ReturnsTrue()
    {
        var repo = new Mock<IAccountHierarchyRepository>(MockBehavior.Strict);
        var sut = new AccountHierarchyCycleGuard(repo.Object);

        var result = await sut.CreatesCycleAsync(1, 10, 10);

        Assert.True(result);
    }

    [Fact]
    public async Task CreatesCycleAsync_WhenAncestorChainContainsChild_ReturnsTrue()
    {
        var repo = new Mock<IAccountHierarchyRepository>(MockBehavior.Strict);
        repo.Setup(x => x.GetParentIdsByChildIdsAsync(1, It.IsAny<IReadOnlyCollection<long>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((long _, IReadOnlyCollection<long> children, CancellationToken _) =>
            {
                // 2 -> 3, then 3 -> 5, detect child=5 as ancestor of parent=2
                if (children.Contains(2)) return new List<long> { 3 };
                if (children.Contains(3)) return new List<long> { 5 };
                return new List<long>();
            });

        var sut = new AccountHierarchyCycleGuard(repo.Object);
        var result = await sut.CreatesCycleAsync(1, parentCorporateCustomerId: 2, childCorporateCustomerId: 5);

        Assert.True(result);
    }

    [Fact]
    public async Task EnsureNoCycleAsync_WhenCycleDetected_Throws()
    {
        var repo = new Mock<IAccountHierarchyRepository>(MockBehavior.Strict);
        repo.Setup(x => x.GetParentIdsByChildIdsAsync(1, It.IsAny<IReadOnlyCollection<long>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyCollection<long>)new[] { 9L });

        var sut = new AccountHierarchyCycleGuard(repo.Object);
        var act = () => sut.EnsureNoCycleAsync(1, parentCorporateCustomerId: 2, childCorporateCustomerId: 9);

        await Assert.ThrowsAsync<InvalidOperationException>(act);
    }
}
