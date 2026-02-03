using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.BranchOfficeAddress.Queries.GetBranchOfficeAddressById;

public record GetBranchOfficeAddressByIdQuery(long Id) : IRequest<BranchOfficeAddressResponse?>;




