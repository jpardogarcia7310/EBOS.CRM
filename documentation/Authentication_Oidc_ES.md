# Configuración Authentication:Oidc (Paso a Paso)

Este documento explica cómo configurar la sección `Authentication:Oidc` para EBOS.CRM y qué significa cada parámetro. La configuración actual está preparada para funcionar sin un proveedor real hasta que exista EBOS.Auth.

## 1) Dónde vive la configuración

Actualiza estos archivos según el entorno:
- `EBOS.CRM.Api/appsettings.json` (valores base)
- `EBOS.CRM.Api/appsettings.Development.json` (desarrollo local)
- `EBOS.CRM.Api/appsettings.Staging.json` (staging)

Estructura de ejemplo:

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

## 2) Parámetro por parámetro

### Authority
- Tipo: `string`
- Ejemplo: `https://auth.local/ebos`
- Descripción: URL base del proveedor OIDC. Si se define, la API intentará leer los metadatos en `/.well-known/openid-configuration`.
- Por ahora: Déjalo vacío para evitar llamadas a un proveedor real mientras EBOS.Auth no exista.

### MetadataAddress
- Tipo: `string`
- Ejemplo: `https://auth.local/ebos/.well-known/openid-configuration`
- Descripción: URL explícita de metadatos. Úsala si el punto final está en una ruta no estándar.
- Por ahora: Déjalo vacío para evitar llamadas externas.

### Audience
- Tipo: `string`
- Ejemplo: `ebos.crm.api`
- Descripción: Valor esperado en el claim `aud` del JWT. Usa el identificador de la API.

### RequireHttpsMetadata
- Tipo: `bool`
- Ejemplo: `false`
- Descripción: Indica si el punto final de metadatos debe ser HTTPS.
- Por ahora: `false` es válido durante desarrollo local o cuando no hay proveedor.

### ClockSkewSeconds
- Tipo: `int`
- Ejemplo: `60`
- Descripción: Margen de desfase de reloj al validar tiempos (`exp`, `nbf`).

### BackchannelTimeoutSeconds
- Tipo: `int`
- Ejemplo: `30`
- Descripción: Tiempo de espera para obtener metadatos OIDC y llaves de firma.

### ValidIssuers
- Tipo: `string[]`
- Ejemplo: `[ "https://auth.local/ebos" ]`
- Descripción: Lista explícita de valores válidos para el claim `iss`. Se usa cuando `Authority` está vacío o para control estricto.
- Por ahora: Mantener un valor ficticio alineado con el futuro EBOS.Auth.

### ValidAudiences
- Tipo: `string[]`
- Ejemplo: `[ "ebos.crm.api" ]`
- Descripción: Lista explícita de valores válidos para `aud`, útil si hay múltiples audiencias.

### RoleClaimType
- Tipo: `string`
- Ejemplo: `roles`
- Descripción: Nombre del atributo de origen que se mapea a `ClaimTypes.Role` durante la validación del token. Soporta arrays (JSON) y valores separados por coma o espacio.

### PermissionClaimType
- Tipo: `string`
- Ejemplo: `permissions`
- Descripción: Nombre del atributo de origen que se mapea a atributos `permission` durante la validación del token. Soporta arrays (JSON) y valores separados por coma o espacio.

## 3) Pasos de configuración (fase actual)

1. Define `Authority` como cadena vacía.
2. Define `Audience` como `ebos.crm.api`.
3. Define `RequireHttpsMetadata` como `false`.
4. Define `ValidIssuers` con un valor ficticio (por ejemplo `https://auth.local/ebos`).
5. Define `ValidAudiences` como `ebos.crm.api`.
6. Ejecuta la API y confirma que inicia sin intentar conectarse a un proveedor real.

## 4) Pasos de configuración (futuro EBOS.Auth)

Cuando EBOS.Auth exista:
1. Define `Authority` con la URL base de EBOS.Auth.
2. Define `RequireHttpsMetadata` como `true` en entornos no locales.
3. Alinea `ValidIssuers` con el issuer real generado por EBOS.Auth.
4. Mantén `Audience` y `ValidAudiences` alineados con el identificador de la API.

Puerto local por defecto para EBOS.Auth (planificado):
- `http://127.0.0.1:5013`

## 5) Notas para EBOS.Auth

EBOS.Auth será responsable de:
- Publicar los metadatos OIDC.
- Emitir JWTs con `iss` y `aud` que coincidan con esta configuración.
