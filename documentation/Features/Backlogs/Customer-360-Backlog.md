# Customer 360 Technical Backlog

Concrete work items aligned with the current local structure (Clean Architecture, CRM module under `EBOS.CRM.*`).
Focus: Customer 360 aggregates, commands/queries, endpoints, and tests.

Mini TOC:
1. [Scope](#scope-customer-360)
2. [Domain](#domain-eboscrmdomain)
3. [Aggregates and entities](#aggregates-and-entities)
4. [Interfaces](#interfaces-repositories)
5. [Invariants](#invariants)
6. [Application](#application-eboscrmapplication)
7. [Contracts](#contracts-requestsresponses)
8. [Features](#features-commandsqueries)
9. [Mapping](#mapping)
10. [Validation](#validation)
11. [ConsentType Catalog](#consenttype-catalog-reference)
12. [API](#api-eboscrmapi)
13. [Controllers](#controllers)
14. [Endpoints](#endpoints-v2)
15. [Infrastructure](#infrastructure-eboscrminfrastructure)
16. [Tests](#tests)
17. [Domain tests](#domain-tests)
18. [Application tests](#application-tests)
19. [Controller tests](#controller-tests)
20. [Integration tests](#integration-tests)
21. [Mapping tests](#mapping-tests)
22. [Existing test suites reference](#existing-test-suites-reference)

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
  - Expire convention (explicit via AddCustomerConsent):
    - Use `Granted = false` with `ExpiresAt == GrantedAt` to record an expiration event.
    - This creates a new consent event (append-only) and marks the consent as not granted.
    - Re-grant is a separate event with `Granted = true` (also append-only).
    - Example payloads:
      - Grant:
        ```
        {
          "tenantId": 1,
          "customerId": 1001,
          "consentType": "MARKETING_EMAIL",
          "granted": true,
          "grantedAt": "2026-02-27T10:15:00Z",
          "source": "web-form",
          "expiresAt": null
        }
        ```
      - Expire:
        ```
        {
          "tenantId": 1,
          "customerId": 1001,
          "consentType": "MARKETING_EMAIL",
          "granted": false,
          "grantedAt": "2026-03-01T00:00:00Z",
          "source": "policy-expiration",
          "expiresAt": "2026-03-01T00:00:00Z"
        }
        ```
      - Re-grant:
        ```
        {
          "tenantId": 1,
          "customerId": 1001,
          "consentType": "MARKETING_EMAIL",
          "granted": true,
          "grantedAt": "2026-03-10T09:30:00Z",
          "source": "call-center",
          "expiresAt": null
        }
        ```
      - Revoke:
        ```
        {
          "tenantId": 1,
          "customerId": 1001,
          "consentType": "MARKETING_EMAIL",
          "granted": false,
          "grantedAt": "2026-03-15T14:05:00Z",
          "source": "customer-request",
          "expiresAt": "2026-03-15T14:05:00Z"
        }
        ```
    - Revoke endpoint note:
      - `PATCH /api/v2/CustomerConsent/{id}/revoke` is an explicit revoke of an existing consent record.
      - For a revoke event, use the revoke endpoint. For an expiration event, use `AddCustomerConsent` with
        `Granted = false` and `ExpiresAt == GrantedAt`.
    - Summary table:
      | Action | Granted | ExpiresAt vs GrantedAt | Endpoint | Notes |
      | --- | --- | --- | --- | --- |
      | Grant | true | `ExpiresAt` optional (>= GrantedAt) | `POST /api/v2/CustomerConsent` | New consent event |
      | Re-grant | true | `ExpiresAt` optional (>= GrantedAt) | `POST /api/v2/CustomerConsent` | New consent event after prior revoke/expire |
      | Expire | false | `ExpiresAt == GrantedAt` (required) | `POST /api/v2/CustomerConsent` | Explicit expiration event |
      | Revoke | false | `ExpiresAt == GrantedAt` (required) | `PATCH /api/v2/CustomerConsent/{id}/revoke` | Explicit revoke of existing record |
    - ConsentType naming:
      - Use stable, uppercase codes with underscores (e.g., `MARKETING_EMAIL`, `PRODUCT_UPDATES_SMS`).
      - Treat `ConsentType` as a functional key for "latest state" grouping.
    - Validation rules:
      - `TenantId > 0`, `CustomerId > 0`.
      - `ConsentType` required, max length 100.
      - `Source` required, max length 100.
      - `GrantedAt` required.
      - `ExpiresAt` must be null or >= `GrantedAt`.
      - If `Granted = false`, `ExpiresAt` is required and must equal `GrantedAt`.


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

### ConsentType Catalog (Reference)

Suggested catalog (by channel):
- Email: `MARKETING_EMAIL`, `NEWSLETTER_EMAIL`, `PRODUCT_UPDATES_EMAIL`, `SECURITY_ALERTS_EMAIL`.
- SMS: `MARKETING_SMS`, `PRODUCT_UPDATES_SMS`, `SECURITY_ALERTS_SMS`.
- Phone: `MARKETING_CALL`, `SERVICE_CALL`, `SURVEYS_CALL`.
- Push: `PRODUCT_UPDATES_PUSH`, `SECURITY_ALERTS_PUSH`.

Examples by channel:
- Email: `MARKETING_EMAIL`, `NEWSLETTER_EMAIL`.
- SMS: `MARKETING_SMS`, `PRODUCT_UPDATES_SMS`.
- Phone: `SERVICE_CALL`, `SURVEYS_CALL`.
- Push: `SECURITY_ALERTS_PUSH`.

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

### Domain tests

- AccountContact and AccountContactRole invariants (primary assignment, role lifecycle, valid date windows).
- AccountHierarchy acyclic and parent-child constraints.
- CustomerConsent append-only behavior and consent state transitions (grant, revoke, expire, re-grant).

### Application tests

- Command/query handlers for AccountContact, AccountContactRole, AccountHierarchy, CustomerPreference, CustomerConsent, and CustomerMerge.
- Validation coverage for tenant scope, required references, and dedupe/merge preconditions.
- Mapping and response-shape behavior for list and detail flows.

### Controller tests

- CRUD-style endpoints for AccountContact and AccountContactRole.
- Primary contact enforcement behavior.
- Account hierarchy end relation behavior.
- Preference upsert and consent revoke/expire conventions.

### Integration tests

- End-to-end account contact flow (create account, create individual, link, set primary).
- Account hierarchy parent/child assignment and tenant isolation.
- Preferences and consent history across updates.
- Dedupe and merge scenarios with golden record rules.

### Mapping tests

- Mapping profiles for AccountContact, AccountContactRole, AccountHierarchy, CustomerPreference, CustomerConsent, and CustomerMerge response contracts.

### Existing test suites reference

- `tests/EBOS.CRM.ApiTests`: unit-level and component-level coverage for handlers, validators, mappings, policies, and CRM controllers involved in Customer 360 flows.
- `tests/EBOS.CRM.ConcurrencyTests`: concurrent access scenarios on CRM endpoints to validate conflict handling, tenant isolation under load, and consistency of state transitions.
- `tests/EBOS.CRM.IntegrationTests`: end-to-end API + persistence validation for Customer 360 paths (contacts, hierarchies, preferences, consent, merge) with real infrastructure wiring.
- `tests/EBOS.CRM.StressTests`: high-volume controller stress coverage to validate throughput, response stability, and behavior under sustained pressure.

