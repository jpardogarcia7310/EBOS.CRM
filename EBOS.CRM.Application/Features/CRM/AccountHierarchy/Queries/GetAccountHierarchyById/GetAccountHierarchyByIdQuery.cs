using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.AccountHierarchy.Queries.GetAccountHierarchyById;

public record GetAccountHierarchyByIdQuery(long Id) : IRequest<AccountHierarchyResponse?>;
