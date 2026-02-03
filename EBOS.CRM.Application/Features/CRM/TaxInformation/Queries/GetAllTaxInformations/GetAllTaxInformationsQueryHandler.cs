using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;


namespace EBOS.CRM.Application.Features.CRM.TaxInformation.Queries.GetAllTaxInformations;

public class GetAllTaxInformationsQueryHandler(ITaxInformationRepository repository, IMapper mapper)
    : IRequestHandler<GetAllTaxInformationsQuery, IReadOnlyCollection<TaxInformationResponse>>
{
    private readonly ITaxInformationRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<IReadOnlyCollection<TaxInformationResponse>> Handle(GetAllTaxInformationsQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entities = await _repository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyCollection<TaxInformationResponse>>(entities);
    }
}









