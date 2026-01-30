using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Address.Commands.AddAddress;

public class AddAddressCommandHandler(IAddressRepository repository, IMapper mapper)
    : IRequestHandler<AddAddressCommand, AddressResponse>
{
    public async Task<AddressResponse> Handle(AddAddressCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var addressRequest = request.AddressRequest 
                             ?? throw new ArgumentNullException(nameof(request.AddressRequest));
        // Mapster creates the complete entity
        var entity = mapper.Map<Domain.Entities.CRM.Address>(addressRequest);
        
        await repository.BeginTransactionAsync(cancellationToken);

        try
        {
            await repository.AddAsync(entity, cancellationToken);
            await repository.CommitAsync(cancellationToken);
        }
        catch
        {
            await repository.RollbackAsync(cancellationToken);
            throw;
        }
        return mapper.Map<AddressResponse>(entity);
    }
}