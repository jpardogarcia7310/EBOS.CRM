using EBOS.CRM.Contracts.Responses.Common;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CustomerMerge.Queries.FindCustomerDuplicates;

public class FindCustomerDuplicatesQueryHandler(ICustomerRepository customerRepository,
    ICorporateCustomerRepository corporateCustomerRepository,
    IIndividualCustomerRepository individualCustomerRepository)
    : IRequestHandler<FindCustomerDuplicatesQuery, PagedResult<CustomerDuplicateCandidateResponse>>
{
    public async Task<PagedResult<CustomerDuplicateCandidateResponse>> Handle(FindCustomerDuplicatesQuery request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var criteria = request.Request ?? throw new ArgumentNullException(nameof(request.Request));

        var customers = await customerRepository.GetAllAsync(cancellationToken);
        var corporateCustomers = await corporateCustomerRepository.GetAllAsync(cancellationToken);
        var individualCustomers = await individualCustomerRepository.GetAllAsync(cancellationToken);

        var candidates = new List<CustomerDuplicateCandidateResponse>();

        foreach (var customer in customers.Where(x => x.TenantId == criteria.TenantId))
        {
            var score = 0;
            var reasons = new List<string>();

            if (!string.IsNullOrWhiteSpace(criteria.Email) &&
                string.Equals(customer.Email, criteria.Email, StringComparison.OrdinalIgnoreCase))
            {
                score += 50;
                reasons.Add("Email");
            }

            if (!string.IsNullOrWhiteSpace(criteria.Phone) &&
                string.Equals(customer.Phone, criteria.Phone, StringComparison.OrdinalIgnoreCase))
            {
                score += 30;
                reasons.Add("Phone");
            }

            var corporate = corporateCustomers.FirstOrDefault(x => x.Id == customer.Id);
            if (corporate is not null && !string.IsNullOrWhiteSpace(criteria.TaxId) &&
                string.Equals(corporate.TaxIdentification, criteria.TaxId, StringComparison.OrdinalIgnoreCase))
            {
                score += 40;
                reasons.Add("TaxId");
            }

            var individual = individualCustomers.FirstOrDefault(x => x.Id == customer.Id);
            if (individual is not null && !string.IsNullOrWhiteSpace(criteria.IdentificationNumber) &&
                string.Equals(individual.IdentificationNumber, criteria.IdentificationNumber, StringComparison.OrdinalIgnoreCase))
            {
                score += 40;
                reasons.Add("IdentificationNumber");
            }

            if (score > 0)
            {
                candidates.Add(new CustomerDuplicateCandidateResponse(customer.Id, string.Join(",", reasons), score));
            }
        }

        var total = candidates.Count;
        var itemsPage = candidates
            .OrderByDescending(x => x.Score)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return new PagedResult<CustomerDuplicateCandidateResponse>(itemsPage, total);
    }
}
