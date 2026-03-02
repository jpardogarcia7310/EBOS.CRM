using EBOS.CRM.Application.Shared.Audit;
using EBOS.CRM.Contracts.Requests.CRM.CustomerMerge;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using EBOS.CRM.Domain.Interfaces.Services.CRM;
using EBOS.CRM.Application.Options;
using MediatR;
using Microsoft.Extensions.Options;

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
    ICurrentUserContext currentUser,
    ICustomer360Metrics metrics,
    IOptions<CustomerMergeOptions> mergeOptions)
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
        if (string.IsNullOrWhiteSpace(mergeRequest.Reason))
        {
            throw new InvalidOperationException("Merge reason is required.");
        }

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
        var options = mergeOptions.Value ?? new CustomerMergeOptions();
        var mergeResolver = new CustomerMergeFieldResolver(currentUser, options);
        var preferWinnerOnTie = options.PreferWinnerOnTie;

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

            winnerUpdated = await MergeGoldenRecordAsync(winner, winnerType, mergeIds, mergeResolver, options, preferWinnerOnTie, cancellationToken);

            var addresses = (await customerAddressRepository.GetByCustomerIdsAsync(mergeRequest.TenantId, customerIds.ToList(), cancellationToken))
                .ToList();

            var preferences = (await customerPreferenceRepository.GetByCustomerIdsAsync(mergeRequest.TenantId, customerIds.ToList(), cancellationToken))
                .ToList();

            var consents = (await customerConsentRepository.GetByCustomerIdsAsync(mergeRequest.TenantId, customerIds.ToList(), cancellationToken))
                .ToList();

            var accountContacts = (await accountContactRepository.GetByCustomerIdsAsync(mergeRequest.TenantId, customerIds.ToList(), cancellationToken))
                .ToList();

            var accountContactIds = accountContacts
                .Select(ac => ac.Id)
                .ToHashSet();

            var contactRoles = (await accountContactRoleRepository.GetByAccountContactIdsAsync(mergeRequest.TenantId, accountContactIds.ToList(), cancellationToken))
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
            metrics.RecordMerge(mergeRequest.TenantId, merged.Count, true);
        }
        catch
        {
            await customerRepository.RollbackAsync(cancellationToken);
            metrics.RecordMerge(mergeRequest.TenantId, 0, false);
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
        IReadOnlyCollection<long> mergeIds, CustomerMergeFieldResolver mergeResolver, CustomerMergeOptions options,
        bool preferWinnerOnTie, CancellationToken cancellationToken)
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

            var winnerEmailContext = CreateFieldContext(winner, "Email", options);
            var candidateEmailContext = CreateFieldContext(candidate, "Email", options);
            var resolvedEmail = mergeResolver.ResolveString(winner.Email, winnerEmailUpdatedAt,
                candidate.Email, candidateUpdatedAt, preferWinnerOnTie, winnerEmailContext, candidateEmailContext);
            if (!string.Equals(resolvedEmail, winner.Email, StringComparison.Ordinal))
            {
                winner.Email = resolvedEmail!;
                winnerEmailUpdatedAt = candidateUpdatedAt;
                updated = true;
            }

            var winnerPhoneContext = CreateFieldContext(winner, "Phone", options);
            var candidatePhoneContext = CreateFieldContext(candidate, "Phone", options);
            var resolvedPhone = mergeResolver.ResolveString(winner.Phone, winnerPhoneUpdatedAt,
                candidate.Phone, candidateUpdatedAt, preferWinnerOnTie, winnerPhoneContext, candidatePhoneContext);
            if (!string.Equals(resolvedPhone, winner.Phone, StringComparison.Ordinal))
            {
                winner.Phone = resolvedPhone!;
                winnerPhoneUpdatedAt = candidateUpdatedAt;
                updated = true;
            }
        }

        if (winnerType == CustomerType.Corporate)
        {
            updated |= await MergeCorporateGoldenRecordAsync(winner.Id, mergeIds, mergeResolver, options, preferWinnerOnTie, cancellationToken);
        }
        else
        {
            updated |= await MergeIndividualGoldenRecordAsync(winner.Id, mergeIds, mergeResolver, options, preferWinnerOnTie, cancellationToken);
        }

        return updated;
    }

    private async Task<bool> MergeCorporateGoldenRecordAsync(long winnerId, IReadOnlyCollection<long> mergeIds,
        CustomerMergeFieldResolver mergeResolver, CustomerMergeOptions options, bool preferWinnerOnTie, CancellationToken cancellationToken)
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

            var winnerLegalContext = CreateFieldContext(winnerCorporate, "LegalName", options);
            var candidateLegalContext = CreateFieldContext(candidate, "LegalName", options);
            var resolvedLegalName = mergeResolver.ResolveString(winnerCorporate.LegalName, legalNameUpdatedAt,
                candidate.LegalName, candidateUpdatedAt, preferWinnerOnTie, winnerLegalContext, candidateLegalContext);
            if (!string.Equals(resolvedLegalName, winnerCorporate.LegalName, StringComparison.Ordinal))
            {
                winnerCorporate.LegalName = resolvedLegalName!;
                legalNameUpdatedAt = candidateUpdatedAt;
                updated = true;
            }

            var winnerTaxContext = CreateFieldContext(winnerCorporate, "TaxIdentification", options);
            var candidateTaxContext = CreateFieldContext(candidate, "TaxIdentification", options);
            var resolvedTaxId = mergeResolver.ResolveString(winnerCorporate.TaxIdentification, taxIdUpdatedAt,
                candidate.TaxIdentification, candidateUpdatedAt, preferWinnerOnTie, winnerTaxContext, candidateTaxContext);
            if (!string.Equals(resolvedTaxId, winnerCorporate.TaxIdentification, StringComparison.Ordinal))
            {
                winnerCorporate.TaxIdentification = resolvedTaxId!;
                taxIdUpdatedAt = candidateUpdatedAt;
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
        CustomerMergeFieldResolver mergeResolver, CustomerMergeOptions options, bool preferWinnerOnTie, CancellationToken cancellationToken)
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

            var winnerFirstContext = CreateFieldContext(winnerIndividual, "FirstName", options);
            var candidateFirstContext = CreateFieldContext(candidate, "FirstName", options);
            var resolvedFirstName = mergeResolver.ResolveString(winnerIndividual.FirstName, firstNameUpdatedAt,
                candidate.FirstName, candidateUpdatedAt, preferWinnerOnTie, winnerFirstContext, candidateFirstContext);
            if (!string.Equals(resolvedFirstName, winnerIndividual.FirstName, StringComparison.Ordinal))
            {
                winnerIndividual.FirstName = resolvedFirstName!;
                firstNameUpdatedAt = candidateUpdatedAt;
                updated = true;
            }

            var winnerLastContext = CreateFieldContext(winnerIndividual, "LastName", options);
            var candidateLastContext = CreateFieldContext(candidate, "LastName", options);
            var resolvedLastName = mergeResolver.ResolveString(winnerIndividual.LastName, lastNameUpdatedAt,
                candidate.LastName, candidateUpdatedAt, preferWinnerOnTie, winnerLastContext, candidateLastContext);
            if (!string.Equals(resolvedLastName, winnerIndividual.LastName, StringComparison.Ordinal))
            {
                winnerIndividual.LastName = resolvedLastName!;
                lastNameUpdatedAt = candidateUpdatedAt;
                updated = true;
            }

            var winnerIdNumberContext = CreateFieldContext(winnerIndividual, "IdentificationNumber", options);
            var candidateIdNumberContext = CreateFieldContext(candidate, "IdentificationNumber", options);
            var resolvedIdNumber = mergeResolver.ResolveString(winnerIndividual.IdentificationNumber, idNumberUpdatedAt,
                candidate.IdentificationNumber, candidateUpdatedAt, preferWinnerOnTie, winnerIdNumberContext, candidateIdNumberContext);
            if (!string.Equals(resolvedIdNumber, winnerIndividual.IdentificationNumber, StringComparison.Ordinal))
            {
                winnerIndividual.IdentificationNumber = resolvedIdNumber!;
                idNumberUpdatedAt = candidateUpdatedAt;
                updated = true;
            }

            var winnerIdTypeContext = CreateFieldContext(winnerIndividual, "IdentificationTypeId", options);
            var candidateIdTypeContext = CreateFieldContext(candidate, "IdentificationTypeId", options);
            var resolvedIdType = mergeResolver.ResolveLong(winnerIndividual.IdentificationTypeId, idTypeUpdatedAt,
                candidate.IdentificationTypeId, candidateUpdatedAt, preferWinnerOnTie, winnerIdTypeContext, candidateIdTypeContext);
            if (resolvedIdType != winnerIndividual.IdentificationTypeId)
            {
                winnerIndividual.IdentificationTypeId = resolvedIdType;
                idTypeUpdatedAt = candidateUpdatedAt;
                updated = true;
            }

            var winnerBirthContext = CreateFieldContext(winnerIndividual, "BirthDate", options);
            var candidateBirthContext = CreateFieldContext(candidate, "BirthDate", options);
            var resolvedBirthDate = mergeResolver.ResolveDate(winnerIndividual.BirthDate, birthDateUpdatedAt,
                candidate.BirthDate, candidateUpdatedAt, preferWinnerOnTie, winnerBirthContext, candidateBirthContext);
            if (resolvedBirthDate != winnerIndividual.BirthDate)
            {
                winnerIndividual.BirthDate = resolvedBirthDate;
                birthDateUpdatedAt = candidateUpdatedAt;
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

    private static CustomerMergeFieldContext CreateFieldContext(Domain.Entities.CRM.Customer customer, string fieldKey,
        CustomerMergeOptions options)
    {
        var channelKey = options.FieldChannelMap.TryGetValue(fieldKey, out var mapped) ? mapped : fieldKey;
        return new CustomerMergeFieldContext(customer.Source, channelKey, customer.Confidentiality);
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
                    winnerPreference.MergeFrom(preference);
                    await customerPreferenceRepository.UpdateAsync(winnerPreference, cancellationToken);
                }

                preference.Erased = true;
                await customerPreferenceRepository.UpdateAsync(preference, cancellationToken);
                continue;
            }

            preference.ReassignCustomer(winnerId);
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
                    existingContact.SetPrimary(true);
                    await accountContactRepository.UpdateAsync(existingContact, cancellationToken);
                }

                contact.SetPrimary(false);
                contact.Erased = true;
                await accountContactRepository.UpdateAsync(contact, cancellationToken);
                continue;
            }

            contact.ReassignCustomers(newCorporateId, newIndividualId);

            if (contact.IsPrimary)
            {
                if (primaryByCorporate.TryGetValue(newCorporateId, out var primaryContactId) &&
                    primaryContactId != contact.Id)
                {
                    contact.SetPrimary(false);
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
        var targetHasPrimary = targetRoles.Any(r => r.IsPrimary && !r.Erased);

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

            if (role.IsPrimary && targetHasPrimary)
            {
                role.SetPrimary(false);
            }
            else if (role.IsPrimary)
            {
                targetHasPrimary = true;
            }

            role.ReassignAccountContact(target.Id);
            await accountContactRoleRepository.UpdateAsync(role, cancellationToken);
            existingCodes.Add(role.RoleCode);
        }
    }
}
