using EBOS.CRM.Contracts.Responses.Common;
using EBOS.CRM.Contracts.Responses.EBOS;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using EBOS.CRM.Domain.Interfaces.Services.EBOS;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.EBOS.TenantConfiguration.Queries.GetAllTenantConfigurations;

public class GetAllTenantConfigurationsQueryHandler(ITenantConfigurationRepository repository, IMapper mapper, IEbosReferenceLookupService? referenceLookupService = null)
    : IRequestHandler<GetAllTenantConfigurationsQuery, PagedResult<TenantConfigurationResponse>>
{
    private readonly ITenantConfigurationRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<PagedResult<TenantConfigurationResponse>> Handle(GetAllTenantConfigurationsQuery request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entities = referenceLookupService is null
            ? await _repository.GetAllPagedAsync(request.PageNumber, request.PageSize, cancellationToken)
            : await referenceLookupService.GetTenantConfigurationsPagedAsync(request.PageNumber, request.PageSize, cancellationToken);
        var items = _mapper.Map<IReadOnlyCollection<TenantConfigurationResponse>>(entities);
        var total = referenceLookupService is null
            ? await _repository.CountAsync(cancellationToken)
            : await referenceLookupService.CountTenantConfigurationsAsync(cancellationToken);
        return new PagedResult<TenantConfigurationResponse>(items, total);
    }
}
