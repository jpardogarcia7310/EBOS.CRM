using EBOS.CRM.Contracts.Requests.CRM.CustomerMerge;
using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CustomerMerge.Commands.MergeCustomers;

public record MergeCustomersCommand(MergeCustomersRequest Request) : IRequest<CustomerMergeResultResponse>;
