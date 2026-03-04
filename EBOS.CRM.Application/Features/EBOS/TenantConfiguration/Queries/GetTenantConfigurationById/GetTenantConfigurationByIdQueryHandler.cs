using EBOS.CRM.Contracts.Responses.EBOS;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.EBOS.TenantConfiguration.Queries.GetTenantConfigurationById;

public class GetTenantConfigurationByIdQueryHandler(ITenantConfigurationRepository repository, IMapper mapper)
    : IRequestHandler<GetTenantConfigurationByIdQuery, TenantConfigurationResponse?>
{
    public async Task<TenantConfigurationResponse?> Handle(GetTenantConfigurationByIdQuery request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        return entity is null ? null : mapper.Map<TenantConfigurationResponse>(entity);
    }
}
