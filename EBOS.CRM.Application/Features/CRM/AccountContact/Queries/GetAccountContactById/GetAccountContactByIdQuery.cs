using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.AccountContact.Queries.GetAccountContactById;

public record GetAccountContactByIdQuery(long Id) : IRequest<AccountContactResponse?>;
