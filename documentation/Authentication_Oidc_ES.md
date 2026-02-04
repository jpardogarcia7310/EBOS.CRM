# Configuracion Authentication:Oidc (Paso a Paso)

Este documento explica como configurar la seccion `Authentication:Oidc` para EBOS.CRM y que significa cada parametro. La configuracion actual esta preparada para funcionar sin un proveedor real hasta que exista EBOS.Auth.

## 1) Donde vive la configuracion

Actualiza estos archivos segun el entorno:
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

## 2) Parametro por parametro

### Authority
- Tipo: `string`
- Ejemplo: `https://auth.local/ebos`
- Descripcion: URL base del proveedor OIDC. Si se define, la API intentara leer la metadata en `/.well-known/openid-configuration`.
- Por ahora: Dejalo vacio para evitar llamadas a un proveedor real mientras EBOS.Auth no exista.

### Audience
- Tipo: `string`
- Ejemplo: `ebos.crm.api`
- Descripcion: Valor esperado en el claim `aud` del JWT. Usa el identificador de la API.

### RequireHttpsMetadata
- Tipo: `bool`
- Ejemplo: `false`
- Descripcion: Indica si el endpoint de metadata debe ser HTTPS.
- Por ahora: `false` es valido durante desarrollo local o cuando no hay proveedor.

### ClockSkewSeconds
- Tipo: `int`
- Ejemplo: `60`
- Descripcion: Margen de desfase de reloj al validar tiempos (`exp`, `nbf`).

### ValidIssuers
- Tipo: `string[]`
- Ejemplo: `[ "https://auth.local/ebos" ]`
- Descripcion: Lista explicita de valores validos para el claim `iss`. Se usa cuando `Authority` esta vacio o para control estricto.
- Por ahora: Mantener un valor ficticio alineado con el futuro EBOS.Auth.

### ValidAudiences
- Tipo: `string[]`
- Ejemplo: `[ "ebos.crm.api" ]`
- Descripcion: Lista explicita de valores validos para `aud`, util si hay multiples audiencias.

## 3) Pasos de configuracion (fase actual)

1. Define `Authority` como cadena vacia.
2. Define `Audience` como `ebos.crm.api`.
3. Define `RequireHttpsMetadata` como `false`.
4. Define `ValidIssuers` con un valor ficticio (por ejemplo `https://auth.local/ebos`).
5. Define `ValidAudiences` como `ebos.crm.api`.
6. Ejecuta la API y confirma que inicia sin intentar conectarse a un proveedor real.

## 4) Pasos de configuracion (futuro EBOS.Auth)

Cuando EBOS.Auth exista:
1. Define `Authority` con la URL base de EBOS.Auth.
2. Define `RequireHttpsMetadata` como `true` en entornos no locales.
3. Alinea `ValidIssuers` con el issuer real generado por EBOS.Auth.
4. Mantén `Audience` y `ValidAudiences` alineados con el identificador de la API.

## 5) Notas para EBOS.Auth

EBOS.Auth sera responsable de:
- Publicar la metadata OIDC.
- Emitir JWTs con `iss` y `aud` que coincidan con esta configuracion.

