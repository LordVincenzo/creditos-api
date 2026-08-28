# Creditos API

REST API de la prueba técnica para autenticar comerciales, registrar créditos, consultarlos y enviar la notificación por correo de forma asíncrona.

## Stack

- .NET 8 / ASP.NET Core Web API
- PostgreSQL + Entity Framework Core
- JWT Bearer + `PasswordHasher<User>`
- Hangfire con PostgreSQL
- MailKit / SMTP
- Swagger / OpenAPI
- xUnit

## Flujo principal

1. `POST /api/auth/login` valida correo y contraseña y entrega JWT.
2. `POST /api/credits` toma el comercial del JWT, persiste el crédito y encola el correo.
3. Hangfire procesa el correo fuera del request HTTP.
4. `GET /api/credits` permite filtros, orden, paginación y máximo 100 filas por página.

## Requisitos

- .NET SDK 8
- PostgreSQL 14+ (recomendado 16)
- Opcional: Docker + Docker Compose

## Configuración

Copie `.env.example` a su gestor local de variables. ASP.NET Core usa nombres como `Jwt__Key` y `ConnectionStrings__DefaultConnection`.

Variables importantes:

- `ConnectionStrings__DefaultConnection`
- `Jwt__Issuer`, `Jwt__Audience`, `Jwt__Key`, `Jwt__ExpirationMinutes`
- `Smtp__Host`, `Smtp__Port`, `Smtp__Username`, `Smtp__Password`
- `Smtp__FromEmail`, `Smtp__FromName`, `Smtp__UseSsl`, `Smtp__UseStartTls`
- `CreditNotification__RecipientEmail` (por defecto `creditos@gmail.com`)
- `AllowedOrigins`
- `DemoUsers__Enabled`, `DemoUsers__Password`

Nunca use en producción la clave JWT de ejemplo ni las credenciales demo.

## Usuarios demo (solo Development)

Cuando `DemoUsers__Enabled=true`:

- `comercial1@demo.local` / `Demo1234!`
- `comercial2@demo.local` / `Demo1234!`

La siembra está bloqueada fuera de `Development`.

## Ejecutar con Docker

```bash
docker compose up --build
```

Servicios:

- API: `http://localhost:8080`
- Swagger: `http://localhost:8080/swagger`
- MailHog: `http://localhost:8025`
- PostgreSQL: `localhost:5432`

MailHog solo se usa para desarrollo. La configuración real sigue siendo SMTP estándar.

## Ejecutar sin Docker

Configure PostgreSQL y las variables de entorno, luego:

```bash
dotnet restore
dotnet ef database update --project src/Creditos.Api
dotnet run --project src/Creditos.Api
```

En Development la API también aplica migraciones al arrancar.

## Base de datos

Migración inicial: `src/Creditos.Api/Data/Migrations/20260828000000_InitialCreate.cs`.

Script entregable: `database/schema.sql`.

Para regenerar un script idempotente con EF:

```bash
dotnet tool install --global dotnet-ef --version 8.*
dotnet ef migrations script --idempotent --project src/Creditos.Api --output database/schema.sql
```

## Endpoints

- `POST /api/auth/login`
- `POST /api/credits` (JWT)
- `GET /api/credits` (JWT)
- `GET /api/health`

Consulta de ejemplo:

```text
/api/credits?clientName=pepito&clientDocument=123&commercial=juan&sortBy=createdAt&sortDirection=desc&page=1&pageSize=20
```

`sortBy`: `createdAt` o `amount`. Cualquier otro valor cae al orden seguro por fecha. `pageSize` se limita a 100.

## Swagger y JWT

En Development abra `/swagger`, ejecute login, copie `accessToken`, pulse **Authorize** y pegue el token. Los endpoints de créditos requieren JWT válido.

## SMTP Gmail

Use una **App Password** de Google; no use la contraseña normal de Gmail. Configure host `smtp.gmail.com`, puerto `587`, StartTLS, usuario, App Password y correo remitente mediante variables de entorno.

El correo incluye cliente, valor, comercial y fecha. Hangfire reintenta hasta 3 veces. Una falla SMTP no elimina ni revierte el crédito ya guardado.

## Hangfire

Los jobs se almacenan en PostgreSQL. Dashboard disponible solo en Development en `/hangfire`.

## Rate limiting

- Login: 10 solicitudes/minuto.
- Crear crédito: 30 solicitudes/minuto.
- Exceso: HTTP 429.

## Tests

```bash
dotnet test
```

La suite cubre autenticación, modelo, creación de créditos, filtros, ordenamientos, paginación, contenido de correo y protección JWT.

## Producción

- Use PostgreSQL administrado y HTTPS.
- Inyecte secretos desde el proveedor de despliegue.
- Use una clave JWT larga y aleatoria.
- Configure CORS únicamente para los orígenes reales.
- Desactive usuarios demo.
- No exponga Hangfire Dashboard.

## Problemas comunes

- `401`: token ausente/expirado o usuario inválido.
- `429`: se superó el límite de solicitudes.
- API no inicia: revise PostgreSQL, `ConnectionStrings__DefaultConnection` y `Jwt__Key`.
- Correo falla: revise SMTP/App Password; el crédito permanece persistido y Hangfire conserva el fallo/reintento.
