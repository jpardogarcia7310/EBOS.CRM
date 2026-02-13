using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Contracts.Responses.Common;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.AccountContact.Queries.GetAllAccountContacts;

public record GetAllAccountContactsQuery(int PageNumber = 1, int PageSize = 50)
    : IRequest<PagedResult<AccountContactResponse>>;
