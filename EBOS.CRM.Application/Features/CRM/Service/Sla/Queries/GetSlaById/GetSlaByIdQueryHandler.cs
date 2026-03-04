using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Service.Sla.Queries.GetSlaById;

public class GetSlaByIdQueryHandler(ISlaRepository repository, IMapper mapper)
    : IRequestHandler<GetSlaByIdQuery, SlaResponse?>
{
    public async Task<SlaResponse?> Handle(GetSlaByIdQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        return entity is null ? null : mapper.Map<SlaResponse>(entity);
    }
}
