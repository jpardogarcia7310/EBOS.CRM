using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.BankInformation.Queries.GetBankInformationById;

public class GetBankInformationByIdQueryHandler(IBankInformationRepository repository, IMapper mapper)
    : IRequestHandler<GetBankInformationByIdQuery, BankInformationResponse?>
{
    private readonly IBankInformationRepository _repository = repository ??
                                                              throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<BankInformationResponse?> Handle(GetBankInformationByIdQuery request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);
        return entity is null ? null : _mapper.Map<BankInformationResponse>(entity);
    }
}




