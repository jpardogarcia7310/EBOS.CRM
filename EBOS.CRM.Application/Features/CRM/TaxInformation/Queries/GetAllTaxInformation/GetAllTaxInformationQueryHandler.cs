using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;


namespace EBOS.CRM.Application.Features.CRM.TaxInformation.Queries.GetAllTaxInformation;

public class GetAllTaxInformationQueryHandler(ITaxInformationRepository repository, IMapper mapper)
    : IRequestHandler<GetAllTaxInformationQuery, IReadOnlyCollection<TaxInformationResponse>>
{
    private readonly ITaxInformationRepository _repository = repository ??
                                                            throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<IReadOnlyCollection<TaxInformationResponse>> Handle(GetAllTaxInformationQuery request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entities = await _repository.GetAllAsync(cancellationToken);
        return entities.Select(e => _mapper.Map<TaxInformationResponse>(e)).ToList();
    }
}





