using EBOS.CRM.Application.Contracts.Responses.Common;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.EBOS.TenantQuota.Queries.GetAllTenantQuotas;

public class GetAllTenantQuotasQueryHandler(ITenantQuotaRepository repository, IMapper mapper)
    : IRequestHandler<GetAllTenantQuotasQuery, PagedResult<TenantQuotaResponse>>
{
    private readonly ITenantQuotaRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<PagedResult<TenantQuotaResponse>> Handle(GetAllTenantQuotasQuery request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entities = await _repository.GetAllPagedAsync(request.PageNumber, request.PageSize, cancellationToken);
        var items = _mapper.Map<IReadOnlyCollection<TenantQuotaResponse>>(entities);
        var total = await _repository.CountAsync(cancellationToken);
        return new PagedResult<TenantQuotaResponse>(items, total);
    }
}
