using MediatR;
using EBOS.CRM.Contracts.Responses.EBOS;

namespace EBOS.CRM.Application.Features.EBOS.ValidationRules.Queries.GetValidationRuleById;

public record GetValidationRuleByIdQuery(long Id) : IRequest<ValidationRuleResponse>;
