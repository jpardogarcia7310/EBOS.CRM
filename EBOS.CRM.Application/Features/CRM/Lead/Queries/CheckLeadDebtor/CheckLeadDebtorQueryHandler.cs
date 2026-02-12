using EBOS.CRM.Contracts.Requests.CRM.Lead;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Lead.Queries.CheckLeadDebtor;

public class CheckLeadDebtorQueryHandler(ILeadDebtorCheckService checkService)
    : IRequestHandler<CheckLeadDebtorQuery, LeadDebtorCheckResponse>
{
    public async Task<LeadDebtorCheckResponse> Handle(CheckLeadDebtorQuery request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var payload = request.Request;
        var serviceRequest = new LeadDebtorCheckRequest(
            payload.TenantId,
            payload.Email,
            payload.Phone,
            payload.CompanyName,
            payload.ContactName);

        var serviceResponse = await checkService.CheckAsync(serviceRequest, cancellationToken);

        return new LeadDebtorCheckResponse(
            serviceResponse.IsDebtor,
            serviceResponse.CustomerId,
            serviceResponse.CustomerType,
            serviceResponse.Code,
            serviceResponse.Name,
            serviceResponse.Email,
            serviceResponse.Phone,
            serviceResponse.StatusId,
            serviceResponse.Status,
            serviceResponse.DebtorSince,
            serviceResponse.DebtAmount);
    }
}