# HomeBase Roadmap Checklist

Progress tracker for **HomeBase**: a self-hosted Command Center + Personal Hub built with C#/.NET, React, and PostgreSQL.

> Pace: ~10 hours/week, about 23 weeks for Phases 0-9.
> Tip: GitHub renders these checkboxes and shows a progress bar. In an issue, hover a checklist item and use **Convert to issue** to turn it into a trackable task.

**Overall progress**

- [ ] Phase 0: Foundation
- [ ] Phase 1: Backend Core
- [ ] Phase 2: Authentication and Security
- [ ] Phase 3: Frontend Foundation
- [ ] Phase 4: Metrics Collector
- [ ] Phase 5: Dashboard and Real-Time
- [ ] Phase 6: Uptime Checks and Alerting
- [ ] Phase 7: Testing and Quality
- [ ] Phase 8: DevOps and Deployment
- [ ] Phase 9: Polish and Documentation
- [ ] Phase 10: External Access (later)

---

## Phase 0: Foundation (Week 1)

**Dev environment**
- [ ] Install .NET 10 SDK (`dotnet --version` works)
- [ ] Install Node.js LTS, Git, Docker, and editor (VS Code + C# Dev Kit, or Rider)
- [ ] Configure Git name/email and add SSH key to GitHub

**Repository**
- [ ] Create `homebase` repo with README, license, .NET + Node `.gitignore`
- [ ] Create folders: `src/`, `tests/`, `web/`, `deploy/`, `docs/`
- [ ] Create GitHub Project board (Backlog, In Progress, Review, Done)
- [ ] Create a milestone per phase and add these items as issues

**Server hardening**
- [ ] Reserve a fixed LAN IP for the server
- [ ] SSH key-only login (`PasswordAuthentication no`), tested from a second session
- [ ] Enable `ufw`: deny incoming, allow SSH and app port from LAN only
- [ ] Enable automatic security updates
- [ ] Confirm Docker and Portainer are healthy
- [ ] Note disk layout and free space for backups

**C# practice**
- [ ] Complete first modules of Microsoft Learn C# path
- [ ] Build console to-do app (classes, interfaces, `List<T>`, LINQ, JSON file I/O, `async/await`)
- [ ] Run the app on the Linux server with `dotnet run`

**Done when**
- [ ] Key-only SSH + firewall active; repo and board exist; console app committed

---

## Phase 1: Backend Core (Weeks 2-4)

**Solution setup**
- [ ] Create solution and projects (Api, Core, Infrastructure, Worker) with references
- [ ] Write ADR explaining layered/clean architecture choice
- [ ] Compose file in `deploy/` running PostgreSQL with a named volume
- [ ] Add Npgsql EF Core provider and `AppDbContext`

**Data model**
- [ ] Design `TaskItem`, `Note`, `Bookmark`, `Tag` (many-to-many) with timestamps
- [ ] Draw ER diagram (Mermaid) in README
- [ ] Configure relationships (Fluent API) and indexes
- [ ] Create and apply first EF Core migration

**Endpoints**
- [ ] CRUD for tasks
- [ ] CRUD for notes
- [ ] CRUD for bookmarks
- [ ] Use DTOs (no entities exposed)
- [ ] Request validation + RFC 7807 `ProblemDetails` errors
- [ ] Pagination, sorting, filtering, text search on list endpoints

**Cross-cutting**
- [ ] Global exception-handling middleware
- [ ] Serilog structured logging
- [ ] OpenAPI docs with Scalar or Swagger UI
- [ ] `/health` endpoint checking the database
- [ ] Development data seeder

**Done when**
- [ ] All Hub resources fully usable via the API docs page

---

## Phase 2: Authentication and Security (Weeks 5-6)

**Identity**
- [ ] ASP.NET Core Identity with EF Core + migration
- [ ] Register, login, logout, refresh endpoints
- [ ] Short-lived JWT access tokens; hashed refresh tokens with rotation and revocation
- [ ] Roles `Admin` and `Viewer`; admin seeded from environment variables
- [ ] `[Authorize]` and role policies; every item owned by a user and enforced in queries

**Hardening**
- [ ] Password policy and account lockout
- [ ] Rate limiting (strict on login/refresh)
- [ ] CORS limited to the frontend origin
- [ ] Security headers (CSP, X-Content-Type-Options, Referrer-Policy, frame protection)
- [ ] Input size limits and validation everywhere
- [ ] Secrets out of source control (user-secrets in dev)

**Study and document**
- [ ] Read OWASP Top 10
- [ ] Write `docs/security.md` mapping each item to your app (with known gaps)
- [ ] List authorization tests (no cross-user access)

**Done when**
- [ ] 401/403 behave correctly and cross-user access is impossible

---

## Phase 3: Frontend Foundation (Weeks 7-9)

**Setup**
- [ ] Scaffold Vite + React + TypeScript in `web/`
- [ ] Add React Router, TanStack Query, react-hook-form + zod
- [ ] Choose and apply a styling approach
- [ ] Generate typed API client from OpenAPI
- [ ] Configure Vite dev proxy to the API

**Auth flow**
- [ ] Login and register pages with validation and error messages
- [ ] Auth context with in-memory token and silent refresh
- [ ] Protected routes and role-aware navigation
- [ ] Global 401 handling (refresh, then redirect)

**Hub UI**
- [ ] App shell: sidebar, header, light/dark toggle
- [ ] Tasks page (list, filters, sort, create/edit, status toggle)
- [ ] Notes page (editor + sanitized Markdown rendering)
- [ ] Bookmarks page (add by URL, tag filtering)
- [ ] Global search and tag management
- [ ] Loading skeletons, empty states, error boundaries

**Quality**
- [ ] Test at phone, tablet, desktop widths
- [ ] Lighthouse accessibility fixes (labels, focus, contrast, keyboard)
- [ ] ESLint + Prettier clean

**Done when**
- [ ] Full Hub usable end to end on desktop and phone

---

## Phase 4: Metrics Collector (Weeks 10-12)

**Collector design**
- [ ] `IMetricCollector` interface, one collector per resource
- [ ] `BackgroundService` loop (10s) with cancellation and per-collector error handling
- [ ] Mount host `/proc` and `/sys` read-only; paths configurable

**Collectors**
- [ ] CPU from `/proc/stat` deltas + load average
- [ ] Memory from `/proc/meminfo` (available memory)
- [ ] Disk usage per mount + I/O rates from `/proc/diskstats`
- [ ] Network rates from `/proc/net/dev`
- [ ] Temperature and uptime (handle missing sensors)

**Storage**
- [ ] `MetricSample` table + composite index (metric, timestamp)
- [ ] Batched inserts
- [ ] Hourly rollup table + retention job
- [ ] `EXPLAIN ANALYZE` before/after index comparison noted in docs

**Docker visibility**
- [ ] Docker socket proxy container (read-only, containers endpoint only)
- [ ] Docker.DotNet client lists containers with state, health, usage
- [ ] Store container state-change events
- [ ] Write `docs/docker-access.md` (why direct socket access is risky)

**Done when**
- [ ] Hours of gap-free metrics stored; container list matches Portainer

---

## Phase 5: Dashboard and Real-Time (Weeks 13-15)

**Query API**
- [ ] Endpoints: current values, time series by range, container list
- [ ] Auto-select raw vs. rollup data by range
- [ ] Auth on all endpoints; admin-only settings

**Real-time**
- [ ] SignalR hub broadcasting latest samples
- [ ] JWT auth for the hub connection
- [ ] ADR on how worker data reaches the API
- [ ] React SignalR client with auto-reconnect and status indicator

**Dashboard UI**
- [ ] Overview tiles/gauges (CPU, memory, disk, temp, uptime, network)
- [ ] Charts with time-range selector and live updates
- [ ] Container board with status and recent events
- [ ] Handle no data, stale data, and API errors
- [ ] Phone layout and full-screen kiosk view

**Done when**
- [ ] Live updates work; stopping the worker visibly marks data stale

---

## Phase 6: Uptime Checks and Alerting (Weeks 16-17)

**Service checks**
- [ ] `ServiceCheck` and `CheckResult` models
- [ ] HTTP, TCP, and ping/TCP checkers behind one interface
- [ ] Scheduled checks with timeouts via `IHttpClientFactory`
- [ ] Services page: status tiles, latency chart, 24h strip

**Alert engine**
- [ ] `AlertRule` model
- [ ] State machine: OK, Pending, Firing, Resolved
- [ ] Notify only on transitions; enforce cooldowns
- [ ] `INotifier` with Discord webhook, then email
- [ ] Resolved notifications
- [ ] UI: rule editor, active alerts, history, acknowledge, test notification
- [ ] Default rules (disk, memory, repeated check failures, unhealthy container)

**Done when**
- [ ] Stopping a container gives one alert and one resolved message

---

## Phase 7: Testing and Quality (Weeks 18-19)

**Backend**
- [ ] xUnit unit tests (alert state machine, CPU calc, retention, validators) using fixture text
- [ ] Integration tests with `WebApplicationFactory` + Testcontainers PostgreSQL
- [ ] Authorization tests (ownership and roles)
- [ ] Coverage via coverlet with a realistic target

**Frontend**
- [ ] Vitest + React Testing Library behavior tests
- [ ] MSW for API mocking

**End to end**
- [ ] Playwright: login, create task, view dashboard tile
- [ ] Run E2E against a Compose test stack

**Automation**
- [ ] GitHub Actions: build and test backend; lint, type-check, test frontend
- [ ] README status badges
- [ ] Branch protection requiring passing checks
- [ ] `dotnet format`, ESLint, and Dependabot enabled

**Done when**
- [ ] Failing tests block merges

---

## Phase 8: DevOps and Deployment (Weeks 20-21)

**Containers**
- [ ] Multi-stage, non-root Dockerfiles for API, worker, frontend
- [ ] Caddy (or Nginx) serving frontend and proxying API/SignalR
- [ ] Production Compose: API, worker, Postgres, Caddy, socket proxy, backup container
- [ ] Health checks, `depends_on`, Postgres on internal network only
- [ ] Secrets handled via env/Docker secrets; `.env.example` documented
- [ ] Decide and document migration strategy

**Pipeline and deploy**
- [ ] Build and push images to GHCR on merge to `main` (SHA + `latest` tags)
- [ ] Deploy as a Portainer stack
- [ ] Document update and rollback procedure

**Backups**
- [ ] Nightly `pg_dump` to a second disk/machine with retention
- [ ] **Restore into a clean database and verify data**
- [ ] Write `docs/runbook.md`

**Done when**
- [ ] One command brings up the stack; a backup has been restored successfully

---

## Phase 9: Polish and Documentation (Weeks 22-23)

**Operational quality**
- [ ] Readiness and liveness endpoints
- [ ] Structured logs with correlation IDs (optional OpenTelemetry/Prometheus)
- [ ] Caching where measurements justify it
- [ ] Slow-query review and missing indexes
- [ ] Security pass: dependency audits, image scan, re-read `docs/security.md`

**Demo mode**
- [ ] Seeded demo dataset
- [ ] Read-only demo account
- [ ] Visible "demo" banner

**Documentation**
- [ ] README: purpose, features, screenshots, architecture, trade-offs, setup, testing
- [ ] Gather ADRs, security doc, alert design doc, runbook in `docs/`
- [ ] 2-minute demo video in README
- [ ] Blog post on the hardest thing you learned
- [ ] Tag `v1.0.0` release with notes

**Done when**
- [ ] A newcomer understands and runs the project from the README

---

## Phase 10: External Access (Later)

Choose one approach (safest first):
- [ ] Option A: Tailscale/WireGuard (no open ports)
- [ ] Option B: Cloudflare Tunnel for a public demo
- [ ] Option C: Port forwarding (443 only) with HTTPS, dynamic DNS, fail2ban, separate demo instance

**Pre-flight checklist**
- [ ] `docs/security.md` reviewed; dependency scan clean
- [ ] Database and admin tools unreachable from outside Docker network
- [ ] Registration disabled or invite-only; strong unique admin password (2FA if added)
- [ ] Rate limiting and lockout verified by testing
- [ ] Backup restored within the last month
- [ ] Failed-login monitoring and alert in place
- [ ] Router firmware updated; UPnP disabled
- [ ] Public demo uses a separate instance with seeded data only

---

## Stretch Goals

- [ ] Multi-host monitoring agent
- [ ] Two-factor authentication (TOTP)
- [ ] PWA install and push notifications
- [ ] Bookmark browser extension
- [ ] Prometheus export + Grafana comparison
- [ ] MAUI / React Native companion app
- [ ] PostgreSQL full-text search for notes and bookmarks
- [ ] Ansible to rebuild the server from scratch

---

## Phase Notes Log

After each phase, add a short note in `docs/learning/` (what you built, what was hard, what you would change).

- [ ] Phase 0 note
- [ ] Phase 1 note
- [ ] Phase 2 note
- [ ] Phase 3 note
- [ ] Phase 4 note
- [ ] Phase 5 note
- [ ] Phase 6 note
- [ ] Phase 7 note
- [ ] Phase 8 note
- [ ] Phase 9 note
