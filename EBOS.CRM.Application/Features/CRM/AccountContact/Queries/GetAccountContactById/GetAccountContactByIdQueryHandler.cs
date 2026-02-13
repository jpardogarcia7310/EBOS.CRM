using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.AccountContact.Queries.GetAccountContactById;

public class GetAccountContactByIdQueryHandler(IAccountContactRepository repository, IMapper mapper)
    : IRequestHandler<GetAccountContactByIdQuery, AccountContactResponse?>
{
    public async Task<AccountContactResponse?> Handle(GetAccountContactByIdQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        return entity is null ? null : mapper.Map<AccountContactResponse>(entity);
    }
}
