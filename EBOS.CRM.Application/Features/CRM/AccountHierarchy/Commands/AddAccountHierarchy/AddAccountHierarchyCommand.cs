using EBOS.CRM.Contracts.Requests.CRM.AccountHierarchy;
using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.AccountHierarchy.Commands.AddAccountHierarchy;

public record AddAccountHierarchyCommand(AddAccountHierarchyRequest AccountHierarchyRequest)
    : IRequest<AccountHierarchyResponse>;
