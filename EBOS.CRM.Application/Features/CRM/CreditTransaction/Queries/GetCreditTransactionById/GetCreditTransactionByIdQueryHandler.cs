using System;
using System.Threading;
using System.Threading.Tasks;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CreditTransaction.Queries.GetCreditTransactionById;

public class GetCreditTransactionByIdQueryHandler(ICreditTransactionRepository repository, IMapper mapper)
    : IRequestHandler<GetCreditTransactionByIdQuery, CreditTransactionResponse?>
{
    private readonly ICreditTransactionRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<CreditTransactionResponse?> Handle(GetCreditTransactionByIdQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);
        return entity is null ? null : _mapper.Map<CreditTransactionResponse>(entity);
    }
}




