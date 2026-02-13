using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM.Models;
using EBOS.CRM.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace EBOS.CRM.Infrastructure.Repositories.Concrete.CRM;

public class CustomerDedupeRepository(CrmDbContext context, IOptions<CustomerDedupeOptions> options)
    : ICustomerDedupeRepository
{
    private readonly CrmDbContext _context = context;
    private readonly CustomerDedupeOptions _options = options.Value ?? new CustomerDedupeOptions();

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
            BuildReason(x.EmailMatch, x.PhoneMatch, x.TaxIdMatch, x.IdentificationMatch),
            x.Score)).ToList();
    }

    public async Task<int> CountDuplicatesAsync(CustomerDedupeCriteria criteria, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await BuildQuery(criteria).CountAsync(cancellationToken);
    }

    private IQueryable<DedupeProjection> BuildQuery(CustomerDedupeCriteria criteria)
    {
        var customers = _context.Customers.AsNoTracking();
        var corporateCustomers = _context.Customers.OfType<CorporateCustomer>().AsNoTracking();
        var individualCustomers = _context.Customers.OfType<IndividualCustomer>().AsNoTracking();

        var email = criteria.Email;
        var phone = criteria.Phone;
        var taxId = criteria.TaxId;
        var idNumber = criteria.IdentificationNumber;

        var query = from customer in customers
            where customer.TenantId == criteria.TenantId
            join corp in corporateCustomers on customer.Id equals corp.Id into corpJoin
            from corp in corpJoin.DefaultIfEmpty()
            join ind in individualCustomers on customer.Id equals ind.Id into indJoin
            from ind in indJoin.DefaultIfEmpty()
            let emailMatch = !string.IsNullOrWhiteSpace(email) && customer.Email.ToLower() == email
            let phoneMatch = !string.IsNullOrWhiteSpace(phone) && customer.Phone == phone
            let taxIdMatch = corp != null && !string.IsNullOrWhiteSpace(taxId) && corp.TaxIdentification.ToUpper() == taxId
            let identificationMatch = ind != null && !string.IsNullOrWhiteSpace(idNumber) && ind.IdentificationNumber != null &&
                                      ind.IdentificationNumber.ToUpper() == idNumber
            let score = (emailMatch ? _options.EmailWeight : 0)
                        + (phoneMatch ? _options.PhoneWeight : 0)
                        + (taxIdMatch ? _options.TaxIdWeight : 0)
                        + (identificationMatch ? _options.IdentificationNumberWeight : 0)
            where score > 0
            select new DedupeProjection(
                customer.Id,
                score,
                emailMatch,
                phoneMatch,
                taxIdMatch,
                identificationMatch);

        return query;
    }

    private static string BuildReason(bool emailMatch, bool phoneMatch, bool taxIdMatch, bool identificationMatch)
    {
        var reasons = new List<string>(4);
        if (emailMatch) reasons.Add("Email");
        if (phoneMatch) reasons.Add("Phone");
        if (taxIdMatch) reasons.Add("TaxId");
        if (identificationMatch) reasons.Add("IdentificationNumber");
        return string.Join(",", reasons);
    }

    private sealed record DedupeProjection(
        long CustomerId,
        int Score,
        bool EmailMatch,
        bool PhoneMatch,
        bool TaxIdMatch,
        bool IdentificationMatch);
}
