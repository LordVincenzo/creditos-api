# Creditos App - Design

## Scope
Build two independent repositories:
- `creditos-api`: .NET 8 Web API + PostgreSQL.
- `creditos-mobile`: Ionic React + TypeScript + Capacitor.

Only the requested credit registration/query flow is included. No password recovery, public signup, credit editing/deletion, payment schedules, complex roles, or microservices.

## Backend architecture
Keep a modular single API project with focused folders: Controllers, Data, Entities, DTOs, Services, Jobs, Authentication, Validation, Middleware, Extensions, Configuration, plus `tests/` and `database/`.

Main dependencies:
- ASP.NET Core Web API on .NET 8.
- EF Core + Npgsql for PostgreSQL.
- JWT Bearer authentication.
- `PasswordHasher<TUser>` for passwords.
- Hangfire with PostgreSQL storage.
- MailKit for SMTP.
- Swagger/OpenAPI with Bearer authorization.
- Built-in ASP.NET Core rate limiting and ProblemDetails/exception handling.

## Data model
### User
- Id: UUID
- Email: varchar, unique
- PasswordHash: varchar
- DisplayName: varchar
- IsActive: bool
- CreatedAtUtc: timestamptz

Development-only seeding creates two demo commercial users. Production never seeds demo credentials.

### Credit
- Id: UUID
- ClientName: varchar
- ClientDocument: varchar
- Amount: numeric(18,2)
- InterestRate: numeric(5,2)
- TermMonths: integer
- RegisteredByUserId: UUID FK -> User
- CommercialNameSnapshot: varchar
- CreatedAtUtc: timestamptz

Indexes support client name/document, registered user, created date and amount.

## API flow
### POST /api/auth/login
Validate email/password, verify hash, issue JWT containing user id, email and display name. Apply login rate limit.

### POST /api/credits
Requires JWT. Validate input, resolve the authenticated user from JWT, persist credit, enqueue email job after persistence, return 201 without waiting for SMTP. The client cannot choose the commercial identity.

### GET /api/credits
Requires JWT. Supports clientName, clientDocument, commercial, sortBy (`createdAt`, `amount`), sortDirection (`asc`, `desc`), page and pageSize. Reads use `AsNoTracking`; pageSize is capped at 100.

### GET /api/health
Reports API health and, when configured, PostgreSQL connectivity.

## Async email
Hangfire uses PostgreSQL persistence. After a credit is saved, a dedicated job loads the credit and sends an HTML/text email through an email service. SMTP settings and recipient come from environment variables. Retry count is capped at 3. SMTP failure must never roll back the credit.

## Security and errors
- No plaintext passwords or committed secrets.
- EF Core parameterized queries only.
- Sort fields use a whitelist.
- Global ProblemDetails-based error responses.
- Rate limit on login and credit creation.
- CORS reads allowed origins from configuration.
- Hangfire dashboard is Development-only.
- Swagger exposes JWT Bearer authorization.

## Mobile architecture
Ionic React + TypeScript + Capacitor with pages for Login, Register Credit and Credits Query. A centralized API service applies the Bearer token and handles global 401 logout. Auth state owns the signed-in user and token.

Token persistence uses Capacitor Preferences for this technical test. No token is written to logs.

### UX
- Bottom tabs for Register and Query; logout available from the authenticated shell.
- Register form validates client, document, amount, interest and term; commercial is read-only from the authenticated user.
- Query screen has filters, sort controls, a real horizontally scrollable table, and backend pagination.
- Loading, toast, validation, empty and error states use Ionic components.

## Android
Capacitor package id: `com.creditos.app`; app name: `Creditos`. Development documentation covers Android emulator access to a local API through `10.0.2.2`. Release expects HTTPS.

## Testing
Backend tests cover valid/invalid login, unauthorized protected access, valid/invalid credit creation, list, filters, sorting and pagination. Prefer PostgreSQL integration testing where the environment supports it; otherwise keep test seams explicit and document the limitation.

Frontend tests cover validation, auth state and important data handling. Build, tests, Capacitor sync, and Android build are all attempted and only reported successful if actually executed.

## Delivery
Both repos contain their own `.git`, `.gitignore`, `README.md`, `AGENTS.md` and `.env.example`/equivalent. Backend also contains EF migrations, `database/schema.sql`, Dockerfile and docker-compose configuration.

No GitHub repository URLs are invented. If `gh` is unavailable or unauthenticated, local Git histories are prepared and exact publication commands are documented.

## Environment findings at design time
- Node.js: available (v22.16.0)
- npm: available (10.9.2)
- Java: available (OpenJDK 21)
- Git: available (2.47.3)
- .NET SDK: not installed
- Docker: not installed
- PostgreSQL CLI: not installed
- GitHub CLI: not installed
- Android SDK environment: not configured

These missing tools are environment limitations, not scope changes. Implementation should continue as far as possible and clearly separate verified results from unverified ones.
