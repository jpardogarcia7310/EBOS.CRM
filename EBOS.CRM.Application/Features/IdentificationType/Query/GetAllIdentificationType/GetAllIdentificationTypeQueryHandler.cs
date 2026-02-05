using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EBOS.CRM.Application.Contracts.Responses;
using EBOS.CRM.Domain.Interfaces.Repositories;
using MapsterMapper;
using MediatR;
using EBOS.CRM.Application.Contracts.Responses.Common;

namespace EBOS.CRM.Application.Features.IdentificationType.Query.GetAllIdentificationType;

public class GetAllIdentificationTypeQueryHandler(IIdentificationTypeRepository repository, IMapper mapper)
    : IRequestHandler<GetAllIdentificationTypeQuery, PagedResult<IdentificationTypeResponse>>
{
    private readonly IIdentificationTypeRepository _repository = repository ??
                                                                 throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<PagedResult<IdentificationTypeResponse>> Handle(GetAllIdentificationTypeQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entities = await _repository.GetAllPagedAsync(request.PageNumber, request.PageSize, cancellationToken);
        var items = _mapper.Map<IReadOnlyCollection<IdentificationTypeResponse>>(entities);
        var total = await _repository.CountAsync(cancellationToken);
        return new PagedResult<IdentificationTypeResponse>(items, total);
    }
}










