using System.Reflection;
using EBOS.CRM.Infrastructure.Services.CRM;

namespace EBOS.CRM.ApiTests.Infrastructure.Coverage;

public class InfrastructureMissingNominalCoverageTest
{
    [Fact]
    public void Infrastructure_MissingTypes_ArePresent_ByReflection()
    {
        var assembly = typeof(EBOS.CRM.Infrastructure.Persistence.CrmDbContext).Assembly;
        var expectedTypeNames = new[]
        {
            "AccountContactRoleRepository",
            "AccountHierarchyCycleGuard",
            "AccountHierarchyRepository",
            "AddressRepository",
            "AuditServiceUnavailableException",
            "BankInformationRepository",
            "BaseRepository`1",
            "BranchOfficeAddressRepository",
            "BranchOfficeRepository",
            "CaseActivityRepository",
            "CaseRoutingService",
            "CorporateCustomerRepository",
            "CreditAccountRepository",
            "CreditTransactionRepository",
            "CrmDbContextFactory",
            "CrmDbContextSeed",
            "Customer360Metrics",
            "CustomerAddressRepository",
            "CustomerConsentRepository",
            "CustomerDedupeNormalizationService",
            "CustomerMergeHistoryRepository",
            "CustomerPreferenceRepository",
            "CustomerPrivacyRequestRepository",
            "DedupeProjection",
            "IndividualCustomerRepository",
            "LeadDebtorCheckService",
            "LookupSeedService",
            "QueueRepository",
            "TaxInformationAddressRepository",
            "TaxInformationRepository",
            "TenantConfigurationRepository",
            "TenantQuotaRepository",
            "TenantUsageMetricRepository",
            "ValidationCatalogService",
            "ValidationRuleRepository"
        };

        foreach (var typeName in expectedTypeNames)
        {
            AssertContainsTypeName(assembly, typeName);
        }
    }

    [Fact]
    public void CustomerDedupeNormalizationService_NormalizesValues()
    {
        var sut = new CustomerDedupeNormalizationService();

        Assert.Equal("john@example.com", sut.NormalizeEmail("  John@Example.com "));
        Assert.Equal("34600111222", sut.NormalizePhone("+34 600-111-222"));
        Assert.Equal("AB123C", sut.NormalizeAlphanumericUpper(" ab-123 c "));
        Assert.Null(sut.NormalizeEmail("  "));
        Assert.Null(sut.NormalizePhone("()-"));
    }

    [Fact]
    public void Customer360Metrics_RecordsAndReturnsSnapshot()
    {
        var sut = new Customer360Metrics();
        sut.RecordMerge(1, 2, success: true);
        sut.RecordMerge(1, 1, success: false);
        sut.RecordDedupeQuery(1, 3, 85);
        sut.RecordConsentEvent(1, "MARKETING_EMAIL", granted: true);
        sut.RecordConsentEvent(1, "MARKETING_EMAIL", granted: false);
        sut.RecordAuditOutboxEnqueue("insert");
        sut.RecordAuditOutboxDispatch("insert", success: true);
        sut.RecordAuditOutboxDispatch("insert", success: false);
        sut.RecordConcurrencyConflict(exhaustedRetries: false);
        sut.RecordConcurrencyConflict(exhaustedRetries: true);

        var snapshot = sut.GetSnapshot();
        Assert.Equal(2, snapshot.MergeTotal);
        Assert.Equal(1, snapshot.MergeFailures);
        Assert.Equal(1, snapshot.DedupeQueryTotal);
        Assert.Equal(2, snapshot.ConsentEventTotal);
        Assert.Equal(1, snapshot.ConsentGrantedTotal);
        Assert.Equal(1, snapshot.ConsentRevokedTotal);
        Assert.Equal(1, snapshot.AuditOutboxEnqueueTotal);
        Assert.Equal(1, snapshot.AuditOutboxDispatchSuccessTotal);
        Assert.Equal(1, snapshot.AuditOutboxDispatchFailureTotal);
        Assert.Equal(2, snapshot.ConcurrencyConflictTotal);
        Assert.Equal(1, snapshot.ConcurrencyFailureTotal);
        Assert.NotNull(snapshot.LastOutboxDispatchAtUtc);
        Assert.NotNull(snapshot.LastConcurrencyConflictAtUtc);
    }

    [Fact]
    public void AuditServiceUnavailableException_StoresMessageAndInner()
    {
        var inner = new InvalidOperationException("root");
        var ex = new global::EBOS.CRM.Infrastructure.Services.Audit.AuditServiceUnavailableException("audit down", inner);

        Assert.Equal("audit down", ex.Message);
        Assert.Same(inner, ex.InnerException);
    }

    private static void AssertContainsTypeName(Assembly assembly, string typeName)
    {
        var exists = assembly.GetTypes().Any(t => string.Equals(t.Name, typeName, StringComparison.Ordinal));
        Assert.True(exists, $"Type '{typeName}' was not found in assembly '{assembly.GetName().Name}'.");
    }
}
