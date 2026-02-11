using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;
using EBOS.CRM.Application.Contracts.Responses.Common;

namespace EBOS.CRM.Application.Features.CRM.CreditAccount.Queries.GetAllCreditAccounts;

public class GetAllCreditAccountsQueryHandler(ICreditAccountRepository repository, IMapper mapper)
    : IRequestHandler<GetAllCreditAccountsQuery, PagedResult<CreditAccountResponse>>
{
    private readonly ICreditAccountRepository _repository = repository ??
                                                            throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<PagedResult<CreditAccountResponse>> Handle(GetAllCreditAccountsQuery request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entities = await _repository.GetAllPagedAsync(request.PageNumber,
            request.PageSize, cancellationToken);
        var items = _mapper.Map<IReadOnlyCollection<CreditAccountResponse>>(entities);
        var total = await _repository.CountAsync(cancellationToken);
        return new PagedResult<CreditAccountResponse>(items, total);
    }
}










