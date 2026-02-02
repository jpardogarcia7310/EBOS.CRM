using MediatR;

namespace EBOS.CRM.Application.Features.CRM.TaxInformationAddress.Commands.DeleteTaxInformationAddress;

public record DeleteTaxInformationAddressCommand(long Id) : IRequest<bool>;
