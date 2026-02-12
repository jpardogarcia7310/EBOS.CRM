using EBOS.CRM.Contracts.Requests.CRM.TaxInformationAddress;
using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.TaxInformationAddress.Commands.UpdateTaxInformationAddress;

public record UpdateTaxInformationAddressCommand(long Id,
    UpdateTaxInformationAddressRequest TaxInformationAddressRequest) : IRequest<TaxInformationAddressResponse?>;




