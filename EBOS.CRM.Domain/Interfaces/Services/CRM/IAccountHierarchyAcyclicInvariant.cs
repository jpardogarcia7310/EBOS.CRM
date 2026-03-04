namespace EBOS.CRM.Domain.Interfaces.Services.CRM;

/// <summary>
/// Domain invariant: account hierarchies must be acyclic. Any operation that creates a parent-child
/// relationship must validate this invariant before persisting changes.
/// </summary>
public interface IAccountHierarchyAcyclicInvariant
{
    /// <summary>
    /// Ensures that linking <paramref name="parentCorporateCustomerId"/> as parent of
    /// <paramref name="childCorporateCustomerId"/> does not introduce a cycle.
    /// Implementations must throw <see cref="InvalidOperationException"/> when a cycle would be created.
    /// </summary>
    Task EnsureNoCycleAsync(long tenantId, long parentCorporateCustomerId, long childCorporateCustomerId,
        CancellationToken cancellationToken = default);
}
