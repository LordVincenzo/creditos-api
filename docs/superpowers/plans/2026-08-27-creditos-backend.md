# Credit API Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build a production-minded .NET 8 REST API for authenticated credit registration and querying with PostgreSQL, Hangfire email jobs, Swagger, validation, rate limiting, tests, and delivery documentation.

**Architecture:** One focused ASP.NET Core Web API project with explicit service boundaries around authentication, credit operations, persistence and email delivery. PostgreSQL is the system of record for users, credits, migrations and Hangfire jobs; controllers remain thin and all configuration comes from environment-compatible settings.

**Tech Stack:** .NET 8, ASP.NET Core Web API, EF Core/Npgsql, JWT Bearer, PasswordHasher, Hangfire.PostgreSql, MailKit, Swashbuckle, xUnit.

**Spec:** `docs/superpowers/specs/2026-08-27-creditos-app-design.md`

## Global Constraints
- Runtime target is .NET 8.
- PostgreSQL is mandatory for application persistence.
- Passwords are hashed with `PasswordHasher<User>` and never stored in plaintext.
- The authenticated commercial is always resolved from JWT, never accepted from credit-create input.
- Credit amount uses `numeric(18,2)` and identity document is text.
- Hangfire email delivery is asynchronous and persisted in PostgreSQL.
- `sortBy` is restricted to `createdAt` and `amount`; `sortDirection` is restricted to `asc` and `desc`.
- Development-only demo accounts are disabled outside Development.
- No production secrets are committed.

---

### Task 1: Repository foundation and configuration

**Files:**
- Create: `src/Creditos.Api/Creditos.Api.csproj`
- Create: `src/Creditos.Api/Program.cs`
- Create: `src/Creditos.Api/appsettings.json`
- Create: `src/Creditos.Api/appsettings.Development.json`
- Create: `src/Creditos.Api/Configuration/JwtOptions.cs`
- Create: `src/Creditos.Api/Configuration/SmtpOptions.cs`
- Create: `src/Creditos.Api/Configuration/CreditNotificationOptions.cs`
- Create: `src/Creditos.Api/Configuration/DemoUsersOptions.cs`
- Create: `src/Creditos.Api/Configuration/AppOptionsValidator.cs`
- Create: `tests/Creditos.Api.Tests/Creditos.Api.Tests.csproj`
- Create: `.gitignore`, `.env.example`, `Dockerfile`, `docker-compose.yml`

**Interfaces:**
- Produces strongly typed configuration records and a bootable API host.

- [ ] Write configuration validation tests asserting missing JWT key and invalid SMTP port are rejected.
- [ ] Run the focused test command and confirm RED because configuration classes do not exist yet.
- [ ] Add the API/test projects, package references, configuration records and minimal `Program` registration needed by the tests.
- [ ] Run focused tests and confirm GREEN.
- [ ] Add `.gitignore`, `.env.example`, Dockerfile and Compose services for API + PostgreSQL.
- [ ] Commit as `chore: initialize backend project`.

### Task 2: Persistence model and database initialization

**Files:**
- Create: `src/Creditos.Api/Entities/User.cs`
- Create: `src/Creditos.Api/Entities/Credit.cs`
- Create: `src/Creditos.Api/Data/AppDbContext.cs`
- Create: `src/Creditos.Api/Data/DevelopmentDataSeeder.cs`
- Create: `src/Creditos.Api/Data/Migrations/*`
- Create: `database/schema.sql`
- Test: `tests/Creditos.Api.Tests/Data/ModelConfigurationTests.cs`

**Interfaces:**
- Produces `AppDbContext.Users` and `AppDbContext.Credits` plus `DevelopmentDataSeeder.SeedAsync`.

- [ ] Write model tests for UUID keys, required fields, decimal precision, unique user email, FK relationship and required indexes.
- [ ] Run tests and confirm RED because entities/context are absent.
- [ ] Implement entities and EF model configuration with UTC timestamps and required indexes.
- [ ] Run tests and confirm GREEN.
- [ ] Implement Development-only demo-user seeding using `PasswordHasher<User>`.
- [ ] Generate EF migration and idempotent `database/schema.sql` when the .NET SDK is available; otherwise keep equivalent migration source and record the environment limitation.
- [ ] Commit as `feat: add postgres persistence model`.

### Task 3: JWT login

**Files:**
- Create: `src/Creditos.Api/DTOs/Auth/LoginRequest.cs`
- Create: `src/Creditos.Api/DTOs/Auth/LoginResponse.cs`
- Create: `src/Creditos.Api/Authentication/ICurrentUser.cs`
- Create: `src/Creditos.Api/Authentication/CurrentUser.cs`
- Create: `src/Creditos.Api/Services/IAuthService.cs`
- Create: `src/Creditos.Api/Services/AuthService.cs`
- Create: `src/Creditos.Api/Controllers/AuthController.cs`
- Test: `tests/Creditos.Api.Tests/Auth/AuthServiceTests.cs`

**Interfaces:**
- Produces `IAuthService.LoginAsync(email, password, cancellationToken)` returning token, expiry and public user data.
- JWT contains user id (`sub`), email and display name claims.

- [ ] Write failing tests for valid login, invalid password and inactive user.
- [ ] Run tests and confirm expected RED.
- [ ] Implement normalized email lookup, password hash verification and JWT creation.
- [ ] Run tests and confirm GREEN.
- [ ] Add `POST /api/auth/login` controller behavior with 200/401 validation responses.
- [ ] Commit as `feat: add jwt authentication`.

### Task 4: Credit registration

**Files:**
- Create: `src/Creditos.Api/DTOs/Credits/CreateCreditRequest.cs`
- Create: `src/Creditos.Api/DTOs/Credits/CreditResponse.cs`
- Create: `src/Creditos.Api/Validation/CreditValidation.cs`
- Create: `src/Creditos.Api/Services/ICreditService.cs`
- Create: `src/Creditos.Api/Services/CreditService.cs`
- Create: `src/Creditos.Api/Controllers/CreditsController.cs`
- Test: `tests/Creditos.Api.Tests/Credits/CreditCreationTests.cs`

**Interfaces:**
- Produces `ICreditService.CreateAsync(CreateCreditRequest, authenticatedUserId, cancellationToken)`.

- [ ] Write failing tests showing valid input persists a credit with the authenticated user's id/name snapshot and invalid values are rejected.
- [ ] Run focused tests and confirm RED.
- [ ] Implement validation and persistence using EF Core only; do not accept commercial id/name from request DTO.
- [ ] Run tests and confirm GREEN.
- [ ] Add authorized `POST /api/credits` returning 201 with `CreatedAtAction` or equivalent.
- [ ] Commit as `feat: implement credit registration`.

### Task 5: Credit query, filters, sorting and pagination

**Files:**
- Create: `src/Creditos.Api/DTOs/Credits/CreditQuery.cs`
- Create: `src/Creditos.Api/DTOs/Common/PagedResult.cs`
- Modify: `src/Creditos.Api/Services/ICreditService.cs`
- Modify: `src/Creditos.Api/Services/CreditService.cs`
- Modify: `src/Creditos.Api/Controllers/CreditsController.cs`
- Test: `tests/Creditos.Api.Tests/Credits/CreditQueryTests.cs`

**Interfaces:**
- Produces `ICreditService.QueryAsync(CreditQuery, cancellationToken)` returning `PagedResult<CreditResponse>`.

- [ ] Write failing tests for client-name filter, document filter, commercial filter, date sorting, amount sorting and page-size cap.
- [ ] Run focused tests and confirm RED.
- [ ] Implement `AsNoTracking`, trimmed text filters, whitelist sorting, count + page query and max page size 100.
- [ ] Run tests and confirm GREEN.
- [ ] Add authorized `GET /api/credits` query binding.
- [ ] Commit as `feat: add credit queries and filtering`.

### Task 6: Asynchronous credit email

**Files:**
- Create: `src/Creditos.Api/Services/IEmailService.cs`
- Create: `src/Creditos.Api/Services/SmtpEmailService.cs`
- Create: `src/Creditos.Api/Jobs/SendCreditRegisteredEmailJob.cs`
- Create: `src/Creditos.Api/Jobs/ICreditNotificationQueue.cs`
- Create: `src/Creditos.Api/Jobs/HangfireCreditNotificationQueue.cs`
- Modify: `src/Creditos.Api/Services/CreditService.cs`
- Test: `tests/Creditos.Api.Tests/Jobs/CreditEmailJobTests.cs`

**Interfaces:**
- Produces `IEmailService.SendAsync(...)`, `SendCreditRegisteredEmailJob.ExecuteAsync(Guid creditId)` and `ICreditNotificationQueue.Enqueue(Guid creditId)`.

- [ ] Write failing tests asserting the job loads persisted credit data, builds required content and that credit creation queues only after save succeeds.
- [ ] Run focused tests and confirm RED.
- [ ] Implement MailKit SMTP service and dedicated job with `[AutomaticRetry(Attempts = 3)]`.
- [ ] Configure Hangfire PostgreSQL storage and Development-only dashboard.
- [ ] Queue the job after credit persistence without awaiting SMTP.
- [ ] Run tests and confirm GREEN.
- [ ] Commit as `feat: add asynchronous credit email job`.

### Task 7: API hardening and OpenAPI

**Files:**
- Modify: `src/Creditos.Api/Program.cs`
- Create: `src/Creditos.Api/Middleware/GlobalExceptionHandler.cs`
- Create: `src/Creditos.Api/Extensions/ServiceCollectionExtensions.cs`
- Create: `src/Creditos.Api/Controllers/HealthController.cs`
- Test: `tests/Creditos.Api.Tests/Api/SecurityConfigurationTests.cs`

**Interfaces:**
- Produces JWT protection, ProblemDetails errors, named rate-limit policies, configured CORS, Swagger Bearer scheme and health endpoint.

- [ ] Write failing configuration/integration tests for unauthenticated credits 401, invalid credit 400, health response and safe default sort behavior.
- [ ] Run tests and confirm RED.
- [ ] Register JWT authentication/authorization, rate limiting for login + credit creation, ProblemDetails exception handling and configured CORS.
- [ ] Configure Swagger/OpenAPI Bearer auth and Development UI.
- [ ] Add `/api/health` with PostgreSQL health check registration.
- [ ] Run tests and confirm GREEN.
- [ ] Commit as `feat: harden api and add openapi`.

### Task 8: Delivery documentation and verification

**Files:**
- Create: `README.md`
- Create: `AGENTS.md`
- Modify: `.env.example`, `docker-compose.yml`, `database/schema.sql` as needed

**Interfaces:**
- Produces evaluator-ready setup and continuation instructions.

- [ ] Document local/Docker setup, variables, demo users, migrations/schema, Swagger, Gmail App Password SMTP, Hangfire, tests, deployment and troubleshooting.
- [ ] Document agent continuation rules, commands, security invariants and acceptance criteria in `AGENTS.md`.
- [ ] Run secret scan (`git grep`) and repository status review.
- [ ] Run `dotnet restore`, `dotnet build`, `dotnet test` if SDK is available; record exact blocker otherwise.
- [ ] Commit as `docs: add backend setup and handoff documentation`.
