namespace EBOS.CRM.Domain.Identity;

public static class PolicyKeys
{
    public static class Operations
    {
        public const string ObservabilityRead = "Policy.Operations.Observability.Read";
        public const string ReadinessRead = "Policy.Operations.Readiness.Read";
    }

    public static class Crm
    {
        public const string CountryRead = "Policy.Crm.Country.Read";
        public const string CountryCreate = "Policy.Crm.Country.Create";
        public const string CountryUpdate = "Policy.Crm.Country.Update";
        public const string CountryDelete = "Policy.Crm.Country.Delete";
        public const string CountryPatch = "Policy.Crm.Country.Patch";

        public const string StatusRead = "Policy.Crm.Status.Read";
        public const string StatusCreate = "Policy.Crm.Status.Create";
        public const string StatusUpdate = "Policy.Crm.Status.Update";
        public const string StatusDelete = "Policy.Crm.Status.Delete";
        public const string StatusPatch = "Policy.Crm.Status.Patch";

        public const string IdentificationTypeRead = "Policy.Crm.IdentificationType.Read";
        public const string IdentificationTypeCreate = "Policy.Crm.IdentificationType.Create";
        public const string IdentificationTypeUpdate = "Policy.Crm.IdentificationType.Update";
        public const string IdentificationTypeDelete = "Policy.Crm.IdentificationType.Delete";
        public const string IdentificationTypePatch = "Policy.Crm.IdentificationType.Patch";

        public const string AddressTypeRead = "Policy.Crm.AddressType.Read";
        public const string AddressTypeCreate = "Policy.Crm.AddressType.Create";
        public const string AddressTypeUpdate = "Policy.Crm.AddressType.Update";
        public const string AddressTypeDelete = "Policy.Crm.AddressType.Delete";
        public const string AddressTypePatch = "Policy.Crm.AddressType.Patch";

        public const string AddressRead = "Policy.Crm.Address.Read";
        public const string AddressCreate = "Policy.Crm.Address.Create";
        public const string AddressUpdate = "Policy.Crm.Address.Update";
        public const string AddressDelete = "Policy.Crm.Address.Delete";
        public const string AddressPatch = "Policy.Crm.Address.Patch";

        public const string BankInformationRead = "Policy.Crm.BankInformation.Read";
        public const string BankInformationCreate = "Policy.Crm.BankInformation.Create";
        public const string BankInformationUpdate = "Policy.Crm.BankInformation.Update";
        public const string BankInformationDelete = "Policy.Crm.BankInformation.Delete";
        public const string BankInformationPatch = "Policy.Crm.BankInformation.Patch";

        public const string BranchOfficeRead = "Policy.Crm.BranchOffice.Read";
        public const string BranchOfficeCreate = "Policy.Crm.BranchOffice.Create";
        public const string BranchOfficeUpdate = "Policy.Crm.BranchOffice.Update";
        public const string BranchOfficeDelete = "Policy.Crm.BranchOffice.Delete";
        public const string BranchOfficePatch = "Policy.Crm.BranchOffice.Patch";

        public const string BranchOfficeAddressRead = "Policy.Crm.BranchOfficeAddress.Read";
        public const string BranchOfficeAddressCreate = "Policy.Crm.BranchOfficeAddress.Create";
        public const string BranchOfficeAddressUpdate = "Policy.Crm.BranchOfficeAddress.Update";
        public const string BranchOfficeAddressDelete = "Policy.Crm.BranchOfficeAddress.Delete";
        public const string BranchOfficeAddressPatch = "Policy.Crm.BranchOfficeAddress.Patch";

        public const string CorporateCustomerRead = "Policy.Crm.CorporateCustomer.Read";
        public const string CorporateCustomerCreate = "Policy.Crm.CorporateCustomer.Create";
        public const string CorporateCustomerUpdate = "Policy.Crm.CorporateCustomer.Update";
        public const string CorporateCustomerDelete = "Policy.Crm.CorporateCustomer.Delete";
        public const string CorporateCustomerPatch = "Policy.Crm.CorporateCustomer.Patch";

        public const string CreditAccountRead = "Policy.Crm.CreditAccount.Read";
        public const string CreditAccountCreate = "Policy.Crm.CreditAccount.Create";
        public const string CreditAccountUpdate = "Policy.Crm.CreditAccount.Update";
        public const string CreditAccountDelete = "Policy.Crm.CreditAccount.Delete";
        public const string CreditAccountPatch = "Policy.Crm.CreditAccount.Patch";

        public const string CreditTransactionRead = "Policy.Crm.CreditTransaction.Read";
        public const string CreditTransactionCreate = "Policy.Crm.CreditTransaction.Create";
        public const string CreditTransactionUpdate = "Policy.Crm.CreditTransaction.Update";
        public const string CreditTransactionDelete = "Policy.Crm.CreditTransaction.Delete";
        public const string CreditTransactionPatch = "Policy.Crm.CreditTransaction.Patch";

        public const string CustomerRead = "Policy.Crm.Customer.Read";
        public const string CustomerPiiRead = "Policy.Crm.Customer.Pii.Read";
        public const string CustomerCreate = "Policy.Crm.Customer.Create";
        public const string CustomerUpdate = "Policy.Crm.Customer.Update";
        public const string CustomerDelete = "Policy.Crm.Customer.Delete";
        public const string CustomerPatch = "Policy.Crm.Customer.Patch";

        public const string CustomerAddressRead = "Policy.Crm.CustomerAddress.Read";
        public const string CustomerAddressCreate = "Policy.Crm.CustomerAddress.Create";
        public const string CustomerAddressUpdate = "Policy.Crm.CustomerAddress.Update";
        public const string CustomerAddressDelete = "Policy.Crm.CustomerAddress.Delete";
        public const string CustomerAddressPatch = "Policy.Crm.CustomerAddress.Patch";

        public const string IndividualCustomerRead = "Policy.Crm.IndividualCustomer.Read";
        public const string IndividualCustomerCreate = "Policy.Crm.IndividualCustomer.Create";
        public const string IndividualCustomerUpdate = "Policy.Crm.IndividualCustomer.Update";
        public const string IndividualCustomerDelete = "Policy.Crm.IndividualCustomer.Delete";
        public const string IndividualCustomerPatch = "Policy.Crm.IndividualCustomer.Patch";

        public const string TaxInformationRead = "Policy.Crm.TaxInformation.Read";
        public const string TaxInformationCreate = "Policy.Crm.TaxInformation.Create";
        public const string TaxInformationUpdate = "Policy.Crm.TaxInformation.Update";
        public const string TaxInformationDelete = "Policy.Crm.TaxInformation.Delete";
        public const string TaxInformationPatch = "Policy.Crm.TaxInformation.Patch";

        public const string TaxInformationAddressRead = "Policy.Crm.TaxInformationAddress.Read";
        public const string TaxInformationAddressCreate = "Policy.Crm.TaxInformationAddress.Create";
        public const string TaxInformationAddressUpdate = "Policy.Crm.TaxInformationAddress.Update";
        public const string TaxInformationAddressDelete = "Policy.Crm.TaxInformationAddress.Delete";
        public const string TaxInformationAddressPatch = "Policy.Crm.TaxInformationAddress.Patch";

        public const string LeadRead = "Policy.Crm.Lead.Read";
        public const string LeadCreate = "Policy.Crm.Lead.Create";
        public const string LeadUpdate = "Policy.Crm.Lead.Update";
        public const string LeadDelete = "Policy.Crm.Lead.Delete";
        public const string LeadPatch = "Policy.Crm.Lead.Patch";

        public const string OpportunityRead = "Policy.Crm.Opportunity.Read";
        public const string OpportunityCreate = "Policy.Crm.Opportunity.Create";
        public const string OpportunityUpdate = "Policy.Crm.Opportunity.Update";
        public const string OpportunityDelete = "Policy.Crm.Opportunity.Delete";
        public const string OpportunityPatch = "Policy.Crm.Opportunity.Patch";

        public const string OpportunityStageRead = "Policy.Crm.OpportunityStage.Read";
        public const string OpportunityStageCreate = "Policy.Crm.OpportunityStage.Create";
        public const string OpportunityStageUpdate = "Policy.Crm.OpportunityStage.Update";
        public const string OpportunityStageDelete = "Policy.Crm.OpportunityStage.Delete";
        public const string OpportunityStagePatch = "Policy.Crm.OpportunityStage.Patch";

        public const string QuoteRead = "Policy.Crm.Quote.Read";
        public const string QuoteCreate = "Policy.Crm.Quote.Create";
        public const string QuoteUpdate = "Policy.Crm.Quote.Update";
        public const string QuoteDelete = "Policy.Crm.Quote.Delete";
        public const string QuotePatch = "Policy.Crm.Quote.Patch";

        public const string CaseRead = "Policy.Crm.Case.Read";
        public const string CaseCreate = "Policy.Crm.Case.Create";
        public const string CaseUpdate = "Policy.Crm.Case.Update";
        public const string CaseDelete = "Policy.Crm.Case.Delete";
        public const string CasePatch = "Policy.Crm.Case.Patch";

        public const string SlaRead = "Policy.Crm.Sla.Read";
        public const string SlaCreate = "Policy.Crm.Sla.Create";
        public const string SlaUpdate = "Policy.Crm.Sla.Update";
        public const string SlaDelete = "Policy.Crm.Sla.Delete";
        public const string SlaPatch = "Policy.Crm.Sla.Patch";

        public const string QueueRead = "Policy.Crm.Queue.Read";
        public const string QueueCreate = "Policy.Crm.Queue.Create";
        public const string QueueUpdate = "Policy.Crm.Queue.Update";
        public const string QueueDelete = "Policy.Crm.Queue.Delete";
        public const string QueuePatch = "Policy.Crm.Queue.Patch";

        public const string CaseActivityRead = "Policy.Crm.CaseActivity.Read";
        public const string CaseActivityCreate = "Policy.Crm.CaseActivity.Create";
        public const string CaseActivityUpdate = "Policy.Crm.CaseActivity.Update";
        public const string CaseActivityDelete = "Policy.Crm.CaseActivity.Delete";
        public const string CaseActivityPatch = "Policy.Crm.CaseActivity.Patch";
    }
}
