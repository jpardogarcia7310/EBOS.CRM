using EBOS.CRM.Application.Features.CRM.CustomerMerge.Commands.MergeCustomers;
using EBOS.CRM.Application.Options;
using EBOS.CRM.Contracts.Requests.CRM.CustomerMerge;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using EBOS.CRM.Domain.Interfaces.Services.CRM;
using Microsoft.Extensions.Options;
using Moq;
using EBOS.CRM.Domain.Exceptions;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CustomerMerge.Commands.MergeCustomers;

public class MergeCustomersCommandHandlerTest
{
    [Fact]
    public async Task Handle_WhenReasonMissing_Throws()
    {
        var handler = BuildHandler();
        var command = new MergeCustomersCommand(new MergeCustomersRequest(1, 10, new[] { 11L }, " "));

        var act = () => handler.Handle(command, CancellationToken.None);

        await Assert.ThrowsAsync<DomainValidationException>(act);
    }

    [Fact]
    public async Task Handle_WhenWinnerNotFound_Throws()
    {
        var customerRepo = new Mock<ICustomerRepository>();
        customerRepo.Setup(x => x.GetByIdAsync(10, It.IsAny<CancellationToken>())).ReturnsAsync((global::EBOS.CRM.Domain.Entities.CRM.Customer?)null);
        var handler = BuildHandler(customerRepo: customerRepo);
        var command = new MergeCustomersCommand(new MergeCustomersRequest(1, 10, new[] { 11L }, "dedupe"));

        var act = () => handler.Handle(command, CancellationToken.None);

        await Assert.ThrowsAsync<DomainValidationException>(act);
    }

    [Fact]
    public async Task Handle_WhenTransientDependencyFails_ThrowsTransientDomainFailure()
    {
        var customerRepo = new Mock<ICustomerRepository>();
        var corporateRepo = new Mock<ICorporateCustomerRepository>();
        var individualRepo = new Mock<IIndividualCustomerRepository>();
        var addressRepo = new Mock<ICustomerAddressRepository>();
        var preferenceRepo = new Mock<ICustomerPreferenceRepository>();
        var consentRepo = new Mock<ICustomerConsentRepository>();
        var mergeHistoryRepo = new Mock<ICustomerMergeHistoryRepository>();
        var accountContactRepo = new Mock<IAccountContactRepository>();
        var accountContactRoleRepo = new Mock<IAccountContactRoleRepository>();
        var audit = new Mock<IAuditService>();
        var currentUser = new Mock<ICurrentUserContext>();
        var metrics = new Mock<ICustomer360Metrics>();
        var options = global::Microsoft.Extensions.Options.Options.Create(new CustomerMergeOptions());

        currentUser.SetupGet(x => x.UserId).Returns(1);
        currentUser.SetupGet(x => x.CorrelationId).Returns("corr");

        var winner = new global::EBOS.CRM.Domain.Entities.CRM.Customer
        {
            Id = 10,
            TenantId = 1,
            Email = "winner@contoso.com",
            Phone = "111111111",
            Source = "CRM",
            Confidentiality = "NORMAL",
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };

        customerRepo.Setup(x => x.GetByIdAsync(10, It.IsAny<CancellationToken>())).ReturnsAsync(winner);
        corporateRepo.Setup(x => x.GetByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new global::EBOS.CRM.Domain.Entities.CRM.CorporateCustomer
            {
                Id = 10,
                TenantId = 1,
                LegalName = "Winner Corp",
                TaxIdentification = "A123",
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            });
        addressRepo.Setup(x => x.GetByCustomerIdsAsync(1, It.IsAny<IReadOnlyCollection<long>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new TimeoutException("address timeout"));

        var handler = new MergeCustomersCommandHandler(
            customerRepo.Object,
            corporateRepo.Object,
            individualRepo.Object,
            addressRepo.Object,
            preferenceRepo.Object,
            consentRepo.Object,
            mergeHistoryRepo.Object,
            accountContactRepo.Object,
            accountContactRoleRepo.Object,
            audit.Object,
            currentUser.Object,
            metrics.Object,
            options);

        var command = new MergeCustomersCommand(new MergeCustomersRequest(1, 10, Array.Empty<long>(), "dedupe"));

        var ex = await Assert.ThrowsAsync<TransientDomainFailureException>(() => handler.Handle(command, CancellationToken.None));
        Assert.Equal("DOMAIN_TRANSIENT_TIMEOUT", ex.Code);
    }

    private static MergeCustomersCommandHandler BuildHandler(
        Mock<ICustomerRepository>? customerRepo = null)
    {
        customerRepo ??= new Mock<ICustomerRepository>();
        var corporateRepo = new Mock<ICorporateCustomerRepository>();
        var individualRepo = new Mock<IIndividualCustomerRepository>();
        var addressRepo = new Mock<ICustomerAddressRepository>();
        var preferenceRepo = new Mock<ICustomerPreferenceRepository>();
        var consentRepo = new Mock<ICustomerConsentRepository>();
        var mergeHistoryRepo = new Mock<ICustomerMergeHistoryRepository>();
        var accountContactRepo = new Mock<IAccountContactRepository>();
        var accountContactRoleRepo = new Mock<IAccountContactRoleRepository>();
        var audit = new Mock<IAuditService>();
        var currentUser = new Mock<ICurrentUserContext>();
        currentUser.SetupGet(x => x.UserId).Returns(1);
        currentUser.SetupGet(x => x.CorrelationId).Returns("corr");
        var metrics = new Mock<ICustomer360Metrics>();
        var options = global::Microsoft.Extensions.Options.Options.Create(new CustomerMergeOptions());

        return new MergeCustomersCommandHandler(
            customerRepo.Object,
            corporateRepo.Object,
            individualRepo.Object,
            addressRepo.Object,
            preferenceRepo.Object,
            consentRepo.Object,
            mergeHistoryRepo.Object,
            accountContactRepo.Object,
            accountContactRoleRepo.Object,
            audit.Object,
            currentUser.Object,
            metrics.Object,
            options);
    }
}

