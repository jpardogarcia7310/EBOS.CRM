# Customer 360 Backlog Técnico

Trabajo concreto alineado con la estructura local (Clean Architecture, módulo CRM bajo `EBOS.CRM.*`).
Enfoque: agregados Customer 360, comandos/queries, endpoints y tests.

Mini TOC:
1. [Alcance](#alcance-customer-360)
2. [Dominio](#dominio-eboscrmdomain)
3. [Agregados y entidades](#agregados-y-entidades)
4. [Interfaces](#interfaces-repositories)
5. [Invariantes](#invariantes)
6. [Aplicación](#aplicacion-eboscrmapplication)
7. [Contratos](#contratos-solicitudesrespuestas)
8. [Funcionalidades](#funcionalidades-comandosconsultas)
9. [Mapeo](#mapeo)
10. [Validación](#validacion)
11. [Catálogo de ConsentType](#catalogo-de-consenttype-referencia)
12. [API](#api-eboscrmapi)
13. [Controladores](#controladores)
14. [Puntos finales](#puntos-finales-v2)
15. [Infraestructura](#infraestructura-eboscrminfrastructure)
16. [Pruebas](#pruebas)
17. [Pruebas de dominio](#pruebas-de-dominio)
18. [Pruebas de aplicación](#pruebas-de-aplicacion)
19. [Pruebas de controladores](#pruebas-de-controladores)
20. [Pruebas de integración](#pruebas-de-integracion)
21. [Pruebas de mapeo](#pruebas-de-mapeo)
22. [Referencia de suites de pruebas existentes](#referencia-de-suites-de-pruebas-existentes)

## Alcance (Customer 360)

- Cuentas (clientes corporativos) e individuos.
- Contactos y roles dentro de cuentas.
- Jerarquía de cuentas (holding/subsidiary).
- Preferencias y consentimientos por canal.
- Deduplicación, fusión y registro maestro (empresarial).

## Dominio (EBOS.CRM.Domain)

### Agregados y entidades

- Customer (existente)
  - Mantener campos base e invariantes.
- CorporateCustomer (existente, cuenta)
  - Extender con atributos de cuenta si hace falta (industry, size, website) sin romper mappings.
- IndividualCustomer (existente)
  - Usar como identidad de contacto en cuentas (ver AccountContact).
- CustomerAddress (existente)
  - Mantener invariante de dirección primaria (una primaria por customer).

- AccountContact (nuevo)
  - Propósito: vínculo entre CorporateCustomer (account) e IndividualCustomer (contact).
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
  - Propósito: preferencias por canal del cliente.
  - Campos: Id, TenantId, CustomerId, Channel (Email/SMS/Phone), Preferred, UpdatedAt, UpdatedBy.

- CustomerConsent (nuevo)
  - Propósito: registro de consentimientos por cliente.
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
- Solo una dirección primaria por cliente (CustomerAddress.IsPrimary).
- La jerarquía de cuentas no puede crear ciclos (Parent != Child, sin loops).
- El historial de consentimientos es de solo anexar (no sobrescribir; agregar nuevo registro).
  - Convención de expiración (explícita vía AddCustomerConsent):
    - Usar `Granted = false` con `ExpiresAt == GrantedAt` para registrar un evento de expiración.
    - Esto crea un nuevo evento de consentimiento (solo anexar) y marca el consentimiento como no otorgado.
    - El regrant es un evento separado con `Granted = true` (también append-only).
    - Ejemplos de carga útil:
      - Otorgar:
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
      - Expirar:
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
      - Reotorgar:
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
      - Revocar:
        ```
        {
          "tenantId": 1,
          "customerId": 1001,
          "consentType": "MARKETING_EMAIL",
          "granted": false,
          "grantedAt": "2026-03-15T14:05:00Z",
          "source": "solicitud-cliente",
          "expiresAt": "2026-03-15T14:05:00Z"
        }
        ```
    - Nota sobre punto final de revocación:
      - `PATCH /api/v2/CustomerConsent/{id}/revoke` es una revocación explícita de un consentimiento existente.
      - Para un evento de revocación, usar el punto final de revocación. Para un evento de expiración, usar
        `AddCustomerConsent` con `Granted = false` y `ExpiresAt == GrantedAt`.
    - Tabla resumen:
      | Acción | Granted | ExpiresAt vs GrantedAt | Punto final | Notas |
      | --- | --- | --- | --- | --- |
      | Otorgar | true | `ExpiresAt` opcional (>= GrantedAt) | `POST /api/v2/CustomerConsent` | Nuevo evento de consentimiento |
      | Reotorgar | true | `ExpiresAt` opcional (>= GrantedAt) | `POST /api/v2/CustomerConsent` | Nuevo evento tras revocar/expirar |
      | Expirar | false | `ExpiresAt == GrantedAt` (requerido) | `POST /api/v2/CustomerConsent` | Evento explícito de expiración |
      | Revocar | false | `ExpiresAt == GrantedAt` (requerido) | `PATCH /api/v2/CustomerConsent/{id}/revoke` | Revocación explícita de registro |
    - Naming de ConsentType:
      - Usar códigos estables, en mayúsculas y con guiones bajos (por ejemplo, `MARKETING_EMAIL`, `PRODUCT_UPDATES_SMS`).
      - Tratar `ConsentType` como clave funcional para agrupar el "último estado".
    - Reglas de validación:
      - `TenantId > 0`, `CustomerId > 0`.
      - `ConsentType` requerido, máximo 100 caracteres.
      - `Source` requerido, máximo 100 caracteres.
      - `GrantedAt` requerido.
      - `ExpiresAt` debe ser null o >= `GrantedAt`.
      - Si `Granted = false`, `ExpiresAt` es requerido y debe ser igual a `GrantedAt`.


## Aplicación (EBOS.CRM.Application)

### Contratos (Solicitudes/Respuestas)

- Solicitudes:
  - AccountContact: AddAccountContactRequest, UpdateAccountContactRequest, SetPrimaryAccountContactRequest, DeleteAccountContactRequest.
  - AccountContactRole: AddAccountContactRoleRequest, UpdateAccountContactRoleRequest, DeleteAccountContactRoleRequest.
  - AccountHierarchy: AddAccountHierarchyRequest, EndAccountHierarchyRequest.
  - Preferences/Consents: UpsertCustomerPreferenceRequest, AddCustomerConsentRequest, RevokeCustomerConsentRequest.
  - Dedupe/Merge: FindCustomerDuplicatesRequest, MergeCustomersRequest.
- Respuestas:
  - AccountContactResponse, AccountContactRoleResponse, AccountHierarchyResponse.
  - CustomerPreferenceResponse, CustomerConsentResponse.
  - CustomerDuplicateCandidateResponse, CustomerMergeResultResponse.

### Funcionalidades (Comandos/Consultas)

La estructura debe copiar el patrón CRM existente (p. ej. `Features/CRM/Customer/...`), y las listas usan `IReadOnlyCollection<T>`.

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

### Mapeo

- `Mappings/CRM/MappingAccountContact`
- `Mappings/CRM/MappingAccountContactRole`
- `Mappings/CRM/MappingAccountHierarchy`
- `Mappings/CRM/MappingCustomerPreference`
- `Mappings/CRM/MappingCustomerConsent`

### Validación

- Validadores por solicitud (FluentValidation).
- Enforzar tenant isolation y existencia de referencias (account/customer/contact).
- La query de dedupe requiere campos mínimos de matching (email, phone, tax id, identification number).

### Catálogo de ConsentType (Referencia)

Catálogo sugerido (por canal):
- Email: `MARKETING_EMAIL`, `NEWSLETTER_EMAIL`, `PRODUCT_UPDATES_EMAIL`, `SECURITY_ALERTS_EMAIL`.
- SMS: `MARKETING_SMS`, `PRODUCT_UPDATES_SMS`, `SECURITY_ALERTS_SMS`.
- Phone: `MARKETING_CALL`, `SERVICE_CALL`, `SURVEYS_CALL`.
- Push: `PRODUCT_UPDATES_PUSH`, `SECURITY_ALERTS_PUSH`.

Ejemplos por canal:
- Email: `MARKETING_EMAIL`, `NEWSLETTER_EMAIL`.
- SMS: `MARKETING_SMS`, `PRODUCT_UPDATES_SMS`.
- Phone: `SERVICE_CALL`, `SURVEYS_CALL`.
- Push: `SECURITY_ALERTS_PUSH`.

## API (EBOS.CRM.Api)

### Controladores

Seguir el layout actual de controllers CRM:

- `Controllers/CRM/AccountContact/AccountContactController`
- `Controllers/CRM/AccountContactRole/AccountContactRoleController`
- `Controllers/CRM/AccountHierarchy/AccountHierarchyController`
- `Controllers/CRM/CustomerPreference/CustomerPreferenceController`
- `Controllers/CRM/CustomerConsent/CustomerConsentController`
- `Controllers/CRM/CustomerMerge/CustomerMergeController`

### Puntos finales (v2)

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

- CustomerMerge (empresarial)
  - `GET /api/v2/CustomerMerge/duplicates?email=...&phone=...&taxId=...&idNumber=...`
  - `POST /api/v2/CustomerMerge/merge`

## Infraestructura (EBOS.CRM.Infrastructure)

- Configuraciones EF Core para nuevas entidades.
- Implementaciones de repositorios y registros DI.
- Actualizar `CrmDbContext` con DbSet para nuevas entidades y migraciones.

## Pruebas

### Pruebas de dominio

- Invariantes de AccountContact y AccountContactRole (asignación de primario, ciclo de vida de roles y ventanas de vigencia).
- Restricciones acíclicas y de relación padre-hijo en AccountHierarchy.
- Comportamiento de solo anexar en CustomerConsent y transiciones de estado (otorgar, revocar, expirar, reotorgar).

### Pruebas de aplicación

- Manejadores de comandos/consultas para AccountContact, AccountContactRole, AccountHierarchy, CustomerPreference, CustomerConsent y CustomerMerge.
- Cobertura de validaciones para alcance por tenant, referencias requeridas y precondiciones de deduplicación/fusión.
- Comportamiento de mapeo y forma de respuestas en flujos de lista y detalle.

### Pruebas de controladores

- Endpoints CRUD para AccountContact y AccountContactRole.
- Comportamiento de contacto primario.
- Comportamiento de fin de relación en AccountHierarchy.
- Upsert de preferencias y convenciones de revocación/expiración de consentimiento.

### Pruebas de integración

- Flujo extremo a extremo de contacto en cuenta (crear cuenta, crear individuo, vincular, marcar como primario).
- Jerarquía parent/child y tenant isolation.
- Preferencias y historial de consentimientos.
- Dedupe y merge con reglas de golden record.

### Pruebas de mapeo

- Perfiles de mapeo para contratos de respuesta de AccountContact, AccountContactRole, AccountHierarchy, CustomerPreference, CustomerConsent y CustomerMerge.

### Referencia de suites de pruebas existentes

- `tests/EBOS.CRM.ApiTests`: cobertura unitaria y de componentes para manejadores, validadores, mapeos, políticas y controladores CRM implicados en flujos Customer 360.
- `tests/EBOS.CRM.ConcurrencyTests`: escenarios de acceso concurrente sobre endpoints CRM para validar manejo de conflictos, aislamiento por tenant bajo carga y consistencia de transiciones.
- `tests/EBOS.CRM.IntegrationTests`: validación extremo a extremo de API + persistencia para rutas Customer 360 (contactos, jerarquías, preferencias, consentimientos y fusión) con infraestructura real.
- `tests/EBOS.CRM.StressTests`: cobertura de estrés sobre controladores con alto volumen para validar rendimiento sostenido, estabilidad de respuesta y comportamiento bajo presión.
