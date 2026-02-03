using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;


namespace EBOS.CRM.Application.Features.CRM.BankInformation.Queries.GetAllBankInformations;

public class GetAllBankInformationsQueryHandler(IBankInformationRepository repository, IMapper mapper)
    : IRequestHandler<GetAllBankInformationsQuery, IReadOnlyCollection<BankInformationResponse>>
{
    private readonly IBankInformationRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<IReadOnlyCollection<BankInformationResponse>> Handle(GetAllBankInformationsQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entities = await _repository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyCollection<BankInformationResponse>>(entities);
    }
}









