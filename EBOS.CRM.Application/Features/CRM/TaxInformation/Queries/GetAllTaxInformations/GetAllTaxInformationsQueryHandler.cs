using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;
using EBOS.CRM.Application.Contracts.Responses.Common;

namespace EBOS.CRM.Application.Features.CRM.TaxInformation.Queries.GetAllTaxInformations;

public class GetAllTaxInformationsQueryHandler(ITaxInformationRepository repository, IMapper mapper)
    : IRequestHandler<GetAllTaxInformationsQuery, PagedResponse<TaxInformationResponse>>
{
    private readonly ITaxInformationRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<PagedResponse<TaxInformationResponse>> Handle(GetAllTaxInformationsQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var result = await _repository.GetPagedAsync(request.Query.ToPagedQuery(), cancellationToken);
        var items = _mapper.Map<IReadOnlyCollection<TaxInformationResponse>>(result.Items);
        return new PagedResponse<TaxInformationResponse>(items, result.PageNumber, result.PageSize, result.TotalCount, result.TotalPages, result.SortBy, result.SortDirection, result.Filter);
    }
}




