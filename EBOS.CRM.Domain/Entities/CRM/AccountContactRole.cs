using EBOS.Core.Primitives;
using EBOS.CRM.Domain.Events;
using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;

namespace EBOS.CRM.Domain.Entities.CRM;

public class AccountContactRole : ErasableEntity, ITenantScopedEntity
{
    private readonly DomainOperationalEventBuffer _operationalEvents = new();

    public long TenantId { get; private set; }
    public long AccountContactId { get; private set; }
    public AccountContact AccountContact { get; private set; } = null!;
    public string RoleCode { get; private set; } = null!;
    public bool IsPrimary { get; private set; }
    public DateTime ValidFrom { get; private set; }
    public DateTime? ValidTo { get; private set; }
    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();
    long ITenantScopedEntity.TenantId { get => TenantId; set => TenantId = value; }

    public IReadOnlyCollection<DomainOperationalEvent> PeekOperationalEvents()
        => _operationalEvents.Peek();

    public IReadOnlyCollection<DomainOperationalEvent> DequeueOperationalEvents()
        => _operationalEvents.Dequeue();

    private AccountContactRole()
    {
    }

    public static AccountContactRole Create(long tenantId, long accountContactId, string roleCode, bool isPrimary,
        DateTime validFrom, DateTime? validTo)
    {
        ValidateIdentity(tenantId, accountContactId, roleCode);

        var entity = new AccountContactRole
        {
            TenantId = tenantId,
            AccountContactId = accountContactId,
            RoleCode = roleCode.Trim().ToUpperInvariant(),
            Erased = false
        };

        entity.Activate(validFrom);
        if (validTo.HasValue)
        {
            entity.Deactivate(validTo.Value);
        }

        entity.SetPrimary(isPrimary);
        return entity;
    }

    public void Update(long tenantId, long accountContactId, string roleCode, bool isPrimary, DateTime validFrom,
        DateTime? validTo)
    {
        ValidateIdentity(tenantId, accountContactId, roleCode);

        TenantId = tenantId;
        AccountContactId = accountContactId;
        RoleCode = roleCode.Trim().ToUpperInvariant();

        Activate(validFrom);
        if (validTo.HasValue)
        {
            Deactivate(validTo.Value);
        }

        SetPrimary(isPrimary);
    }

    public void Activate(DateTime validFrom)
    {
        ValidFrom = validFrom;
        ValidTo = null;
        EmitOperationalEvent(
            "AccountContactRoleChanged",
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["aggregate"] = nameof(AccountContactRole),
                ["action"] = nameof(Activate)
            });
    }

    public void Deactivate(DateTime validTo)
    {
        if (validTo < ValidFrom)
        {
            throw new DomainValidationException("ValidTo cannot be earlier than ValidFrom.", "DOMAIN_VALIDATION_VALID_TO_RANGE");
        }

        ValidTo = validTo;
        IsPrimary = false;
        EmitOperationalEvent(
            "AccountContactRoleChanged",
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["aggregate"] = nameof(AccountContactRole),
                ["action"] = nameof(Deactivate)
            });
    }

    public void SetPrimary(bool isPrimary)
    {
        if (isPrimary && ValidTo.HasValue)
        {
            EmitOperationalEvent(
                "DomainInvariantBreachDetected",
                new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    ["aggregate"] = nameof(AccountContactRole),
                    ["command"] = nameof(SetPrimary),
                    ["invariant"] = "ROLE_PRIMARY_INACTIVE"
                });
            throw new DomainRuleViolationException("Cannot set primary role when role is not active.", "DOMAIN_RULE_VIOLATION_ROLE_PRIMARY_INACTIVE");
        }

        if (IsPrimary == isPrimary)
        {
            EmitOperationalEvent(
                "DomainCommandDeduplicated",
                new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    ["aggregate"] = nameof(AccountContactRole),
                    ["command"] = nameof(SetPrimary),
                    ["isPrimary"] = isPrimary.ToString()
                });
            return;
        }

        IsPrimary = isPrimary;
        EmitOperationalEvent(
            "AccountContactRoleChanged",
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["aggregate"] = nameof(AccountContactRole),
                ["action"] = nameof(SetPrimary),
                ["isPrimary"] = isPrimary.ToString()
            });
    }

    public void ReassignAccountContact(long accountContactId)
    {
        if (accountContactId <= 0)
        {
            throw new DomainValidationException("AccountContactId must be a positive value.", "DOMAIN_VALIDATION_ACCOUNT_CONTACT_ID_POSITIVE");
        }

        AccountContactId = accountContactId;
    }

    private static void ValidateIdentity(long tenantId, long accountContactId, string roleCode)
    {
        if (tenantId <= 0)
        {
            throw new DomainValidationException("TenantId must be a positive value.", "DOMAIN_VALIDATION_TENANT_ID_POSITIVE");
        }

        if (accountContactId <= 0)
        {
            throw new DomainValidationException("AccountContactId must be a positive value.", "DOMAIN_VALIDATION_ACCOUNT_CONTACT_ID_POSITIVE");
        }

        if (string.IsNullOrWhiteSpace(roleCode))
        {
            throw new DomainValidationException("RoleCode is required.", "DOMAIN_VALIDATION_ROLE_CODE_REQUIRED");
        }
    }

    private void EmitOperationalEvent(string eventName, IReadOnlyDictionary<string, string>? evidence = null)
        => _operationalEvents.Emit(eventName, evidence);
}

