using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CorporateCustomer.Commands.DeleteCorporateCustomer;

public record DeleteCorporateCustomerCommand(long Id) : IRequest<bool>;
