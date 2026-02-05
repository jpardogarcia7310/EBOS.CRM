using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;
using EBOS.CRM.Application.Contracts.Responses.Common;

namespace EBOS.CRM.Application.Features.CRM.TaxInformation.Queries.GetAllTaxInformations;

public class GetAllTaxInformationsQueryHandler(ITaxInformationRepository repository, IMapper mapper)
    : IRequestHandler<GetAllTaxInformationsQuery, PagedResult<TaxInformationResponse>>
{
    private readonly ITaxInformationRepository _repository = repository ??
                                                             throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<PagedResult<TaxInformationResponse>> Handle(GetAllTaxInformationsQuery request, 
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










