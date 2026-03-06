using EBOS.Core.Primitives;
using EBOS.CRM.Domain.Events;
using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;

namespace EBOS.CRM.Domain.Entities.CRM;

public class AccountContact : ErasableEntity, ITenantScopedEntity
{
    private readonly DomainOperationalEventBuffer _operationalEvents = new();

    public long TenantId { get; private set; }
    public long CorporateCustomerId { get; private set; }
    public CorporateCustomer CorporateCustomer { get; private set; } = null!;
    public long IndividualCustomerId { get; private set; }
    public IndividualCustomer IndividualCustomer { get; private set; } = null!;
    public bool IsPrimary { get; private set; }
    public DateTime StartAt { get; private set; }
    public DateTime? EndAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public long CreatedBy { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public long? UpdatedBy { get; private set; }
    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

    public ICollection<AccountContactRole> Roles { get; private set; } = new List<AccountContactRole>();

    public IReadOnlyCollection<DomainOperationalEvent> PeekOperationalEvents()
        => _operationalEvents.Peek();

    public IReadOnlyCollection<DomainOperationalEvent> DequeueOperationalEvents()
        => _operationalEvents.Dequeue();

    long ITenantScopedEntity.TenantId
    {
        get => TenantId;
        set => TenantId = value;
    }

    private AccountContact()
    {
    }

    public static AccountContact Create(long tenantId, long corporateCustomerId, long individualCustomerId,
        bool isPrimary, DateTime startAt, DateTime? endAt, long createdBy, DateTime? createdAt = null)
    {
        if (tenantId <= 0)
        {
            throw new DomainValidationException("TenantId must be a positive value.", "DOMAIN_VALIDATION_TENANT_ID_POSITIVE");
        }

        if (createdBy <= 0)
        {
            throw new DomainValidationException("CreatedBy must be a positive value.", "DOMAIN_VALIDATION_CREATED_BY_POSITIVE");
        }

        var entity = new AccountContact
        {
            TenantId = tenantId,
            CreatedBy = createdBy,
            CreatedAt = createdAt ?? DateTime.UtcNow
        };

        entity.Assign(corporateCustomerId, individualCustomerId, startAt);
        if (endAt.HasValue)
        {
            entity.Unassign(endAt.Value);
        }

        entity.SetPrimary(isPrimary);
        return entity;
    }

    public void Assign(long corporateCustomerId, long individualCustomerId, DateTime startAt)
    {
        if (corporateCustomerId <= 0)
        {
            throw new DomainValidationException("CorporateCustomerId must be a positive value.", "DOMAIN_VALIDATION_CORPORATE_CUSTOMER_ID_POSITIVE");
        }

        if (individualCustomerId <= 0)
        {
            throw new DomainValidationException("IndividualCustomerId must be a positive value.", "DOMAIN_VALIDATION_INDIVIDUAL_CUSTOMER_ID_POSITIVE");
        }

        CorporateCustomerId = corporateCustomerId;
        IndividualCustomerId = individualCustomerId;
        StartAt = startAt;
        EndAt = null;
        EmitOperationalEvent(
            "AccountContactAssigned",
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["aggregate"] = nameof(AccountContact),
                ["corporateCustomerId"] = CorporateCustomerId.ToString(),
                ["individualCustomerId"] = IndividualCustomerId.ToString()
            });
    }

    public void ReassignCustomers(long corporateCustomerId, long individualCustomerId)
    {
        if (corporateCustomerId <= 0 || individualCustomerId <= 0)
        {
            throw new DomainValidationException("Customer ids must be positive values.", "DOMAIN_VALIDATION_CUSTOMER_IDS_POSITIVE");
        }

        if (EndAt.HasValue)
        {
            EmitOperationalEvent(
                "DomainInvariantBreachDetected",
                new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    ["aggregate"] = nameof(AccountContact),
                    ["command"] = nameof(ReassignCustomers),
                    ["invariant"] = "CONTACT_REASSIGN_UNASSIGNED"
                });
            throw new DomainRuleViolationException("Cannot reassign an unassigned account contact. Assign it first.", "DOMAIN_RULE_VIOLATION_CONTACT_REASSIGN_UNASSIGNED");
        }

        CorporateCustomerId = corporateCustomerId;
        IndividualCustomerId = individualCustomerId;
    }

    public void Unassign(DateTime endAt)
    {
        if (endAt < StartAt)
        {
            throw new DomainValidationException("EndAt cannot be earlier than StartAt.", "DOMAIN_VALIDATION_END_AT_RANGE");
        }

        EndAt = endAt;
        IsPrimary = false;
    }

    public void SetPrimary(bool isPrimary)
    {
        if (isPrimary && EndAt.HasValue)
        {
            EmitOperationalEvent(
                "DomainInvariantBreachDetected",
                new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    ["aggregate"] = nameof(AccountContact),
                    ["command"] = nameof(SetPrimary),
                    ["invariant"] = "CONTACT_PRIMARY_UNASSIGNED"
                });
            throw new DomainRuleViolationException("Cannot set as primary when account contact is unassigned.", "DOMAIN_RULE_VIOLATION_CONTACT_PRIMARY_UNASSIGNED");
        }

        if (IsPrimary == isPrimary)
        {
            EmitOperationalEvent(
                "DomainCommandDeduplicated",
                new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    ["aggregate"] = nameof(AccountContact),
                    ["command"] = nameof(SetPrimary),
                    ["isPrimary"] = isPrimary.ToString()
                });
            return;
        }

        IsPrimary = isPrimary;
    }

    public void Touch(long updatedBy, DateTime? updatedAt = null)
    {
        if (updatedBy <= 0)
        {
            throw new DomainValidationException("UpdatedBy must be a positive value.", "DOMAIN_VALIDATION_UPDATED_BY_POSITIVE");
        }

        UpdatedBy = updatedBy;
        UpdatedAt = updatedAt ?? DateTime.UtcNow;
    }

    public void SetPrimaryRole(long accountContactRoleId)
    {
        if (accountContactRoleId <= 0)
        {
            throw new DomainValidationException("AccountContactRoleId must be a positive value.", "DOMAIN_VALIDATION_ACCOUNT_CONTACT_ROLE_ID_POSITIVE");
        }

        var target = Roles.FirstOrDefault(r => r.Id == accountContactRoleId);
        if (target is null)
        {
            throw new DomainConflictException("AccountContactRole not found.", "DOMAIN_CONFLICT_ACCOUNT_CONTACT_ROLE_NOT_FOUND");
        }

        foreach (var role in Roles)
        {
            role.SetPrimary(role.Id == accountContactRoleId);
        }
    }

    private void EmitOperationalEvent(string eventName, IReadOnlyDictionary<string, string>? evidence = null)
        => _operationalEvents.Emit(eventName, evidence);
}
