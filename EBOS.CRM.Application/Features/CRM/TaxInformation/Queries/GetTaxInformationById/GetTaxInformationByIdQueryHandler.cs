using System;
using System.Threading;
using System.Threading.Tasks;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.TaxInformation.Queries.GetTaxInformationById;

public class GetTaxInformationByIdQueryHandler(ITaxInformationRepository repository, IMapper mapper)
    : IRequestHandler<GetTaxInformationByIdQuery, TaxInformationResponse?>
{
    private readonly ITaxInformationRepository _repository = repository ?? 
                                                             throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<TaxInformationResponse?> Handle(GetTaxInformationByIdQuery request, 
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);
        return entity is null ? null : _mapper.Map<TaxInformationResponse>(entity);
    }
}




