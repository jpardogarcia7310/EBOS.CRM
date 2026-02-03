using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;


namespace EBOS.CRM.Application.Features.CRM.CreditTransaction.Queries.GetAllCreditTransactions;

public class GetAllCreditTransactionsQueryHandler(ICreditTransactionRepository repository, IMapper mapper)
    : IRequestHandler<GetAllCreditTransactionsQuery, IReadOnlyCollection<CreditTransactionResponse>>
{
    private readonly ICreditTransactionRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<IReadOnlyCollection<CreditTransactionResponse>> Handle(GetAllCreditTransactionsQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entities = await _repository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyCollection<CreditTransactionResponse>>(entities);
    }
}









