using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;
using EBOS.CRM.Application.Contracts.Requests.Common;
using EBOS.CRM.Application.Contracts.Responses.Common;

namespace EBOS.CRM.Application.Features.CRM.BankInformation.Queries.GetAllBankInformations;

public record GetAllBankInformationsQuery(PagedQueryRequest Query) : IRequest<PagedResponse<BankInformationResponse>>;




