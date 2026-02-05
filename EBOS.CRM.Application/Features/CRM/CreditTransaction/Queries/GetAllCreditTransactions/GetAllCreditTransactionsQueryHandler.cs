using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;
using EBOS.CRM.Application.Contracts.Responses.Common;

namespace EBOS.CRM.Application.Features.CRM.CreditTransaction.Queries.GetAllCreditTransactions;

public class GetAllCreditTransactionsQueryHandler(ICreditTransactionRepository repository, IMapper mapper)
    : IRequestHandler<GetAllCreditTransactionsQuery, PagedResult<CreditTransactionResponse>>
{
    private readonly ICreditTransactionRepository _repository = repository ?? 
                                                                throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<PagedResult<CreditTransactionResponse>> Handle(GetAllCreditTransactionsQuery request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entities = await _repository.GetAllPagedAsync(request.PageNumber, 
            request.PageSize, cancellationToken);
        var items = _mapper.Map<IReadOnlyCollection<CreditTransactionResponse>>(entities);
        var total = await _repository.CountAsync(cancellationToken);
        return new PagedResult<CreditTransactionResponse>(items, total);
    }
}










