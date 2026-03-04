using EBOS.CRM.Contracts.Requests.CRM.Lead;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using EBOS.CRM.Domain.Interfaces.Services.CRM;

namespace EBOS.CRM.Infrastructure.Services.CRM;

public sealed class LeadDebtorCheckService(CrmDbContext context) : ILeadDebtorCheckService
{
    public async Task<LeadDebtorCheckResponse> CheckAsync(LeadDebtorCheckRequest request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var email = Normalize(request.Email);
        var phone = Normalize(request.Phone);
        var companyName = Normalize(request.CompanyName);
        var contactName = Normalize(request.ContactName);
        var (firstName, lastName) = SplitName(contactName);

        var individual = await FindIndividualAsync(request.TenantId, email, phone, firstName, lastName, cancellationToken);
        if (individual is not null)
        {
            return MapDebtor(individual, "Individual");
        }

        var corporate = await FindCorporateAsync(request.TenantId, email, phone, companyName, cancellationToken);
        if (corporate is not null)
        {
            return MapDebtor(corporate, "Corporate");
        }

        return new LeadDebtorCheckResponse(false, null, null, null, null, null, null, null, null, null, null);
    }

    private async Task<IndividualCustomer?> FindIndividualAsync(long tenantId, string? email, string? phone,
        string? firstName, string? lastName, CancellationToken cancellationToken)
    {
        var query = context.Set<IndividualCustomer>()
            .Include(c => c.Status)
            .Include(c => c.CreditAccount)
            .Where(c => c.TenantId == tenantId);

        query = ApplyMatchFilters(query, email, phone);
        if (!string.IsNullOrWhiteSpace(firstName) && !string.IsNullOrWhiteSpace(lastName))
        {
            query = query.Where(c =>
                c.FirstName.ToLower() == firstName &&
                c.LastName.ToLower() == lastName);
        }

        return await query
            .Where(c =>
                c.Status.Description != null &&
                (c.Status.Description.ToLower() == "debtor" ||
                 c.Status.Description.ToLower() == "moroso"))
            .FirstOrDefaultAsync(cancellationToken);
    }

    private async Task<CorporateCustomer?> FindCorporateAsync(long tenantId, string? email, string? phone,
        string? companyName, CancellationToken cancellationToken)
    {
        var query = context.Set<CorporateCustomer>()
            .Include(c => c.Status)
            .Include(c => c.CreditAccount)
            .Where(c => c.TenantId == tenantId);

        query = ApplyMatchFilters(query, email, phone);
        if (!string.IsNullOrWhiteSpace(companyName))
        {
            query = query.Where(c => c.LegalName.ToLower() == companyName);
        }

        return await query
            .Where(c =>
                c.Status.Description != null &&
                (c.Status.Description.ToLower() == "debtor" ||
                 c.Status.Description.ToLower() == "moroso"))
            .FirstOrDefaultAsync(cancellationToken);
    }

    private static IQueryable<T> ApplyMatchFilters<T>(IQueryable<T> query, string? email, string? phone)
        where T : Customer
    {
        if (!string.IsNullOrWhiteSpace(email))
        {
            query = query.Where(c => c.Email.ToLower() == email);
        }

        if (!string.IsNullOrWhiteSpace(phone))
        {
            query = query.Where(c => c.Phone.ToLower() == phone);
        }

        return query;
    }

    private static LeadDebtorCheckResponse MapDebtor(Customer customer, string customerType)
    {
        var name = customer switch
        {
            IndividualCustomer individual => $"{individual.FirstName} {individual.LastName}".Trim(),
            CorporateCustomer corporate => corporate.LegalName,
            _ => null
        };

        var debtorSince = customer.UpdatedAt ?? customer.CreatedAt;
        var debtAmount = customer.CreditAccount?.UsedAmount ?? 0m;

        return new LeadDebtorCheckResponse(
            true,
            customer.Id,
            customerType,
            customer.Code,
            name,
            customer.Email,
            customer.Phone,
            customer.StatusId,
            customer.Status.Description,
            debtorSince,
            debtAmount);
    }

    private static string? Normalize(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToLowerInvariant();

    private static (string? firstName, string? lastName) SplitName(string? contactName)
    {
        if (string.IsNullOrWhiteSpace(contactName))
        {
            return (null, null);
        }

        var parts = contactName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 2)
        {
            return (null, null);
        }

        return (parts[0].ToLowerInvariant(), parts[^1].ToLowerInvariant());
    }
}