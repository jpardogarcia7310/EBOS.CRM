using EBOS.CRM.Application.Contracts.Requests.CRM.TaxInformation;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.TaxInformation.Commands.UpdateTaxInformation;

public record UpdateTaxInformationCommand(long Id, UpdateTaxInformationRequest TaxInformationRequest) :
    IRequest<TaxInformationResponse?>;




