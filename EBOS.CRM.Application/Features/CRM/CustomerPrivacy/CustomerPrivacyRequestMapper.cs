using EBOS.CRM.Contracts.Responses.CRM;

namespace EBOS.CRM.Application.Features.CRM.CustomerPrivacy;

internal static class CustomerPrivacyRequestMapper
{
    public static CustomerPrivacyRequestResponse ToResponse(this Domain.Entities.CRM.CustomerPrivacyRequest entity)
    {
        return new CustomerPrivacyRequestResponse(
            entity.Id,
            entity.TenantId,
            entity.CustomerId,
            entity.RequestType,
            entity.Status,
            entity.Reason,
            entity.RequestedBy,
            entity.RequestedAt,
            entity.ProcessedBy,
            entity.ProcessedAt,
            entity.FailureCode,
            entity.FailureReason,
            entity.CorrelationId);
    }
}
