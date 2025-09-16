# RM.Mars.ParcelTracking

A .NET 8 ASP.NET Core Web API for tracking parcels destined for Mars. This solution demonstrates layered architecture, business logic separation, validation, audit trail handling, and a simple document-backed data store for experimentation and learning.

## Contents
- [Tech Stack](#tech-stack)
- [Project Layout](#project-layout)
- [Prerequisites](#prerequisites)
- [Build](#build)
- [Run the API](#run-the-api)
- [Swagger / OpenAPI](#swagger--openapi)
- [Sample Requests](#sample-requests)
- [Status Lifecycle Rules](#status-lifecycle-rules)
- [Persistence](#persistence)
- [Running Tests](#running-tests)
- [Common Issues / FAQ](#common-issues--faq)
- [Improvement Ideas](#improvement-ideas)
- [AI Usage Disclosure](#ai-usage-disclosure)

## Tech Stack
- .NET 8 / C# 12
- ASP.NET Core Web API
- Newtonsoft NOT used – native `System.Text.Json` with custom converters (enum + date-only)
- NUnit / FluentAssertions / NSubstitute
- Simple JSON file as a document store

## Project Layout
```
RM.Mars.ParcelTracking.sln
├── RM.Mars.ParcelTracking/            # Web API project
│   ├── Controllers/
│   ├── Services/
│   ├── Repositories/
│   ├── Models/
│   ├── Utils/                         # Date provider & JSON converters
│   └── Database/DocumentDb.json       # Local persistence file
└── RM.Mars.ParcelTracking.Test/       # Unit & integration tests
```

## Prerequisites
- .NET 8 SDK (`dotnet --version` >= 8.x)
- (Optional) VS Code / Visual Studio 2022 / Rider
- Trust the ASP.NET Core HTTPS development certificate if prompted:
  ```bash
  dotnet dev-certs https --trust
  ```

## Build
From the solution root:
```bash
dotnet restore
dotnet build --no-restore
```

## Run the API
Run the web project explicitly (solution contains multiple projects):
```bash
dotnet run --project RM.Mars.ParcelTracking/RM.Mars.ParcelTracking.csproj
```
Typical console output will show two URLs, e.g.:
```
Now listening on: https://localhost:5001
Now listening on: http://localhost:5000
```
Use the HTTPS URL where possible.

### Hot Reload (optional)
```bash
dotnet watch --project RM.Mars.ParcelTracking/RM.Mars.ParcelTracking.csproj run
```

## Swagger / OpenAPI
Swagger UI is enabled automatically in `Development`:
```
https://localhost:5001/swagger
```
If you don't see it, ensure `ASPNETCORE_ENVIRONMENT=Development` (default when running locally).

## Sample Requests
All payloads use camelCase due to configured `JsonNamingPolicy.CamelCase`.

### Create Parcel
```bash
curl -X POST https://localhost:5001/Parcels \
  -H "Content-Type: application/json" \
  -d '{
    "barcode":"RMARS1234567890123456789M",
    "sender":"Anders Hejlsberg",
    "recipient":"Elon Musk",
    "contents":"Signed C# language specification and a Christmas card",
    "deliveryService":"Express"
  }'
```
Response (example):
```json
{
  "barcode": "RMARS1234567890123456789M",
  "status": "Created",
  "launchDate": "2025-09-03",
  "etaDays": 90,
  "estimatedArrivalDate": "2025-12-02",
  "origin": "Starport Thames Estuary",
  "destination": "New London",
  "sender": "Anders Hejlsberg",
  "recipient": "Elon Musk",
  "contents": "Signed C# language specification and a Christmas card"
}
```

### Get Parcel
```bash
curl https://localhost:5001/Parcels/RMARS1234567890123456789M
```

### Update Status
```bash
curl -X PATCH https://localhost:5001/Parcels/RMARS1234567890123456789M \
  -H "Content-Type: application/json" \
  -d '{"newStatus":"OnRocketToMars"}'
```
Allowed transitions are enforced (see below).

## Status Lifecycle Rules
```
Created -> OnRocketToMars -> LandedOnMars -> OutForMartianDelivery -> Delivered
                   └------------------------------└-------------------------------> Lost
```
Validation rules:
- Created → OnRocketToMars only if `launchDate <= UtcNow`
- OnRocketToMars → LandedOnMars only if `estimatedArrivalDate <= UtcNow`
- OnRocketToMars → Lost
- LandedOnMars → OutForMartianDelivery always valid
- OutForMartianDelivery → Delivered or Lost
- Delivered / Lost are terminal

## Mock Document Database
A simple file-backed JSON document store simulates persistence.
- File: `RM.Mars.ParcelTracking/Database/DocumentDb.json`

When running locally you can edit the json manually to simulate passage of time to allow status transitions.

Reset the database by deleting or editing the JSON file while the API is stopped.

DocumentDb.json must be present and in the following format:

```json
{
  "Parcels": [
    {
      "Barcode": "RMARS1234567890123456789M",
      "Status": "Created",
      "LaunchDate": "2025-09-03",
      "EstimatedArrivalDate": "2025-12-02",
      "Origin": "Starport Thames Estuary",
      "Destination": "New London",
      "Sender": "Anders Hejlsberg",
      "Recipient": "Elon Musk",
      "Contents": "Signed C# language specification and a Christmas card",
      "History": [
        {
          "Status": "Created",
          "TimeStamp": "2025-09-15"
        }
      ],
      "LastUpdated": "2025-09-15T21:14:47.398378Z"
    }
  ]
}
```

## Running Tests
```bash
dotnet test --no-build
```
Includes:
- Service logic
- Validation logic
- Status transition rules
- Date-only & enum serialization
- End-to-end controller flow (mocked backing services)

## Design Choices & Trade-offs

- **Layered Architecture:** Controllers → Services → Repositories. Keeps responsibilities separate and business logic testable.
- **File-backed Persistence:** Uses a JSON document store for simplicity and ease of local development. Not suitable for concurrency, scaling, or production durability.
- **Abstractions for Testability:** `IDateTimeProvider` and `ITimeCalculatorService` allow time-based logic in tests.
- **Minimal Validation:** Request validation is basic. A production system would use a more robust validation.
- **No Authentication/Authorization:** Left out for ease of testing since its MVP. In production, add token-based auth e.g. OAuth2.0

## What to Improve for Enterprise Scale

1. **Persistence & Data Integrity:**
   - Replace file-based store with Azure Cosmos DB for scalable document storage.
   - Add concurency control (ETags) and transactions where needed.

2. **Concurrency, Resiliency & Performance:**
   - Make all I/O asynchronous.
   - Add retry/backoff, circuit breakers for external dependencies.

3. **Observability & Operations:**
   - Add structured logging and integrate with Azure Application Insights for distributed tracing and metrics.
   - Use Azure Monitor for dashboards and alerting.

4. **Security:**
   - Add authentication (OAuth2 / Azure Active Directory) and authorization policies.
   - Implement input sanitization, rate-limiting, and Azure Key Vault for secrets management.
   - Add to APIM for API gateway features.

5. **Testing & CI/CD:**
   - Add automation tests
   - Use Azure DevOps CI/CD pipelines for automated build, test, and deployment.

6. **API Design & Contracts:**
   - Add versioning for breaking changes.
   - Add pagination, filtering, and sorting for list endpoints.

## Design Assumptions & Shortcuts

- Single project for simplicity.
- File-based JSON doc "database", allows persistence between runs, easier to refactor for production using CosmosDB.
- No authentication required.
- Next launch data stored in appsettings config hard-coded, would need to be updated in reality would store in a db.
- No pagination/filtering on GET all parcels endpoint.
- No rate limiting.
- No API versioning.
- Added a lastUpdatedUtc field to the parcel model to help with concurrency if this were to be extended.

## AI Tool Usage

- GitHub Copilot was used for drafting documentation, including the README and the xml summaries on the classes.
- GitHub Copilot was used for adding some more in-depth testing, including an end-to-end happy path test, and serialization tests.
- ChatGPT was used to breakdown the requirments into separate epics and stories (explicitly avoiding solutionising to avoid it providing any logic suggestions).
- GitHub Copilot was used to generate a util for json serialization of DateTime properties.
