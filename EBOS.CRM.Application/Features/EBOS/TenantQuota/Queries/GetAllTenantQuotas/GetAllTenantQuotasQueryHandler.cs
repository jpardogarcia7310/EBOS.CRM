using EBOS.CRM.Contracts.Responses.Common;
using EBOS.CRM.Contracts.Responses.EBOS;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using EBOS.CRM.Domain.Interfaces.Services.EBOS;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.EBOS.TenantQuota.Queries.GetAllTenantQuotas;

public class GetAllTenantQuotasQueryHandler(ITenantQuotaRepository repository, IMapper mapper, IEbosReferenceLookupService? referenceLookupService = null)
    : IRequestHandler<GetAllTenantQuotasQuery, PagedResult<TenantQuotaResponse>>
{
    private readonly ITenantQuotaRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<PagedResult<TenantQuotaResponse>> Handle(GetAllTenantQuotasQuery request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entities = referenceLookupService is null
            ? await _repository.GetAllPagedAsync(request.PageNumber, request.PageSize, cancellationToken)
            : await referenceLookupService.GetTenantQuotasPagedAsync(request.PageNumber, request.PageSize, cancellationToken);
        var items = _mapper.Map<IReadOnlyCollection<TenantQuotaResponse>>(entities);
        var total = referenceLookupService is null
            ? await _repository.CountAsync(cancellationToken)
            : await referenceLookupService.CountTenantQuotasAsync(cancellationToken);
        return new PagedResult<TenantQuotaResponse>(items, total);
    }
}
