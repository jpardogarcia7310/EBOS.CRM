using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services.CRM;

namespace EBOS.CRM.Infrastructure.Services.CRM;

public class AccountHierarchyCycleGuard(IAccountHierarchyRepository repository)
    : IAccountHierarchyCycleGuard, IAccountHierarchyAcyclicInvariant
{
    public async Task EnsureNoCycleAsync(long tenantId, long parentCorporateCustomerId,
        long childCorporateCustomerId, CancellationToken cancellationToken = default)
    {
        if (await CreatesCycleAsync(tenantId, parentCorporateCustomerId, childCorporateCustomerId, cancellationToken))
        {
            throw new InvalidOperationException("Account hierarchy cycle detected.");
        }
    }

    public async Task<bool> CreatesCycleAsync(long tenantId, long parentCorporateCustomerId,
        long childCorporateCustomerId, CancellationToken cancellationToken = default)
    {
        if (parentCorporateCustomerId == childCorporateCustomerId)
        {
            return true;
        }

        // Walk up the hierarchy from parent to see if child is an ancestor.
        var visited = new HashSet<long> { parentCorporateCustomerId };
        var frontier = new Queue<long>();
        frontier.Enqueue(parentCorporateCustomerId);

        while (frontier.Count > 0)
        {
            var levelChildren = new List<long>(frontier.Count);
            while (frontier.Count > 0)
            {
                levelChildren.Add(frontier.Dequeue());
            }

            var parentIds = await repository.GetParentIdsByChildIdsAsync(tenantId, levelChildren, cancellationToken);
            foreach (var parentId in parentIds)
            {
                if (parentId == childCorporateCustomerId)
                {
                    return true;
                }

                if (visited.Add(parentId))
                {
                    frontier.Enqueue(parentId);
                }
            }
        }

        return false;
    }
}
