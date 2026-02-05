using EBOS.CRM.Application.Contracts.Requests.CRM.TaxInformationAddress;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.TaxInformationAddress.Commands.UpdateTaxInformationAddress;

public record UpdateTaxInformationAddressCommand(long Id, 
    UpdateTaxInformationAddressRequest TaxInformationAddressRequest) : IRequest<TaxInformationAddressResponse?>;




