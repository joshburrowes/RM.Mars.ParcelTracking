# RM.Mars.ParcelTracking

A small .NET 8 microservice-style sample for tracking parcels destined for Mars. This repository demonstrates layered design (controllers, services, repositories), simple validation and audit trail handling, and a minimal in-memory/document-backed data store for quick experimentation.

## Requirements

- .NET SDK 8.x installed (verify with `dotnet --version`)
- Optional: an IDE that supports .NET (Visual Studio, VS Code)

## Quick start

1. Open a terminal and change to the project folder:

   `cd src/RM.Mars.ParcelTracking/RM.Mars.ParcelTracking`

2. Restore (optional, `dotnet run` will auto-restore):

   `dotnet restore`

3. Run the API:

   `dotnet run`

   By default the app runs on HTTPS at `https://localhost:7151` (this is the value used in the provided `RM.Mars.ParcelTracking.http` sample file). Swagger UI will be available at `https://localhost:7151/swagger`.

4. Run tests (if any test projects are present):

   From the repository root or solution folder run:

   `dotnet test`

   Note: this solution intentionally focuses on the API implementation. If no test projects exist, `dotnet test` will report that there are no test projects to run.

## Sample API flows

A small `RM.Mars.ParcelTracking.http` file is included at the project root to exercise the API. Typical endpoints:

- `POST /parcels` - create a parcel
- `GET /parcels/{barcode}` - retrieve parcel by barcode
- `PATCH /parcels/{barcode}` - update parcel status

Headers: use `Content-Type: application/json` for write operations.

## Project layout and walkthrough

Key folders and responsibilities (map to files in this project):

- `Controllers` - API surface. Handles HTTP requests and maps to service layer.
- `Services` - Business logic. Contains parcel operations, validation, time calculations and audit trail management.
- `Repositories` - Data access abstraction. The sample uses a document file (`Database/DocumentDb.json`) as a lightweight persistence store for development/demo purposes.
- `Models` - DTOs, request/response models and domain enums.
- `Utils` - small helpers such as date/time provider abstraction for testability.
- `Extensions` - common extension methods used across the codebase.

Important services/abstractions to review:

- `IParcelService` / `ParcelService` - core parcel lifecycle handling.
- `IParcelsRepository` / `ParcelsRepository` - repository layer; currently wired to a simple document store.
- `IParcelRequestValidation` / `ParcelRequestValidation` - request validation and business rules.
- `ITimeCalculatorService` / `TimeCalculatorService` - expected delivery calculations.
- `IAuditTrailService` / `AuditTrailService` - record status changes and history.

Files to inspect for quick orientation:

- `Program.cs` - application bootstrap and DI registration
- `appsettings.json` - configuration values
- `RM.Mars.ParcelTracking.http` - example HTTP requests you can run with REST Client extensions or copy into Postman

## Design choices and trade-offs

- Layered architecture: Controllers -> Services -> Repositories. This keeps responsibilities separate, makes the business logic testable, and keeps controllers thin.

- Lightweight persistence: a file-backed document store (`DocumentDb.json`) is used to keep the demo self-contained and easy to run. Trade-off: not suitable for concurrency, scaling, transactional guarantees, or production durability.

- Abstractions for testability: `IDateTimeProvider` and `ITimeCalculatorService` enable deterministic behavior for time-based logic in tests. Trade-off: slightly more code and indirection but valuable for unit testing.

- Limited scope validation: provided request validation is pragmatic and short; a production system would use a richer validation library (e.g., FluentValidation) and centralized model validation.

- Synchronous vs. asynchronous: some repository/service methods may be simplified to synchronous calls for clarity. For real-world scalable services, use asynchronous I/O throughout.

- No authentication/authorization: omitted to keep the sample small. In production, add token-based auth, role checks and per-tenant isolation as required.

## What to improve for enterprise scale (recommended roadmap)

1. Persistence and data integrity
   - Replace the file-based document store with a production-grade DB (Cosmos DB, PostgreSQL, SQL Server, or other) tailored to scale needs.
   - Add migrations, schema management, indexing and retention policies.
   - Ensure ACID semantics where required or adopt eventual-consistency patterns explicitly.

2. Concurrency, resiliency and performance
   - Make all I/O asynchronous (`async/await`) and use high-performance connection pooling.
   - Add retry/backoff, circuit breakers (Polly) for external dependencies.
   - Consider CQRS for separating reads and writes at scale.

3. Observability and operations
   - Add structured logging (Serilog), distributed tracing (OpenTelemetry), metrics (Prometheus) and health checks.
   - Provide dashboards and alerting.

4. Security
   - Add authentication (OAuth2 / OpenID Connect) and authorization policies.
   - Implement input sanitization, rate-limiting, and secrets management.

5. Testing and CI/CD
   - Add unit, integration, and contract tests.
   - Add pipeline for build, test, container image build and deployment (GitHub Actions/Azure DevOps).
   - Provide staging environments and automated canary/blue-green deployments.

6. API design and contracts
   - Expand OpenAPI documentation, specify response codes clearly, and add versioning for breaking changes.

7. Scaling architecture
   - Containerize the service and deploy on orchestrators (Kubernetes) with autoscaling and service discovery.
   - Consider event-driven architecture (event sourcing, message bus) if business requires complex async workflows.

## Assumptions and shortcuts taken

- Uses a single project for the example API to keep the sample small and focused.
- Persistence uses a JSON document file to avoid external dependencies and to make running the sample easier.
- Authentication and multi-tenant isolation are intentionally omitted.
- Some validations and edge-case handling are simplified for readability and speed of delivery.

These shortcuts were chosen to make the codebase straightforward to explore and run locally. They are not recommendations for production without reworking.

## How to contribute

- Fork the repository and open a pull request with a clear description of changes.
- Add tests for any business logic changes and ensure `dotnet test` stays green.
- Follow semantic commit messages and keep changes small and focused.

## AI assistance disclosure

This README (and potentially other developer-facing text in the repo) was generated with assistance from an AI coding assistant: `GitHub Copilot`.

AI usage was limited to drafting documentation text and suggesting improvement areas. Code in the repository was not modified by the assistant during this README creation step except for adding this file.


