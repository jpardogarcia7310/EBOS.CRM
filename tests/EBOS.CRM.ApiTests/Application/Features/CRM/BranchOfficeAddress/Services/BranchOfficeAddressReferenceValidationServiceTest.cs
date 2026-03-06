using EBOS.CRM.Application.Features.CRM.BranchOfficeAddress.Services;
using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.BranchOfficeAddress.Services;

public class BranchOfficeAddressReferenceValidationServiceTest
{
    [Fact]
    public async Task EnsureDependenciesAvailableAsync_WhenBranchOfficeMissing_ThrowsValidation()
    {
        var branchOfficeRepository = new Mock<IBranchOfficeRepository>();
        var addressRepository = new Mock<IAddressRepository>();
        branchOfficeRepository.Setup(x => x.GetByIdAsync(4, It.IsAny<CancellationToken>()))
            .ReturnsAsync((global::EBOS.CRM.Domain.Entities.CRM.BranchOffice?)null);

        var service = new BranchOfficeAddressReferenceValidationService(branchOfficeRepository.Object, addressRepository.Object);

        var ex = await Assert.ThrowsAsync<DomainValidationException>(() =>
            service.EnsureDependenciesAvailableAsync(1, 4, 10, CancellationToken.None));
        Assert.Equal("DOMAIN_VALIDATION_BRANCH_OFFICE_NOT_FOUND", ex.Code);
    }

    [Fact]
    public async Task EnsureDependenciesAvailableAsync_WhenTimeout_ThrowsTransientDeterministicCode()
    {
        var branchOfficeRepository = new Mock<IBranchOfficeRepository>();
        var addressRepository = new Mock<IAddressRepository>();
        branchOfficeRepository.Setup(x => x.GetByIdAsync(4, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new TimeoutException("branch office timeout"));

        var service = new BranchOfficeAddressReferenceValidationService(branchOfficeRepository.Object, addressRepository.Object);

        var ex = await Assert.ThrowsAsync<TransientDomainFailureException>(() =>
            service.EnsureDependenciesAvailableAsync(1, 4, 10, CancellationToken.None));
        Assert.Equal("DOMAIN_TRANSIENT_BRANCH_OFFICE_ADDRESS_REFERENCE_RESOLUTION", ex.Code);
    }
}
