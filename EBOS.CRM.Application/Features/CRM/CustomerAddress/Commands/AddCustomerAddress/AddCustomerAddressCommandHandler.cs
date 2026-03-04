using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Application.Shared.Audit;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CustomerAddress.Commands.AddCustomerAddress;

public class AddCustomerAddressCommandHandler(ICustomerAddressRepository repository, IAuditService auditService,
    ICurrentUserContext currentUser, IMapper mapper) :
    IRequestHandler<AddCustomerAddressCommand, CustomerAddressResponse>
{
    public async Task<CustomerAddressResponse> Handle(AddCustomerAddressCommand request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entityRequest = request.CustomerAddressRequest ??
                            throw new ArgumentNullException(nameof(request.CustomerAddressRequest));
        var entity = mapper.Map<global::EBOS.CRM.Domain.Entities.CRM.CustomerAddress>(entityRequest);

        await repository.BeginTransactionAsync(cancellationToken);

        try
        {
            await repository.AddAsync(entity, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            var auditRequest = new AuditInsertRequest(
                UserId: currentUser.UserId,
                TimeStamp: DateTimeOffset.UtcNow,
                Action: AuditActions.Add,
                Entity: nameof(Domain.Entities.CRM.CustomerAddress),
                RegisterId: entity.Id,
                OldValues: null,
                NewValues: AuditSerialization.Serialize(entity),
                CorrelationId: currentUser.CorrelationId);

            await auditService.InsertAuditAsync(auditRequest, cancellationToken);
            await repository.CommitAsync(cancellationToken);
        }
        catch
        {
            await repository.RollbackAsync(cancellationToken);
            throw;
        }

        return mapper.Map<CustomerAddressResponse>(entity);
    }
}




