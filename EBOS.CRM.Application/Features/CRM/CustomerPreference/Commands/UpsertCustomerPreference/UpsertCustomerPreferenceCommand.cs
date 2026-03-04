using EBOS.CRM.Contracts.Requests.CRM.CustomerPreference;
using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CustomerPreference.Commands.UpsertCustomerPreference;

public record UpsertCustomerPreferenceCommand(UpsertCustomerPreferenceRequest PreferenceRequest)
    : IRequest<CustomerPreferenceResponse>;
