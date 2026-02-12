# Customer 360 Backlog Técnico

Trabajo concreto alineado con la estructura local (Clean Architecture, módulo CRM bajo `EBOS.CRM.*`).
Enfoque: agregados Customer 360, comandos/queries, endpoints y tests.

## Alcance (Customer 360)

- Cuentas (corporate customers) e individuos.
- Contactos y roles dentro de cuentas.
- Jerarquía de cuentas (holding/subsidiary).
- Preferencias y consentimientos por canal.
- Dedupe, merge y golden record (enterprise).

## Domain (EBOS.CRM.Domain)

### Agregados y entidades

- Customer (existente)
  - Mantener campos base e invariantes.
- CorporateCustomer (existente, account)
  - Extender con atributos de cuenta si hace falta (industry, size, website) sin romper mappings.
- IndividualCustomer (existente)
  - Usar como identidad de contacto en cuentas (ver AccountContact).
- CustomerAddress (existente)
  - Mantener invariante de dirección primaria (una primaria por customer).

- AccountContact (nuevo)
  - Propósito: vinculo entre CorporateCustomer (account) e IndividualCustomer (contact).
  - Campos: Id, TenantId, CorporateCustomerId, IndividualCustomerId, IsPrimary, StartAt, EndAt, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy.
  - Comportamiento: Assign, Unassign, SetPrimary.

- AccountContactRole (nuevo)
  - Propósito: roles del contacto dentro de la cuenta (Billing, Legal, Procurement, Tech, etc.).
  - Campos: Id, TenantId, AccountContactId, RoleCode, IsPrimary, ValidFrom, ValidTo.
  - Comportamiento: Activate, Deactivate.

- AccountHierarchy (nuevo)
  - Propósito: relación parent-child entre cuentas (holding/subsidiary).
  - Campos: Id, TenantId, ParentCorporateCustomerId, ChildCorporateCustomerId, RelationType, ValidFrom, ValidTo, IsCurrent.
  - Comportamiento: AssignParent, EndRelation.

- CustomerPreference (nuevo)
  - Propósito: preferencias por canal del customer.
  - Campos: Id, TenantId, CustomerId, Channel (Email/SMS/Phone), Preferred, UpdatedAt, UpdatedBy.

- CustomerConsent (nuevo)
  - Propósito: registro de consentimientos por customer.
  - Campos: Id, TenantId, CustomerId, ConsentType, Granted, GrantedAt, Source, ExpiresAt, RevokedAt.

### Interfaces (Repositories)

- `IAccountContactRepository`
- `IAccountContactRoleRepository`
- `IAccountHierarchyRepository`
- `ICustomerPreferenceRepository`
- `ICustomerConsentRepository`

### Invariantes

- TenantId requerido en todos los agregados.
- Solo un contacto primario por CorporateCustomer (AccountContact.IsPrimary).
- Solo una dirección primaria por customer (CustomerAddress.IsPrimary).
- La jerarquía de cuentas no puede crear ciclos (Parent != Child, sin loops).
- El historial de consentimientos es append-only (no sobrescribir; agregar nuevo registro).

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

La estructura debe copiar el patron CRM existente (p. ej. `Features/CRM/Customer/...`), y las listas usan `IReadOnlyCollection<T>`.

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

- Validadores por request (FluentValidation).
- Enforzar tenant isolation y existencia de referencias (account/customer/contact).
- La query de dedupe requiere campos mínimos de matching (email, phone, tax id, identification number).

## API (EBOS.CRM.Api)

### Controllers

Seguir el layout actual de controllers CRM:

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

- Configuraciones EF Core para nuevas entidades.
- Implementaciones de repositorios y registros DI.
- Actualizar `CrmDbContext` con DbSet para nuevas entidades y migraciones.

## Tests

### Unit tests (tests/EBOS.CRM.ApiTests)

- Tests de command handlers para nuevos comandos (happy path + not found).
- Tests de validacion para required fields y tenant scope.
- Tests de query handlers para listas con `IReadOnlyCollection<T>`.

### Controller tests

- Endpoints CRUD para AccountContact y AccountContactRole.
- Comportamiento de contacto primario.
- Comportamiento de fin de relación en AccountHierarchy.
- Upsert de preferencias y revocación de consentimiento.

### Integration tests (tests/EBOS.CRM.IntegrationTests)

- Flujo end-to-end account contact (crear account, crear individual, link, set primary).
- Jerarquía parent/child y tenant isolation.
- Preferencias y historial de consentimientos.
- Dedupe y merge con reglas de golden record.

### Concurrency/Stress tests

- Agregar tests similares a los CRM existentes para AccountContact y AccountHierarchy.

