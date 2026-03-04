using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.Domain.Interfaces.Services.CRM;

public interface ICaseRoutingService
{
    Task<RouteCaseResult> RouteAsync(Case entity, bool force, CancellationToken cancellationToken = default);
}

public sealed record RouteCaseResult(
    long QueueId,
    long? OwnerUserId,
    string Rule
);