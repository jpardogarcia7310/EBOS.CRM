using EBOS.CRM.Contracts.Requests.CRM.AccountHierarchy;
using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.AccountHierarchy.Commands.EndAccountHierarchy;

public record EndAccountHierarchyCommand(long Id, EndAccountHierarchyRequest AccountHierarchyRequest)
    : IRequest<AccountHierarchyResponse?>;
