using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Service.CaseActivity.Queries.GetCaseActivityById;

public class GetCaseActivityByIdQueryHandler(ICaseActivityRepository repository, IMapper mapper)
    : IRequestHandler<GetCaseActivityByIdQuery, CaseActivityResponse?>
{
    public async Task<CaseActivityResponse?> Handle(GetCaseActivityByIdQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        return entity is null ? null : mapper.Map<CaseActivityResponse>(entity);
    }
}
