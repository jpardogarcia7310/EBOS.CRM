using EBOS.CRM.Contracts.Requests.CRM.TaxInformationAddress;
using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.TaxInformationAddress.Commands.AddTaxInformationAddress;

public record AddTaxInformationAddressCommand(AddTaxInformationAddressRequest TaxInformationAddressRequest) :
    IRequest<TaxInformationAddressResponse>;




