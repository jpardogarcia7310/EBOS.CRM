using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;
using EBOS.CRM.Application.Contracts.Responses.Common;

namespace EBOS.CRM.Application.Features.CRM.TaxInformation.Queries.GetAllTaxInformation;

public class GetAllTaxInformationQueryHandler(ITaxInformationRepository repository, IMapper mapper)
    : IRequestHandler<GetAllTaxInformationQuery, PagedResult<TaxInformationResponse>>
{
    private readonly ITaxInformationRepository _repository = repository ??
                                                            throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<PagedResult<TaxInformationResponse>> Handle(GetAllTaxInformationQuery request, 
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entities = await _repository.GetAllPagedAsync(request.PageNumber, 
            request.PageSize, cancellationToken);
        var items = _mapper.Map<IReadOnlyCollection<TaxInformationResponse>>(entities);
        var total = await _repository.CountAsync(cancellationToken);
        return new PagedResult<TaxInformationResponse>(items, total);
    }
}






