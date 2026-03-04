# Proteccion de ramas (main/develop)

Este repositorio aplica esta politica:

- `main` solo puede actualizarse por Pull Request.
- El PR a `main` debe venir desde `develop`.
- `develop` no puede borrarse.

## 1) Check obligatorio en PR a main

El workflow `.github/workflows/WFProtectBranchMain.yml` falla cualquier PR a `main` cuyo origen no sea `develop`.

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
