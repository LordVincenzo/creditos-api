# AGENTS.md - Creditos API

## Objetivo
Mantener una API pequeña y demostrable para login de comerciales, registro/consulta de créditos y correo asíncrono.

## Reglas que no se deben romper
- .NET 8 y PostgreSQL son obligatorios.
- Nunca guardar contraseñas en texto plano.
- Nunca aceptar desde el cliente quién es el comercial que registra el crédito; se obtiene del JWT.
- Encolar el correo solo después de persistir el crédito.
- Una falla SMTP nunca debe revertir el crédito.
- No concatenar SQL ni aceptar nombres de columnas arbitrarios para ordenar.
- No versionar secretos, `.env`, keystores o claves privadas.
- Demo users únicamente en Development.

## Estructura
- `src/Creditos.Api/Controllers`: HTTP.
- `src/Creditos.Api/Data`: EF Core, seeding y migrations.
- `src/Creditos.Api/Entities`: modelo persistente.
- `src/Creditos.Api/DTOs`: contratos HTTP.
- `src/Creditos.Api/Services`: autenticación, créditos y SMTP.
- `src/Creditos.Api/Jobs`: Hangfire.
- `src/Creditos.Api/Authentication`: usuario actual desde JWT.
- `src/Creditos.Api/Middleware`: errores globales.
- `tests/Creditos.Api.Tests`: pruebas.
- `database/schema.sql`: esquema entregable.

## Comandos
```bash
dotnet restore
dotnet build
dotnet test
dotnet ef database update --project src/Creditos.Api
dotnet run --project src/Creditos.Api
docker compose up --build
```

## Convenciones
- UTC en backend.
- IDs UUID.
- Documento de cliente siempre texto.
- `Amount` numeric(18,2), `InterestRate` numeric(5,2).
- `pageSize` máximo 100.
- `sortBy`: solo `createdAt` o `amount`.
- Controllers delgados; reglas en services.

## Seguridad
- Producción requiere JWT key segura por variables de entorno.
- Swagger y Hangfire Dashboard no deben convertirse en paneles públicos de producción.
- CORS debe configurarse con orígenes explícitos.

## Criterio de aceptación
Antes de entregar: build, tests, login JWT, 201 al crear, persistencia PostgreSQL, filtros/orden/paginación, job Hangfire persistente, SMTP configurado, Swagger Bearer, rate limiting y secreto scan.
