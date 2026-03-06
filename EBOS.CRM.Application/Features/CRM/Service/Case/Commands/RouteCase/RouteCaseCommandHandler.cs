using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Application.Shared.Audit;
using EBOS.CRM.Application.Shared.Observability;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using EBOS.CRM.Domain.Interfaces.Services.CRM;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Service.Case.Commands.RouteCase;

public sealed class RouteCaseCommandHandler(
    ICaseRepository repository,
    ICaseRoutingService routingService,
    IAuditService auditService,
    ICurrentUserContext currentUser,
    IMapper mapper,
    IDomainOperationalEventPublisher? domainOperationalEventPublisher = null) : IRequestHandler<RouteCaseCommand, CaseResponse?>
{
    public async Task<CaseResponse?> Handle(RouteCaseCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entityRequest = request.CaseRequest ?? throw new ArgumentNullException(nameof(request.CaseRequest));
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        if (entity.ClosedAt.HasValue)
        {
            throw new DomainRuleViolationException("Cannot route a closed case.", "DOMAIN_RULE_VIOLATION_CASE_CLOSED_ROUTE");
        }

        var oldValues = AuditSerialization.Serialize(entity);
        RouteCaseResult route;
        try
        {
            route = await routingService.RouteAsync(entity, entityRequest.Force, cancellationToken);
        }
        catch (Exception ex) when (DomainTransientFailureClassifier.TryClassify(ex, nameof(Handle), out var transient))
        {
            throw transient;
        }

        if (route.QueueId != entity.QueueId)
        {
            entity.AssignQueue(route.QueueId);
        }

        if (route.OwnerUserId.HasValue && (entity.OwnerUserId <= 0 || entityRequest.Force))
        {
            entity.AssignOwner(route.OwnerUserId.Value);
        }

        await repository.BeginTransactionAsync(cancellationToken);

        try
        {
            await repository.UpdateAsync(entity, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            var auditRequest = new AuditInsertRequest(
                UserId: currentUser.UserId,
                TimeStamp: DateTimeOffset.UtcNow,
                Action: AuditActions.Update,
                Entity: nameof(Domain.Entities.CRM.Case),
                RegisterId: entity.Id,
                OldValues: oldValues,
                NewValues: AuditSerialization.Serialize(entity),
                CorrelationId: currentUser.CorrelationId);

            await auditService.InsertAuditAsync(auditRequest, cancellationToken);
            if (domainOperationalEventPublisher is not null)
            {
                await domainOperationalEventPublisher.PublishAsync(
                    nameof(Domain.Entities.CRM.Case),
                    entity.Id,
                    entity.DequeueOperationalEvents(),
                    cancellationToken);
            }
            await repository.CommitAsync(cancellationToken);
        }
        catch
        {
            await repository.RollbackAsync(cancellationToken);
            throw;
        }

        return mapper.Map<CaseResponse>(entity);
    }
}
