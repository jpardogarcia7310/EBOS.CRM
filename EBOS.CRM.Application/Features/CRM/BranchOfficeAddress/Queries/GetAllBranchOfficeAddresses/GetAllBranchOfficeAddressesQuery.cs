using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;


namespace EBOS.CRM.Application.Features.CRM.BranchOfficeAddress.Queries.GetAllBranchOfficeAddresses;

public record GetAllBranchOfficeAddressesQuery : IRequest<IReadOnlyCollection<BranchOfficeAddressResponse>>;









