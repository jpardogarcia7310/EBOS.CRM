using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CreditAccount.Queries.GetAllCreditAccounts;

public class GetAllCreditAccountsQueryHandler(ICreditAccountRepository repository, IMapper mapper)
    : IRequestHandler<GetAllCreditAccountsQuery, ICollection<CreditAccountResponse>>
{
    private readonly ICreditAccountRepository _repository = repository ??
                                                           throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<ICollection<CreditAccountResponse>> Handle(GetAllCreditAccountsQuery request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entities = await _repository.GetAllAsync(cancellationToken);
        return entities.Select(e => _mapper.Map<CreditAccountResponse>(e)).ToList();
    }
}
