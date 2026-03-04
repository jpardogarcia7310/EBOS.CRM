using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.BankInformation.Queries.GetBankInformationById;

public record GetBankInformationByIdQuery(long Id) : IRequest<BankInformationResponse?>;




