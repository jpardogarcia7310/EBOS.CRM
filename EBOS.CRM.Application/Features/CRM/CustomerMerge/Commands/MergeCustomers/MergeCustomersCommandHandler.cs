using EBOS.CRM.Application.Shared.Audit;
using EBOS.CRM.Contracts.Requests.CRM.CustomerMerge;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CustomerMerge.Commands.MergeCustomers;

public class MergeCustomersCommandHandler(
    ICustomerRepository customerRepository,
    IAuditService auditService,
    ICurrentUserContext currentUser)
    : IRequestHandler<MergeCustomersCommand, CustomerMergeResultResponse>
{
    public async Task<CustomerMergeResultResponse> Handle(MergeCustomersCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var mergeRequest = request.Request ?? throw new ArgumentNullException(nameof(request.Request));

        var winner = await customerRepository.GetByIdAsync(mergeRequest.WinnerCustomerId, cancellationToken)
            ?? throw new InvalidOperationException("Winner customer not found.");
        if (winner.TenantId != mergeRequest.TenantId)
        {
            throw new InvalidOperationException("Winner customer tenant mismatch.");
        }

        var mergeIds = mergeRequest.MergeCustomerIds
            .Where(id => id != mergeRequest.WinnerCustomerId)
            .Distinct()
            .ToList();

        var merged = new List<long>();

        await customerRepository.BeginTransactionAsync(cancellationToken);

        try
        {
            foreach (var mergeId in mergeIds)
            {
                var entity = await customerRepository.GetByIdAsync(mergeId, cancellationToken);
                if (entity is null)
                {
                    continue;
                }

                if (entity.TenantId != mergeRequest.TenantId)
                {
                    throw new InvalidOperationException("Customer tenant mismatch in merge list.");
                }

                entity.Erased = true;
                await customerRepository.UpdateAsync(entity, cancellationToken);
                merged.Add(entity.Id);
            }

            await customerRepository.SaveChangesAsync(cancellationToken);

            var auditRequest = new AuditInsertRequest(
                UserId: currentUser.UserId,
                TimeStamp: DateTimeOffset.UtcNow,
                Action: AuditActions.Update,
                Entity: nameof(Domain.Entities.CRM.Customer),
                RegisterId: winner.Id,
                OldValues: null,
                NewValues: AuditSerialization.Serialize(winner),
                CorrelationId: currentUser.CorrelationId);

            await auditService.InsertAuditAsync(auditRequest, cancellationToken);
            await customerRepository.CommitAsync(cancellationToken);
        }
        catch
        {
            await customerRepository.RollbackAsync(cancellationToken);
            throw;
        }

        return new CustomerMergeResultResponse(winner.Id, merged, "Merged");
    }
}
