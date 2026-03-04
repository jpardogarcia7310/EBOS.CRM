using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CustomerPrivacy.Queries.GetCustomerPrivacyRequestById;

public sealed class GetCustomerPrivacyRequestByIdQueryHandler(ICustomerPrivacyRequestRepository repository)
    : IRequestHandler<GetCustomerPrivacyRequestByIdQuery, CustomerPrivacyRequestResponse?>
{
    public async Task<CustomerPrivacyRequestResponse?> Handle(GetCustomerPrivacyRequestByIdQuery request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null || entity.TenantId != request.TenantId)
        {
            return null;
        }

        return entity.ToResponse();
    }
}
