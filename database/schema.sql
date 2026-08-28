-- Application schema equivalent to the InitialCreate EF Core migration.
-- Hangfire creates and manages its own PostgreSQL schema at runtime.
BEGIN;

CREATE TABLE IF NOT EXISTS users (
    "Id" uuid PRIMARY KEY,
    "Email" varchar(320) NOT NULL,
    "PasswordHash" varchar(500) NOT NULL,
    "DisplayName" varchar(120) NOT NULL,
    "IsActive" boolean NOT NULL,
    "CreatedAtUtc" timestamp with time zone NOT NULL
);
CREATE UNIQUE INDEX IF NOT EXISTS "IX_users_Email" ON users ("Email");

CREATE TABLE IF NOT EXISTS credits (
    "Id" uuid PRIMARY KEY,
    "ClientName" varchar(150) NOT NULL,
    "ClientDocument" varchar(50) NOT NULL,
    "Amount" numeric(18,2) NOT NULL,
    "InterestRate" numeric(5,2) NOT NULL,
    "TermMonths" integer NOT NULL,
    "RegisteredByUserId" uuid NOT NULL,
    "CommercialNameSnapshot" varchar(120) NOT NULL,
    "CreatedAtUtc" timestamp with time zone NOT NULL,
    CONSTRAINT "FK_credits_users_RegisteredByUserId"
        FOREIGN KEY ("RegisteredByUserId") REFERENCES users ("Id") ON DELETE RESTRICT
);
CREATE INDEX IF NOT EXISTS "IX_credits_ClientName" ON credits ("ClientName");
CREATE INDEX IF NOT EXISTS "IX_credits_ClientDocument" ON credits ("ClientDocument");
CREATE INDEX IF NOT EXISTS "IX_credits_RegisteredByUserId" ON credits ("RegisteredByUserId");
CREATE INDEX IF NOT EXISTS "IX_credits_CreatedAtUtc" ON credits ("CreatedAtUtc");
CREATE INDEX IF NOT EXISTS "IX_credits_Amount" ON credits ("Amount");

COMMIT;
