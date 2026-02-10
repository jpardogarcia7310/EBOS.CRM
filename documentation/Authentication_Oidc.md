# Authentication:Oidc Configuration (Step by Step)

This document explains how to configure the `Authentication:Oidc` section for EBOS.CRM and what each parameter means. The current setup is designed to work without a real provider until EBOS.Auth exists.

## 1) Where the configuration lives

Update these files depending on the environment:
- `EBOS.CRM.Api/appsettings.json` (base defaults)
- `EBOS.CRM.Api/appsettings.Development.json` (local development)
- `EBOS.CRM.Api/appsettings.Staging.json` (staging)

Example structure:

```json
"Authentication": {
  "Oidc": {
    "Authority": "",
    "Audience": "ebos.crm.api",
    "RequireHttpsMetadata": false,
    "ClockSkewSeconds": 60,
    "ValidIssuers": [ "https://auth.local/ebos" ],
    "ValidAudiences": [ "ebos.crm.api" ]
  }
}
```

## 2) Parameter-by-parameter

### Authority
- Type: `string`
- Example: `https://auth.local/ebos`
- Description: Base URL of the OIDC provider. When set, the API will try to read metadata from `/.well-known/openid-configuration`.
- For now: Leave empty to avoid calls to a real provider while EBOS.Auth is not ready.

### MetadataAddress
- Type: `string`
- Example: `https://auth.local/ebos/.well-known/openid-configuration`
- Description: Explicit metadata URL. Use this when the metadata endpoint is hosted in a non-standard location.
- For now: Leave empty to avoid outbound calls.

### Audience
- Type: `string`
- Example: `ebos.crm.api`
- Description: Expected `aud` (audience) claim in JWTs. Use the API identifier.

### RequireHttpsMetadata
- Type: `bool`
- Example: `false`
- Description: Controls whether the OIDC metadata endpoint must be HTTPS.
- For now: `false` is acceptable during local development or when no provider exists.

### ClockSkewSeconds
- Type: `int`
- Example: `60`
- Description: Allowed clock drift when validating token timestamps (`exp`, `nbf`).

### BackchannelTimeoutSeconds
- Type: `int`
- Example: `30`
- Description: Timeout for fetching OIDC metadata and signing keys.

### ValidIssuers
- Type: `string[]`
- Example: `[ "https://auth.local/ebos" ]`
- Description: Explicit list of valid `iss` claim values. Use this when `Authority` is empty or when you want strict issuer control.
- For now: Keep a fictitious issuer string to match future EBOS.Auth.

### ValidAudiences
- Type: `string[]`
- Example: `[ "ebos.crm.api" ]`
- Description: Explicit list of valid `aud` values, used when `Audience` is not enough or for multiple audiences.

### RoleClaimType
- Type: `string`
- Example: `roles`
- Description: Source claim name that will be mapped into `ClaimTypes.Role` during token validation. Supports arrays (JSON), comma, or space-separated values.

### PermissionClaimType
- Type: `string`
- Example: `permissions`
- Description: Source claim name that will be mapped into `permission` claims during token validation. Supports arrays (JSON), comma, or space-separated values.

## 3) Step-by-step setup (current phase)

1. Set `Authority` to an empty string.
2. Set `Audience` to `ebos.crm.api`.
3. Set `RequireHttpsMetadata` to `false`.
4. Set `ValidIssuers` to a fictitious value (for example `https://auth.local/ebos`).
5. Set `ValidAudiences` to `ebos.crm.api`.
6. Run the API and confirm it starts without trying to contact a real provider.

## 4) Step-by-step setup (future EBOS.Auth)

When EBOS.Auth exists:
1. Set `Authority` to the EBOS.Auth base URL.
2. Set `RequireHttpsMetadata` to `true` in non-development environments.
3. Align `ValidIssuers` with the real issuer produced by EBOS.Auth.
4. Keep `Audience` and `ValidAudiences` aligned with the API identifier.

Default local port for EBOS.Auth (planned):
- `http://127.0.0.1:5013`

## 5) Notes for EBOS.Auth

EBOS.Auth will be responsible for:
- Publishing OIDC metadata.
- Issuing JWTs that contain `iss` and `aud` values matching this configuration.
