# Credit Mobile Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build an Ionic React + TypeScript mobile client for login, credit registration and searchable/paginated credit consultation, configured for Capacitor Android.

**Architecture:** A Vite/Ionic single-page app with an auth context, centralized API client and focused pages. Token persistence uses Capacitor Preferences; the authenticated shell provides bottom tabs and logout, while all server-authoritative behavior remains in the API.

**Tech Stack:** Ionic React, React, TypeScript, Vite, Capacitor, Capacitor Preferences, Vitest, React Testing Library.

**Spec:** `../creditos-api/docs/superpowers/specs/2026-08-27-creditos-app-design.md`

## Global Constraints
- App name is `Creditos` and package id is `com.creditos.app`.
- API base URL comes from `VITE_API_URL`.
- JWT is never written to logs.
- Commercial identity is displayed read-only from authenticated user data and is not submitted as authoritative input.
- Credits list includes a real HTML table with horizontal scrolling on small screens.
- Backend pagination is used; the app does not load all records.
- Release API is expected to use HTTPS.

---

### Task 1: Repository and testable app foundation

**Files:**
- Create: `creditos-mobile/package.json`, `vite.config.ts`, `tsconfig.json`, `index.html`
- Create: `creditos-mobile/src/main.tsx`, `src/App.tsx`, `src/theme/*`
- Create: `creditos-mobile/.gitignore`, `.env.example`
- Create: `creditos-mobile/src/test/setup.ts`

- [ ] Scaffold Ionic React TypeScript/Vite repository with Vitest and Testing Library.
- [ ] Add one boot smoke test and run it RED before implementing app shell.
- [ ] Implement minimal Ionic app shell and run the test GREEN.
- [ ] Initialize independent Git repository and commit `chore: initialize ionic application`.

### Task 2: Validation and auth state

**Files:**
- Create: `src/models/auth.ts`, `src/models/credit.ts`
- Create: `src/services/api.ts`, `src/services/authStorage.ts`
- Create: `src/context/AuthContext.tsx`
- Create: `src/validation/creditValidation.ts`
- Test: `src/validation/creditValidation.test.ts`, `src/context/AuthContext.test.tsx`

- [ ] Write failing tests for credit form validation and persisted auth restoration/logout.
- [ ] Run focused tests and confirm RED.
- [ ] Implement validation, Preferences-backed token storage, centralized fetch client and auth context.
- [ ] Run tests and confirm GREEN.
- [ ] Commit `feat: add authentication state and validation`.

### Task 3: Login page and protected shell

**Files:**
- Create: `src/pages/LoginPage.tsx`
- Create: `src/components/AuthenticatedTabs.tsx`
- Modify: `src/App.tsx`
- Test: `src/pages/LoginPage.test.tsx`

- [ ] Write failing UI tests for required fields, loading-disabled button and failed login message.
- [ ] Run tests RED.
- [ ] Implement Ionic login form, auth request and navigation to authenticated tabs.
- [ ] Run tests GREEN.
- [ ] Commit `feat: add authentication flow`.

### Task 4: Credit registration page

**Files:**
- Create: `src/pages/RegisterCreditPage.tsx`
- Create: `src/services/credits.ts`
- Test: `src/pages/RegisterCreditPage.test.tsx`

- [ ] Write failing tests for read-only commercial, client-side validation and double-submit prevention.
- [ ] Run tests RED.
- [ ] Implement form with Ionic inputs, loading/toast states, numeric normalization and success reset.
- [ ] Run tests GREEN.
- [ ] Commit `feat: add credit registration form`.

### Task 5: Credits query table and pagination

**Files:**
- Create: `src/pages/CreditsQueryPage.tsx`
- Create: `src/components/CreditsTable.tsx`
- Create: `src/components/PaginationControls.tsx`
- Test: `src/pages/CreditsQueryPage.test.tsx`

- [ ] Write failing tests for filters, sort parameters, real table rendering, empty state and next/previous pagination.
- [ ] Run tests RED.
- [ ] Implement query form, credits service request, money/date formatting, scrollable table and pagination controls.
- [ ] Run tests GREEN.
- [ ] Commit `feat: add credits query screen`.

### Task 6: Capacitor Android

**Files:**
- Create: `capacitor.config.ts`
- Generate: `android/`
- Modify Android networking only as needed for debug emulator use.

- [ ] Add Capacitor Android dependencies and configuration for `com.creditos.app` / `Creditos`.
- [ ] Run `npm run build` and confirm success before sync.
- [ ] Run `npx cap add android` / `npx cap sync android` if Android SDK tooling allows.
- [ ] Attempt Gradle APK/AAB generation and copy real artifacts to `release/`; never fabricate results.
- [ ] Commit `chore: configure android build`.

### Task 7: Documentation and final verification

**Files:**
- Create: `README.md`, `AGENTS.md`

- [ ] Document install, `VITE_API_URL`, browser, Android emulator `10.0.2.2`, Capacitor sync, Android Studio, APK/AAB commands, demo credentials and troubleshooting.
- [ ] Document structure, commands, security invariants, testing and acceptance criteria in `AGENTS.md`.
- [ ] Run `npm install`, `npm run build`, `npm test -- --run` and lint if configured.
- [ ] Run secret scan and Git status review.
- [ ] Commit `docs: add mobile setup and handoff documentation`.
