using EBOS.CRM.Application.Contracts.Requests.Services;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Application.Services.Audit;
using EBOS.CRM.Application.Services.Interfaces;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.TaxInformationAddress.Commands.AddTaxInformationAddress;

public class AddTaxInformationAddressCommandHandler(ITaxInformationAddressRepository repository, 
    IAuditService auditService, ICurrentUserContext currentUser, IMapper mapper) : 
    IRequestHandler<AddTaxInformationAddressCommand, TaxInformationAddressResponse>
{
    public async Task<TaxInformationAddressResponse> Handle(AddTaxInformationAddressCommand request, 
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entityRequest = request.TaxInformationAddressRequest ?? 
                            throw new ArgumentNullException(nameof(request.TaxInformationAddressRequest));
        var entity = mapper.Map<EBOS.CRM.Domain.Entities.CRM.TaxInformationAddress>(entityRequest);

        await repository.BeginTransactionAsync(cancellationToken);

        try
        {
            await repository.AddAsync(entity, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            var auditRequest = new AuditInsertRequest(
                UserId: currentUser.UserId,
                TimeStamp: DateTimeOffset.UtcNow,
                Action: AuditActions.Add,
                Entity: nameof(Domain.Entities.CRM.TaxInformationAddress),
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

        return mapper.Map<TaxInformationAddressResponse>(entity);
    }
}




