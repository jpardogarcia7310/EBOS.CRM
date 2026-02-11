using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Service.Case.Queries.GetCaseById;

public class GetCaseByIdQueryHandler(ICaseRepository repository, IMapper mapper)
    : IRequestHandler<GetCaseByIdQuery, CaseResponse?>
{
    public async Task<CaseResponse?> Handle(GetCaseByIdQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        return entity is null ? null : mapper.Map<CaseResponse>(entity);
    }
}
