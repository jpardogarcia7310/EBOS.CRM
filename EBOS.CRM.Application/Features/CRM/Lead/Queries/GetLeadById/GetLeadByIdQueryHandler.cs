using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Lead.Queries.GetLeadById;

public class GetLeadByIdQueryHandler(ILeadRepository repository, IMapper mapper)
    : IRequestHandler<GetLeadByIdQuery, LeadResponse?>
{
    public async Task<LeadResponse?> Handle(GetLeadByIdQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        return entity is null ? null : mapper.Map<LeadResponse>(entity);
    }
}
