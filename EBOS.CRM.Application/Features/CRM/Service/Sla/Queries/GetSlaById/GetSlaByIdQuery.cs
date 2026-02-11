using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Service.Sla.Queries.GetSlaById;

public record GetSlaByIdQuery(long Id) : IRequest<SlaResponse?>;
