using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;


namespace EBOS.CRM.Application.Features.CRM.BankInformation.Queries.GetAllBankInformations;

public record GetAllBankInformationsQuery : IRequest<IReadOnlyCollection<BankInformationResponse>>;









