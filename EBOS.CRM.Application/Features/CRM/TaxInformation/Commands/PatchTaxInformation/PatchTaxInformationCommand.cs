using EBOS.CRM.Contracts.Requests.CRM.TaxInformation;
using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.TaxInformation.Commands.PatchTaxInformation;

public sealed record PatchTaxInformationCommand(long Id, PatchTaxInformationRequest TaxInformationRequest)
    : IRequest<TaxInformationResponse?>;




