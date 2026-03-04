using EBOS.CRM.Contracts.Requests.CRM.CustomerMerge;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Contracts.Responses.Common;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CustomerMerge.Queries.FindCustomerDuplicates;

public record FindCustomerDuplicatesQuery(FindCustomerDuplicatesRequest Request, int PageNumber = 1, int PageSize = 50)
    : IRequest<PagedResult<CustomerDuplicateCandidateResponse>>;
