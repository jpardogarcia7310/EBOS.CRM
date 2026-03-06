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

