using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;
using EBOS.CRM.Application.Contracts.Responses.Common;

namespace EBOS.CRM.Application.Features.CRM.BankInformation.Queries.GetAllBankInformations;

public record GetAllBankInformationsQuery(int PageNumber = 1, int PageSize = 50) : IRequest<PagedResult<BankInformationResponse>>;









