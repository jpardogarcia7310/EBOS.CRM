using EBOS.CRM.Application.Shared.Audit;
using EBOS.CRM.Contracts.Requests.CRM.CustomerMerge;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CustomerMerge.Commands.MergeCustomers;

public class MergeCustomersCommandHandler(
    ICustomerRepository customerRepository,
    ICorporateCustomerRepository corporateCustomerRepository,
    IIndividualCustomerRepository individualCustomerRepository,
    ICustomerAddressRepository customerAddressRepository,
    ICustomerPreferenceRepository customerPreferenceRepository,
    ICustomerConsentRepository customerConsentRepository,
    IAccountContactRepository accountContactRepository,
    IAccountContactRoleRepository accountContactRoleRepository,
    IAuditService auditService,
    ICurrentUserContext currentUser)
    : IRequestHandler<MergeCustomersCommand, CustomerMergeResultResponse>
{
    private enum CustomerType
    {
        Corporate,
        Individual
    }

    public async Task<CustomerMergeResultResponse> Handle(MergeCustomersCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var mergeRequest = request.Request ?? throw new ArgumentNullException(nameof(request.Request));

        var winner = await customerRepository.GetByIdAsync(mergeRequest.WinnerCustomerId, cancellationToken)
            ?? throw new InvalidOperationException("Winner customer not found.");
        if (winner.TenantId != mergeRequest.TenantId)
        {
            throw new InvalidOperationException("Winner customer tenant mismatch.");
        }

        var winnerType = await ResolveCustomerTypeAsync(winner.Id, cancellationToken);

        var mergeIds = mergeRequest.MergeCustomerIds
            .Where(id => id != mergeRequest.WinnerCustomerId)
            .Distinct()
            .ToList();

        var merged = new List<long>();
        var customerIds = new HashSet<long>(mergeIds) { winner.Id };
        var mergeTypeById = new Dictionary<long, CustomerType>();

        var winnerOldValues = AuditSerialization.Serialize(winner);
        var winnerUpdated = false;

        await customerRepository.BeginTransactionAsync(cancellationToken);

        try
        {
            foreach (var mergeId in mergeIds)
            {
                var type = await ResolveCustomerTypeAsync(mergeId, cancellationToken);
                if (type != winnerType)
                {
                    throw new InvalidOperationException("Customer type mismatch in merge list.");
                }

                mergeTypeById[mergeId] = type;
            }

            winnerUpdated = await MergeGoldenRecordAsync(winner, winnerType, mergeIds, cancellationToken);

            var addresses = (await customerAddressRepository.GetAllAsync(cancellationToken))
                .Where(a => customerIds.Contains(a.CustomerId))
                .ToList();

            var preferences = (await customerPreferenceRepository.GetAllAsync(cancellationToken))
                .Where(p => customerIds.Contains(p.CustomerId))
                .ToList();

            var consents = (await customerConsentRepository.GetAllAsync(cancellationToken))
                .Where(c => customerIds.Contains(c.CustomerId))
                .ToList();

            var accountContacts = (await accountContactRepository.GetAllAsync(cancellationToken))
                .Where(ac => customerIds.Contains(ac.CorporateCustomerId) || customerIds.Contains(ac.IndividualCustomerId))
                .ToList();

            var accountContactIds = accountContacts
                .Select(ac => ac.Id)
                .ToHashSet();

            var contactRoles = (await accountContactRoleRepository.GetAllAsync(cancellationToken))
                .Where(role => accountContactIds.Contains(role.AccountContactId))
                .ToList();

            await MergeCustomerAddressesAsync(winner.Id, mergeIds, addresses, cancellationToken);
            await MergeCustomerPreferencesAsync(winner.Id, mergeIds, preferences, cancellationToken);
            await MergeCustomerConsentsAsync(winner.Id, mergeIds, consents, cancellationToken);
            await MergeAccountContactsAsync(winner.Id, winnerType, mergeIds, accountContacts, contactRoles, cancellationToken);

            foreach (var mergeId in mergeIds)
            {
                var entity = await customerRepository.GetByIdAsync(mergeId, cancellationToken);
                if (entity is null)
                {
                    continue;
                }

                if (entity.TenantId != mergeRequest.TenantId)
                {
                    throw new InvalidOperationException("Customer tenant mismatch in merge list.");
                }

                entity.Erased = true;
                await customerRepository.UpdateAsync(entity, cancellationToken);
                merged.Add(entity.Id);
            }

            if (winnerUpdated)
            {
                winner.UpdatedAt = DateTime.UtcNow;
                winner.UpdatedBy = currentUser.UserId;
                await customerRepository.UpdateAsync(winner, cancellationToken);
            }

            await customerRepository.SaveChangesAsync(cancellationToken);

            var auditRequest = new AuditInsertRequest(
                UserId: currentUser.UserId,
                TimeStamp: DateTimeOffset.UtcNow,
                Action: AuditActions.Update,
                Entity: nameof(Domain.Entities.CRM.Customer),
                RegisterId: winner.Id,
                OldValues: winnerOldValues,
                NewValues: AuditSerialization.Serialize(winner),
                CorrelationId: currentUser.CorrelationId);

            await auditService.InsertAuditAsync(auditRequest, cancellationToken);
            await customerRepository.CommitAsync(cancellationToken);
        }
        catch
        {
            await customerRepository.RollbackAsync(cancellationToken);
            throw;
        }

        return new CustomerMergeResultResponse(winner.Id, merged, "Merged");
    }

    private async Task<CustomerType> ResolveCustomerTypeAsync(long customerId, CancellationToken cancellationToken)
    {
        var corporate = await corporateCustomerRepository.GetByIdAsync(customerId, cancellationToken);
        if (corporate is not null)
        {
            return CustomerType.Corporate;
        }

        var individual = await individualCustomerRepository.GetByIdAsync(customerId, cancellationToken);
        if (individual is not null)
        {
            return CustomerType.Individual;
        }

        throw new InvalidOperationException("Customer type could not be resolved.");
    }

    private async Task MergeCustomerAddressesAsync(long winnerId, IReadOnlyCollection<long> mergeIds,
        IReadOnlyCollection<Domain.Entities.CRM.CustomerAddress> addresses, CancellationToken cancellationToken)
    {
        var winnerHasPrimary = addresses.Any(a => a.CustomerId == winnerId && a.IsPrimary && a.IsCurrent);
        long? primaryCandidateId = null;

        if (!winnerHasPrimary)
        {
            var candidate = addresses
                .Where(a => mergeIds.Contains(a.CustomerId) && a.IsPrimary && a.IsCurrent)
                .OrderByDescending(a => a.ValidFrom)
                .ThenByDescending(a => a.Id)
                .FirstOrDefault();

            primaryCandidateId = candidate?.Id;
        }

        foreach (var address in addresses.Where(a => mergeIds.Contains(a.CustomerId)))
        {
            var updated = false;

            if (address.CustomerId != winnerId)
            {
                address.CustomerId = winnerId;
                updated = true;
            }

            if (address.IsPrimary && address.IsCurrent)
            {
                if (winnerHasPrimary)
                {
                    address.IsPrimary = false;
                    updated = true;
                }
                else if (primaryCandidateId.HasValue && address.Id == primaryCandidateId.Value)
                {
                    winnerHasPrimary = true;
                }
                else
                {
                    address.IsPrimary = false;
                    updated = true;
                }
            }

            if (updated)
            {
                await customerAddressRepository.UpdateAsync(address, cancellationToken);
            }
        }
    }

    private async Task<bool> MergeGoldenRecordAsync(Domain.Entities.CRM.Customer winner, CustomerType winnerType,
        IReadOnlyCollection<long> mergeIds, CancellationToken cancellationToken)
    {
        var updated = false;

        var winnerBaseUpdatedAt = GetEffectiveUpdatedAt(winner);
        var winnerEmailUpdatedAt = winnerBaseUpdatedAt;
        var winnerPhoneUpdatedAt = winnerBaseUpdatedAt;

        foreach (var mergeId in mergeIds)
        {
            var candidate = await customerRepository.GetByIdAsync(mergeId, cancellationToken);
            if (candidate is null || candidate.Erased)
            {
                continue;
            }

            var candidateUpdatedAt = GetEffectiveUpdatedAt(candidate);

            if (TryPromoteString(winner.Email, winnerEmailUpdatedAt, candidate.Email, candidateUpdatedAt,
                    out var mergedEmail, out var mergedEmailUpdatedAt))
            {
                winner.Email = mergedEmail;
                winnerEmailUpdatedAt = mergedEmailUpdatedAt;
                updated = true;
            }

            if (TryPromoteString(winner.Phone, winnerPhoneUpdatedAt, candidate.Phone, candidateUpdatedAt,
                    out var mergedPhone, out var mergedPhoneUpdatedAt))
            {
                winner.Phone = mergedPhone;
                winnerPhoneUpdatedAt = mergedPhoneUpdatedAt;
                updated = true;
            }
        }

        if (winnerType == CustomerType.Corporate)
        {
            updated |= await MergeCorporateGoldenRecordAsync(winner.Id, mergeIds, cancellationToken);
        }
        else
        {
            updated |= await MergeIndividualGoldenRecordAsync(winner.Id, mergeIds, cancellationToken);
        }

        return updated;
    }

    private async Task<bool> MergeCorporateGoldenRecordAsync(long winnerId, IReadOnlyCollection<long> mergeIds,
        CancellationToken cancellationToken)
    {
        var updated = false;
        var winnerCorporate = await corporateCustomerRepository.GetByIdAsync(winnerId, cancellationToken);
        if (winnerCorporate is null)
        {
            return false;
        }

        var winnerUpdatedAt = GetEffectiveUpdatedAt(winnerCorporate);
        var legalNameUpdatedAt = winnerUpdatedAt;
        var taxIdUpdatedAt = winnerUpdatedAt;

        foreach (var mergeId in mergeIds)
        {
            var candidate = await corporateCustomerRepository.GetByIdAsync(mergeId, cancellationToken);
            if (candidate is null || candidate.Erased)
            {
                continue;
            }

            var candidateUpdatedAt = GetEffectiveUpdatedAt(candidate);

            if (TryPromoteString(winnerCorporate.LegalName, legalNameUpdatedAt, candidate.LegalName,
                    candidateUpdatedAt, out var mergedLegalName, out var mergedLegalNameUpdatedAt))
            {
                winnerCorporate.LegalName = mergedLegalName;
                legalNameUpdatedAt = mergedLegalNameUpdatedAt;
                updated = true;
            }

            if (TryPromoteString(winnerCorporate.TaxIdentification, taxIdUpdatedAt, candidate.TaxIdentification,
                    candidateUpdatedAt, out var mergedTaxId, out var mergedTaxIdUpdatedAt))
            {
                winnerCorporate.TaxIdentification = mergedTaxId;
                taxIdUpdatedAt = mergedTaxIdUpdatedAt;
                updated = true;
            }
        }

        if (updated)
        {
            await corporateCustomerRepository.UpdateAsync(winnerCorporate, cancellationToken);
        }

        return updated;
    }

    private async Task<bool> MergeIndividualGoldenRecordAsync(long winnerId, IReadOnlyCollection<long> mergeIds,
        CancellationToken cancellationToken)
    {
        var updated = false;
        var winnerIndividual = await individualCustomerRepository.GetByIdAsync(winnerId, cancellationToken);
        if (winnerIndividual is null)
        {
            return false;
        }

        var winnerUpdatedAt = GetEffectiveUpdatedAt(winnerIndividual);
        var firstNameUpdatedAt = winnerUpdatedAt;
        var lastNameUpdatedAt = winnerUpdatedAt;
        var idNumberUpdatedAt = winnerUpdatedAt;
        var idTypeUpdatedAt = winnerUpdatedAt;
        var birthDateUpdatedAt = winnerUpdatedAt;

        foreach (var mergeId in mergeIds)
        {
            var candidate = await individualCustomerRepository.GetByIdAsync(mergeId, cancellationToken);
            if (candidate is null || candidate.Erased)
            {
                continue;
            }

            var candidateUpdatedAt = GetEffectiveUpdatedAt(candidate);

            if (TryPromoteString(winnerIndividual.FirstName, firstNameUpdatedAt, candidate.FirstName,
                    candidateUpdatedAt, out var mergedFirstName, out var mergedFirstNameUpdatedAt))
            {
                winnerIndividual.FirstName = mergedFirstName;
                firstNameUpdatedAt = mergedFirstNameUpdatedAt;
                updated = true;
            }

            if (TryPromoteString(winnerIndividual.LastName, lastNameUpdatedAt, candidate.LastName,
                    candidateUpdatedAt, out var mergedLastName, out var mergedLastNameUpdatedAt))
            {
                winnerIndividual.LastName = mergedLastName;
                lastNameUpdatedAt = mergedLastNameUpdatedAt;
                updated = true;
            }

            if (TryPromoteString(winnerIndividual.IdentificationNumber, idNumberUpdatedAt,
                    candidate.IdentificationNumber, candidateUpdatedAt,
                    out var mergedIdNumber, out var mergedIdNumberUpdatedAt))
            {
                winnerIndividual.IdentificationNumber = mergedIdNumber;
                idNumberUpdatedAt = mergedIdNumberUpdatedAt;
                updated = true;
            }

            if (TryPromoteId(winnerIndividual.IdentificationTypeId, idTypeUpdatedAt,
                    candidate.IdentificationTypeId, candidateUpdatedAt,
                    out var mergedIdType, out var mergedIdTypeUpdatedAt))
            {
                winnerIndividual.IdentificationTypeId = mergedIdType;
                idTypeUpdatedAt = mergedIdTypeUpdatedAt;
                updated = true;
            }

            if (TryPromoteDate(winnerIndividual.BirthDate, birthDateUpdatedAt, candidate.BirthDate,
                    candidateUpdatedAt, out var mergedBirthDate, out var mergedBirthDateUpdatedAt))
            {
                winnerIndividual.BirthDate = mergedBirthDate;
                birthDateUpdatedAt = mergedBirthDateUpdatedAt;
                updated = true;
            }
        }

        if (updated)
        {
            await individualCustomerRepository.UpdateAsync(winnerIndividual, cancellationToken);
        }

        return updated;
    }

    private static DateTime GetEffectiveUpdatedAt(Domain.Entities.CRM.Customer customer)
        => customer.UpdatedAt ?? customer.CreatedAt;

    private static bool TryPromoteString(string? current, DateTime currentUpdatedAt,
        string? candidate, DateTime candidateUpdatedAt, out string merged, out DateTime mergedUpdatedAt)
    {
        if (string.IsNullOrWhiteSpace(candidate))
        {
            merged = current ?? string.Empty;
            mergedUpdatedAt = currentUpdatedAt;
            return false;
        }

        if (string.IsNullOrWhiteSpace(current))
        {
            merged = candidate!;
            mergedUpdatedAt = candidateUpdatedAt;
            return true;
        }

        if (candidateUpdatedAt > currentUpdatedAt)
        {
            merged = candidate!;
            mergedUpdatedAt = candidateUpdatedAt;
            return true;
        }

        merged = current;
        mergedUpdatedAt = currentUpdatedAt;
        return false;
    }

    private static bool TryPromoteId(long current, DateTime currentUpdatedAt,
        long candidate, DateTime candidateUpdatedAt, out long merged, out DateTime mergedUpdatedAt)
    {
        if (candidate <= 0)
        {
            merged = current;
            mergedUpdatedAt = currentUpdatedAt;
            return false;
        }

        if (current <= 0 || candidateUpdatedAt > currentUpdatedAt)
        {
            merged = candidate;
            mergedUpdatedAt = candidateUpdatedAt;
            return true;
        }

        merged = current;
        mergedUpdatedAt = currentUpdatedAt;
        return false;
    }

    private static bool TryPromoteDate(DateTime current, DateTime currentUpdatedAt,
        DateTime candidate, DateTime candidateUpdatedAt, out DateTime merged, out DateTime mergedUpdatedAt)
    {
        if (candidate == default)
        {
            merged = current;
            mergedUpdatedAt = currentUpdatedAt;
            return false;
        }

        if (current == default || candidateUpdatedAt > currentUpdatedAt)
        {
            merged = candidate;
            mergedUpdatedAt = candidateUpdatedAt;
            return true;
        }

        merged = current;
        mergedUpdatedAt = currentUpdatedAt;
        return false;
    }

    private async Task MergeCustomerPreferencesAsync(long winnerId, IReadOnlyCollection<long> mergeIds,
        IReadOnlyCollection<Domain.Entities.CRM.CustomerPreference> preferences, CancellationToken cancellationToken)
    {
        var winnerPreferences = preferences
            .Where(p => p.CustomerId == winnerId)
            .ToDictionary(p => p.ChannelId, p => p);

        foreach (var preference in preferences.Where(p => mergeIds.Contains(p.CustomerId)))
        {
            if (winnerPreferences.TryGetValue(preference.ChannelId, out var winnerPreference))
            {
                if (preference.UpdatedAt > winnerPreference.UpdatedAt)
                {
                    winnerPreference.Preferred = preference.Preferred;
                    winnerPreference.UpdatedAt = preference.UpdatedAt;
                    winnerPreference.UpdatedBy = preference.UpdatedBy;
                    await customerPreferenceRepository.UpdateAsync(winnerPreference, cancellationToken);
                }

                preference.Erased = true;
                await customerPreferenceRepository.UpdateAsync(preference, cancellationToken);
                continue;
            }

            preference.CustomerId = winnerId;
            await customerPreferenceRepository.UpdateAsync(preference, cancellationToken);
            winnerPreferences[preference.ChannelId] = preference;
        }
    }

    private async Task MergeCustomerConsentsAsync(long winnerId, IReadOnlyCollection<long> mergeIds,
        IReadOnlyCollection<Domain.Entities.CRM.CustomerConsent> consents, CancellationToken cancellationToken)
    {
        foreach (var consent in consents.Where(c => mergeIds.Contains(c.CustomerId)))
        {
            consent.AssignCustomer(winnerId);
            await customerConsentRepository.UpdateAsync(consent, cancellationToken);
        }
    }

    private async Task MergeAccountContactsAsync(long winnerId, CustomerType winnerType,
        IReadOnlyCollection<long> mergeIds,
        IReadOnlyCollection<Domain.Entities.CRM.AccountContact> accountContacts,
        IReadOnlyCollection<Domain.Entities.CRM.AccountContactRole> contactRoles,
        CancellationToken cancellationToken)
    {
        var mergeIdSet = new HashSet<long>(mergeIds);
        var contactRolesByContactId = contactRoles
            .GroupBy(r => r.AccountContactId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var contactMap = accountContacts
            .Where(ac => !ac.Erased)
            .GroupBy(ac => (ac.CorporateCustomerId, ac.IndividualCustomerId))
            .ToDictionary(g => g.Key, g => g.First());

        var primaryByCorporate = accountContacts
            .Where(ac => ac.IsPrimary && !ac.Erased)
            .GroupBy(ac => ac.CorporateCustomerId)
            .ToDictionary(g => g.Key, g => g.First().Id);

        foreach (var contact in accountContacts.Where(ac =>
                     mergeIdSet.Contains(winnerType == CustomerType.Corporate ? ac.CorporateCustomerId : ac.IndividualCustomerId)))
        {
            var oldKey = (contact.CorporateCustomerId, contact.IndividualCustomerId);
            var newCorporateId = contact.CorporateCustomerId;
            var newIndividualId = contact.IndividualCustomerId;

            if (winnerType == CustomerType.Corporate)
            {
                if (!mergeIdSet.Contains(contact.CorporateCustomerId))
                {
                    continue;
                }

                newCorporateId = winnerId;
            }
            else
            {
                if (!mergeIdSet.Contains(contact.IndividualCustomerId))
                {
                    continue;
                }

                newIndividualId = winnerId;
            }

            var newKey = (newCorporateId, newIndividualId);
            if (contactMap.TryGetValue(newKey, out var existingContact) && existingContact.Id != contact.Id)
            {
                await MergeContactRolesAsync(existingContact, contact, contactRolesByContactId, cancellationToken);

                if (contact.IsPrimary && !existingContact.IsPrimary)
                {
                    existingContact.IsPrimary = true;
                    await accountContactRepository.UpdateAsync(existingContact, cancellationToken);
                }

                contact.IsPrimary = false;
                contact.Erased = true;
                await accountContactRepository.UpdateAsync(contact, cancellationToken);
                continue;
            }

            contact.CorporateCustomerId = newCorporateId;
            contact.IndividualCustomerId = newIndividualId;

            if (contact.IsPrimary)
            {
                if (primaryByCorporate.TryGetValue(newCorporateId, out var primaryContactId) &&
                    primaryContactId != contact.Id)
                {
                    contact.IsPrimary = false;
                }
                else
                {
                    primaryByCorporate[newCorporateId] = contact.Id;
                }
            }

            await accountContactRepository.UpdateAsync(contact, cancellationToken);

            if (!oldKey.Equals(newKey))
            {
                contactMap.Remove(oldKey);
                contactMap[newKey] = contact;
            }
        }
    }

    private async Task MergeContactRolesAsync(Domain.Entities.CRM.AccountContact target,
        Domain.Entities.CRM.AccountContact source,
        IReadOnlyDictionary<long, List<Domain.Entities.CRM.AccountContactRole>> rolesByContactId,
        CancellationToken cancellationToken)
    {
        var targetRoles = rolesByContactId.TryGetValue(target.Id, out var existingRoles)
            ? existingRoles
            : new List<Domain.Entities.CRM.AccountContactRole>();
        var existingCodes = new HashSet<string>(targetRoles
            .Where(r => !r.Erased)
            .Select(r => r.RoleCode), StringComparer.OrdinalIgnoreCase);

        if (!rolesByContactId.TryGetValue(source.Id, out var sourceRoles))
        {
            return;
        }

        foreach (var role in sourceRoles)
        {
            if (existingCodes.Contains(role.RoleCode))
            {
                role.Erased = true;
                await accountContactRoleRepository.UpdateAsync(role, cancellationToken);
                continue;
            }

            role.AccountContactId = target.Id;
            await accountContactRoleRepository.UpdateAsync(role, cancellationToken);
            existingCodes.Add(role.RoleCode);
        }
    }
}
