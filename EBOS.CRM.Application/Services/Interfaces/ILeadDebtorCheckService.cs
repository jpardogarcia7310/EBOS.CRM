using EBOS.CRM.Application.Contracts.Requests.CRM.Lead;
using EBOS.CRM.Application.Contracts.Responses.CRM;

namespace EBOS.CRM.Application.Services.Interfaces;

public interface ILeadDebtorCheckService
{
    Task<LeadDebtorCheckResponse> CheckAsync(LeadDebtorCheckRequest request,
        CancellationToken cancellationToken = default);
}
