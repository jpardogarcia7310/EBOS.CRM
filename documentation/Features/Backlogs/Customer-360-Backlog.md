# Customer 360 Technical Backlog

Concrete work items aligned with the current local structure (Clean Architecture, CRM module under `EBOS.CRM.*`).
Focus: Customer 360 aggregates, commands/queries, endpoints, and tests.

## Scope (Customer 360)

- Accounts (corporate customers) and individuals.
- Contacts and roles inside accounts.
- Account hierarchies (holding/subsidiary).
- Communication preferences and consents.
- Dedupe, merge, and golden record (enterprise).

## Domain (EBOS.CRM.Domain)

### Aggregates and entities

- Customer (existing)
  - Ensure base fields and invariants remain consistent.
- CorporateCustomer (existing, account)
  - Extend with account-level attributes if needed (industry, size, website) without breaking existing mappings.
- IndividualCustomer (existing)
  - Use as contact identity for Account contacts (see AccountContact).
- CustomerAddress (existing)
  - Maintain primary address invariant (one primary per customer).

- AccountContact (new)
  - Purpose: link CorporateCustomer (account) to IndividualCustomer (contact).
  - Fields: Id, TenantId, CorporateCustomerId, IndividualCustomerId, IsPrimary, StartAt, EndAt, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy.
  - Behavior: Assign, Unassign, SetPrimary.

- AccountContactRole (new)
  - Purpose: roles of a contact in an account (Billing, Legal, Procurement, Tech, etc.).
  - Fields: Id, TenantId, AccountContactId, RoleCode, IsPrimary, ValidFrom, ValidTo.
  - Behavior: Activate, Deactivate.

- AccountHierarchy (new)
  - Purpose: parent-child relations between accounts (holding/subsidiary).
  - Fields: Id, TenantId, ParentCorporateCustomerId, ChildCorporateCustomerId, RelationType, ValidFrom, ValidTo, IsCurrent.
  - Behavior: AssignParent, EndRelation.

- CustomerPreference (new)
  - Purpose: channel preferences per customer.
  - Fields: Id, TenantId, CustomerId, Channel (Email/SMS/Phone), Preferred, UpdatedAt, UpdatedBy.

- CustomerConsent (new)
  - Purpose: explicit consent records per customer.
  - Fields: Id, TenantId, CustomerId, ConsentType, Granted, GrantedAt, Source, ExpiresAt, RevokedAt.

### Interfaces (Repositories)

- `IAccountContactRepository`
- `IAccountContactRoleRepository`
- `IAccountHierarchyRepository`
- `ICustomerPreferenceRepository`
- `ICustomerConsentRepository`

### Invariants

- TenantId required on all aggregates.
- Only one primary contact per CorporateCustomer (AccountContact.IsPrimary).
- Only one primary address per customer (CustomerAddress.IsPrimary).
- Account hierarchy cannot create cycles (Parent != Child, no loops).
- Consent history is append-only (do not overwrite; add new record).

## Application (EBOS.CRM.Application)

### Contracts (Requests/Responses)

- Requests:
  - AccountContact: AddAccountContactRequest, UpdateAccountContactRequest, SetPrimaryAccountContactRequest, DeleteAccountContactRequest.
  - AccountContactRole: AddAccountContactRoleRequest, UpdateAccountContactRoleRequest, DeleteAccountContactRoleRequest.
  - AccountHierarchy: AddAccountHierarchyRequest, EndAccountHierarchyRequest.
  - Preferences/Consents: UpsertCustomerPreferenceRequest, AddCustomerConsentRequest, RevokeCustomerConsentRequest.
  - Dedupe/Merge: FindCustomerDuplicatesRequest, MergeCustomersRequest.
- Responses:
  - AccountContactResponse, AccountContactRoleResponse, AccountHierarchyResponse.
  - CustomerPreferenceResponse, CustomerConsentResponse.
  - CustomerDuplicateCandidateResponse, CustomerMergeResultResponse.

### Features (Commands/Queries)

Structure mirrors current CRM features (e.g. `Features/CRM/Customer/...`), and use `IReadOnlyCollection<T>` for list queries.

- `Features/CRM/AccountContact/Commands/AddAccountContact`
- `Features/CRM/AccountContact/Commands/UpdateAccountContact`
- `Features/CRM/AccountContact/Commands/SetPrimaryAccountContact`
- `Features/CRM/AccountContact/Commands/DeleteAccountContact`
- `Features/CRM/AccountContact/Queries/GetAccountContactById`
- `Features/CRM/AccountContact/Queries/GetAllAccountContacts`
- `Features/CRM/AccountContact/Queries/GetAccountContactsByAccount`

- `Features/CRM/AccountContactRole/Commands/AddAccountContactRole`
- `Features/CRM/AccountContactRole/Commands/UpdateAccountContactRole`
- `Features/CRM/AccountContactRole/Commands/DeleteAccountContactRole`
- `Features/CRM/AccountContactRole/Queries/GetAccountContactRoleById`
- `Features/CRM/AccountContactRole/Queries/GetAccountContactRolesByAccountContact`

- `Features/CRM/AccountHierarchy/Commands/AddAccountHierarchy`
- `Features/CRM/AccountHierarchy/Commands/EndAccountHierarchy`
- `Features/CRM/AccountHierarchy/Queries/GetAccountHierarchyById`
- `Features/CRM/AccountHierarchy/Queries/GetAccountHierarchyByAccount`

- `Features/CRM/CustomerPreference/Commands/UpsertCustomerPreference`
- `Features/CRM/CustomerPreference/Queries/GetCustomerPreferencesByCustomer`

- `Features/CRM/CustomerConsent/Commands/AddCustomerConsent`
- `Features/CRM/CustomerConsent/Commands/RevokeCustomerConsent`
- `Features/CRM/CustomerConsent/Queries/GetCustomerConsentsByCustomer`

- `Features/CRM/CustomerMerge/Queries/FindCustomerDuplicates`
- `Features/CRM/CustomerMerge/Commands/MergeCustomers`

### Mapping

- `Mappings/CRM/MappingAccountContact`
- `Mappings/CRM/MappingAccountContactRole`
- `Mappings/CRM/MappingAccountHierarchy`
- `Mappings/CRM/MappingCustomerPreference`
- `Mappings/CRM/MappingCustomerConsent`

### Validation

- Validators per request (FluentValidation).
- Enforce tenant isolation and reference existence (account/customer/contact).
- Dedupe query should require minimal matching fields (email, phone, tax id, identification number).

## API (EBOS.CRM.Api)

### Controllers

Follow existing CRM controllers layout:

- `Controllers/CRM/AccountContact/AccountContactController`
- `Controllers/CRM/AccountContactRole/AccountContactRoleController`
- `Controllers/CRM/AccountHierarchy/AccountHierarchyController`
- `Controllers/CRM/CustomerPreference/CustomerPreferenceController`
- `Controllers/CRM/CustomerConsent/CustomerConsentController`
- `Controllers/CRM/CustomerMerge/CustomerMergeController`

### Endpoints (v2)

- AccountContact
  - `GET /api/v2/AccountContact`
  - `GET /api/v2/AccountContact/{id}`
  - `GET /api/v2/AccountContact/by-account/{corporateCustomerId}`
  - `POST /api/v2/AccountContact`
  - `PUT /api/v2/AccountContact/{id}`
  - `PATCH /api/v2/AccountContact/{id}/primary`
  - `DELETE /api/v2/AccountContact/{id}`

- AccountContactRole
  - `GET /api/v2/AccountContactRole/by-account-contact/{accountContactId}`
  - `POST /api/v2/AccountContactRole`
  - `PUT /api/v2/AccountContactRole/{id}`
  - `DELETE /api/v2/AccountContactRole/{id}`

- AccountHierarchy
  - `GET /api/v2/AccountHierarchy/by-account/{corporateCustomerId}`
  - `POST /api/v2/AccountHierarchy`
  - `PATCH /api/v2/AccountHierarchy/{id}/end`

- CustomerPreference
  - `GET /api/v2/CustomerPreference/by-customer/{customerId}`
  - `PUT /api/v2/CustomerPreference`

- CustomerConsent
  - `GET /api/v2/CustomerConsent/by-customer/{customerId}`
  - `POST /api/v2/CustomerConsent`
  - `PATCH /api/v2/CustomerConsent/{id}/revoke`

- CustomerMerge (enterprise)
  - `GET /api/v2/CustomerMerge/duplicates?email=...&phone=...&taxId=...&idNumber=...`
  - `POST /api/v2/CustomerMerge/merge`

## Infrastructure (EBOS.CRM.Infrastructure)

- EF Core configurations for new entities.
- Repository implementations and DI registrations.
- Update `CrmDbContext` with DbSet for new entities and migrations.

## Tests

### Unit tests (tests/EBOS.CRM.ApiTests)

- Command handler tests for new commands (happy path + not found).
- Validator tests for required fields and tenant scope.
- Query handler tests for list responses using `IReadOnlyCollection<T>`.

### Controller tests

- CRUD-style endpoints for AccountContact and AccountContactRole.
- Primary contact enforcement behavior.
- Account hierarchy end relation behavior.
- Preference upsert and consent revoke endpoints.

### Integration tests (tests/EBOS.CRM.IntegrationTests)

- End-to-end account contact flow (create account, create individual, link, set primary).
- Account hierarchy parent/child assignment and tenant isolation.
- Preferences and consent history across updates.
- Dedupe and merge scenarios with golden record rules.

### Concurrency/Stress tests

- Add controller-level tests similar to existing CRM entities for AccountContact and AccountHierarchy.

