

using MediatR;


namespace EBOS.CRM.Application.Features.CRM.TaxInformation.Commands.DeleteTaxInformation;

public record DeleteTaxInformationCommand(long Id) : IRequest<bool>;




