using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Application.Services.Interfaces;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Lead.Queries.CheckLeadDebtor;

public class CheckLeadDebtorQueryHandler(ILeadDebtorCheckService checkService)
    : IRequestHandler<CheckLeadDebtorQuery, LeadDebtorCheckResponse>
{
    public async Task<LeadDebtorCheckResponse> Handle(CheckLeadDebtorQuery request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await checkService.CheckAsync(request.Request, cancellationToken);
    }
}
