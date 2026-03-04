# Proteccion de ramas (main/develop)

Este repositorio aplica esta politica:

- `main` solo puede actualizarse por Pull Request.
- El PR a `main` debe venir desde `develop`.
- `develop` no puede borrarse.

## 1) Check obligatorio en PR a main

El workflow `.github/workflows/WFProtectBranchMain.yml` falla cualquier PR a `main` cuyo origen no sea `develop`.

## 1.1) Auditoria y notificacion de cambios de reglas

El workflow `.github/workflows/branch-protection-audit-alert.yml` crea un issue de alerta cuando GitHub detecta cambios en:

- reglas de proteccion de ramas (`branch_protection_rule`)
- rulesets del repositorio (`repository_ruleset`)

## 1.2) Monitoreo periodico de drift (cron)

El workflow `.github/workflows/branch-protection-drift-monitor.yml` corre cada hora (`0 * * * *`) y valida que `main` y `develop` mantengan la configuracion esperada.

Si detecta drift:

- crea (o actualiza) un issue con etiqueta `security` y `branch-protection-audit`
- marca el workflow como fallido para dejar evidencia en Actions

Si el drift desaparece:

- cierra automaticamente el issue abierto de drift
- agrega comentario de resolucion con fecha y enlace a la ejecucion

## 1.3) Guard post-merge para `develop`

El workflow `.github/workflows/develop-branch-guard.yml` se ejecuta cuando se cierra un PR hacia `main` (solo si fue mergeado) y valida que la rama `develop` exista.

Si `develop` no existe:

- crea/actualiza issue `[SECURITY] develop branch is missing`
- marca el workflow como fallido

Si `develop` vuelve a existir:

- comenta y cierra automaticamente el issue abierto

## 2) Aplicar proteccion de ramas en GitHub

Requisitos:

- GitHub CLI (`gh`) instalado.
- Sesion iniciada con permisos de admin sobre el repo: `gh auth login`.

Ejecuta desde la raiz del repo:

```powershell
.\scripts\Protect-GitHubBranches.ps1
```

Opcionalmente, puedes pasar owner/repo manualmente:

```powershell
.\scripts\Protect-GitHubBranches.ps1 -Owner jpardogarcia7310 -Repo EBOS.CRM
```

El script configura:

- `main`: requiere PR, bloqueo de borrado, sin force-push, review obligatoria, y check `Validate source branch is develop`.
- `develop`: bloqueo de borrado y sin force-push.
- repositorio: desactiva `Automatically delete head branches` (`delete_branch_on_merge=false`).
