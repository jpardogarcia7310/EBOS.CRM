using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM.Models;
using EBOS.CRM.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace EBOS.CRM.Infrastructure.Repositories.Concrete.CRM;

public class CustomerDedupeRepository(CrmDbContext context, IOptions<CustomerDedupeOptions> options)
    : ICustomerDedupeRepository
{
    private readonly CustomerDedupeOptions _options = options.Value;

    public async Task<IReadOnlyCollection<CustomerDuplicateCandidate>> FindDuplicatesAsync(CustomerDedupeCriteria criteria,
        int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var query = BuildQuery(criteria);
        var safePageNumber = Math.Max(1, pageNumber);
        var safePageSize = Math.Max(1, pageSize);

        var results = await query
            .OrderByDescending(x => x.Score)
            .ThenBy(x => x.CustomerId)
            .Skip((safePageNumber - 1) * safePageSize)
            .Take(safePageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return results.Select(x => new CustomerDuplicateCandidate(
            x.CustomerId,
            BuildReason(x.EmailMatch, x.PhoneMatch, x.PhoneApproxMatch, x.TaxIdMatch, x.IdentificationMatch),
            x.Score)).ToList();
    }

    public async Task<int> CountDuplicatesAsync(CustomerDedupeCriteria criteria, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await BuildQuery(criteria).CountAsync(cancellationToken);
    }

    private IQueryable<DedupeProjection> BuildQuery(CustomerDedupeCriteria criteria)
    {
        var customers = context.Customers.AsNoTracking();
        var corporateCustomers = context.Customers.OfType<CorporateCustomer>().AsNoTracking();
        var individualCustomers = context.Customers.OfType<IndividualCustomer>().AsNoTracking();

        var email = criteria.Email;
        var phone = criteria.Phone;
        var taxId = criteria.TaxId;
        var idNumber = criteria.IdentificationNumber;
        var phoneSuffix = ResolvePhoneSuffix(phone);

        var query = from customer in customers
            where customer.TenantId == criteria.TenantId && !customer.Erased
            join corp in corporateCustomers on customer.Id equals corp.Id into corpJoin
            from corp in corpJoin.DefaultIfEmpty()
            join ind in individualCustomers on customer.Id equals ind.Id into indJoin
            from ind in indJoin.DefaultIfEmpty()
            let emailMatch = !string.IsNullOrWhiteSpace(email) && customer.Email == email
            let phoneMatch = !string.IsNullOrWhiteSpace(phone) && customer.Phone == phone
            let phoneApproxMatch = phoneSuffix != null
                                   && !phoneMatch
                                   && customer.Phone.EndsWith(phoneSuffix)
            let taxIdMatch = corp != null
                             && !string.IsNullOrWhiteSpace(taxId)
                             && corp.TaxIdentification != null
                             && corp.TaxIdentification == taxId
            let identificationMatch = ind != null
                                      && !string.IsNullOrWhiteSpace(idNumber)
                                      && ind.IdentificationNumber != null
                                      && ind.IdentificationNumber == idNumber
            let score = (emailMatch ? _options.EmailWeight : 0)
                        + (phoneMatch ? _options.PhoneWeight : 0)
                        + (phoneApproxMatch ? _options.PhoneApproxWeight : 0)
                        + (taxIdMatch ? _options.TaxIdWeight : 0)
                        + (identificationMatch ? _options.IdentificationNumberWeight : 0)
            where score >= _options.MinScore
            select new DedupeProjection
            {
                CustomerId = customer.Id,
                Score = score,
                EmailMatch = emailMatch,
                PhoneMatch = phoneMatch,
                PhoneApproxMatch = phoneApproxMatch,
                TaxIdMatch = taxIdMatch,
                IdentificationMatch = identificationMatch
            };

        return query;
    }

    private string? ResolvePhoneSuffix(string? phone)
    {
        if (!_options.EnablePhoneSuffixFallback)
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(phone))
        {
            return null;
        }

        if (_options.PhoneSuffixLength <= 0 || phone.Length < _options.PhoneSuffixLength)
        {
            return null;
        }

        return phone[^_options.PhoneSuffixLength..];
    }

    private static string BuildReason(bool emailMatch, bool phoneMatch, bool phoneApproxMatch, bool taxIdMatch,
        bool identificationMatch)
    {
        var reasons = new List<string>(5);
        if (emailMatch) reasons.Add("Email");
        if (phoneMatch) reasons.Add("Phone");
        if (phoneApproxMatch) reasons.Add("PhoneApprox");
        if (taxIdMatch) reasons.Add("TaxId");
        if (identificationMatch) reasons.Add("IdentificationNumber");
        return string.Join(",", reasons);
    }

    private sealed class DedupeProjection
    {
        public long CustomerId { get; init; }
        public int Score { get; init; }
        public bool EmailMatch { get; init; }
        public bool PhoneMatch { get; init; }
        public bool PhoneApproxMatch { get; init; }
        public bool TaxIdMatch { get; init; }
        public bool IdentificationMatch { get; init; }
    }
}
