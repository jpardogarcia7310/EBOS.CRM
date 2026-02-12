using EBOS.CRM.Contracts.Requests.CRM.TaxInformation;
using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.TaxInformation.Commands.AddTaxInformation;

public record AddTaxInformationCommand(AddTaxInformationRequest TaxInformationRequest) :
    IRequest<TaxInformationResponse>;




