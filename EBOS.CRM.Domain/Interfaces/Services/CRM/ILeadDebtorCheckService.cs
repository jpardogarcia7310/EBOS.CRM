using EBOS.CRM.Contracts.Requests.CRM.Lead;
using EBOS.CRM.Contracts.Responses.CRM;

namespace EBOS.CRM.Domain.Interfaces.Services.CRM;

public interface ILeadDebtorCheckService
{
    Task<LeadDebtorCheckResponse> CheckAsync(LeadDebtorCheckRequest request,
        CancellationToken cancellationToken = default);
}