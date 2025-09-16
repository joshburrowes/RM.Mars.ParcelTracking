# RM.Mars.ParcelTracking

A .NET 8 ASP.NET Core Web API for tracking parcels destined for Mars. This solution demonstrates layered architecture, business logic separation, validation, audit trail handling, and a simple document-backed data store for experimentation and learning.

## Setup Instructions

1. **Run the API:**
   - Open a terminal and navigate to the project folder:
     ```sh
     cd src/RM.Mars.ParcelTracking/RM.Mars.ParcelTracking
     ```
   - Restore dependencies (optional, `dotnet run` will auto-restore):
     ```sh
     dotnet restore
     ```
   - Start the API:
     ```sh
     dotnet run
     ```
   - The app runs on HTTPS at `https://localhost:<port>`. Swagger UI is available at `https://localhost:<port>/swagger`.

2. **Run Tests:**
   - From the solution or repository root:
     ```sh
     dotnet test
     ```

## Solution Walkthrough

- **Controllers:** Handle HTTP requests and map to service layer.
- **Services:** Business logic for parcel operations, validation, time calculations, and audit trail management.
- **Repositories:** Data access abstraction. Uses a document file (`Database/DocumentDb.json`) for persistence.
- **Models:** DTOs, request/response models, and domain enums.
- **Utils:** Helpers such as date/time provider abstraction for testability.
- **Extensions:** Common extension methods.

### Key Flows
- `POST /parcels` - register a parcel
- `GET /parcels/{barcode}` - retrieve parcel by barcode
- `PATCH /parcels/{barcode}` - update parcel status
- Swagger UI at `/swagger` for API exploration

## Design Choices & Trade-offs

- **Layered Architecture:** Controllers → Services → Repositories. Keeps responsibilities separate and business logic testable.
- **File-backed Persistence:** Uses a JSON document store for simplicity and ease of local development. Not suitable for concurrency, scaling, or production durability.
- **Abstractions for Testability:** `IDateTimeProvider` and `ITimeCalculatorService` allow deterministic time-based logic in tests.
- **Minimal Validation:** Request validation is pragmatic and short. A production system would use a richer validation library and centralized model validation.
- **No Authentication/Authorization:** Omitted for brevity. In production, add token-based auth, role checks, and tenant isolation.
- **Synchronous/Asynchronous Mix:** Some repository/service methods are synchronous for clarity. Real-world services should use async I/O throughout.

## What to Improve for Enterprise Scale

1. **Persistence & Data Integrity:**
   - Replace file-based store with Azure Cosmos DB for scalable, cloud-native document storage.
   - Add migrations, schema management, indexing, and retention policies using Azure tooling.
   - Ensure ACID semantics or adopt eventual-consistency patterns as supported by Cosmos DB.

2. **Concurrency, Resiliency & Performance:**
   - Make all I/O asynchronous.
   - Add retry/backoff, circuit breakers (Polly) for external dependencies.
   - Consider CQRS for separating reads and writes at scale.

3. **Observability & Operations:**
   - Add structured logging with Serilog and integrate with Azure Application Insights for distributed tracing and metrics.
   - Use Azure Monitor for dashboards and alerting.

4. **Security:**
   - Add authentication (OAuth2 / Azure Active Directory) and authorization policies.
   - Implement input sanitization, rate-limiting, and Azure Key Vault for secrets management.

5. **Testing & CI/CD:**
   - Add unit, integration, and contract tests.
   - Use Azure DevOps pipelines for build, test, container image build, and deployment.
   - Provide staging environments and automated canary/blue-green deployments in Azure.

6. **API Design & Contracts:**
   - Expand OpenAPI documentation, specify response codes, and add versioning for breaking changes.

7. **Scaling Architecture:**
   - Containerize the service and deploy on Azure Kubernetes Service (AKS) with autoscaling and service discovery.
   - Consider Azure Event Grid or Service Bus for event-driven architecture if business requires complex async workflows.

## Design Assumptions & Shortcuts

- Single project for simplicity.
- File-based persistence to avoid external dependencies.
- No authentication required.
- Some validations and edge-case handling are simplified for readability and speed.
- These shortcuts are for local exploration and demo purposes, not production.

## AI Tool Usage

This README and some developer-facing documentation were generated with assistance from an AI coding assistant: **GitHub Copilot**.
- AI was used for drafting documentation and suggesting improvement areas.
- Code in the repository was not modified by the assistant during README creation except for adding this file.
- Please confirm technical details against the codebase and runtime behavior.

---