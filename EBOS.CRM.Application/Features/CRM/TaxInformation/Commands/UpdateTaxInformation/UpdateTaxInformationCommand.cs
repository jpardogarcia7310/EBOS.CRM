using EBOS.CRM.Contracts.Requests.CRM.TaxInformation;
using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.TaxInformation.Commands.UpdateTaxInformation;

public record UpdateTaxInformationCommand(long Id, UpdateTaxInformationRequest TaxInformationRequest) :
    IRequest<TaxInformationResponse?>;




