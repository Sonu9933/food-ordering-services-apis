# 🍔 Food Ordering Services API

A production-style **ASP.NET Core 8 Web API** implementing Clean Architecture for customer registration,
restaurant management, order placement, and payment processing.

---

## 📋 Table of Contents

- [Project Overview](#project-overview)
- [Architecture](#architecture)
- [Project Structure](#project-structure)
- [Domain Model](#domain-model)
- [API Endpoints](#api-endpoints)
- [Security](#security)
- [Caching](#caching)
- [Rate Limiting](#rate-limiting)
- [Resilience](#resilience)
- [Testing](#testing)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [Known Issues & Roadmap](#known-issues--roadmap)

---

## Project Overview

| Feature | Status |
|---------|--------|
| Customer registration & JWT login | ✅ Complete |
| Restaurant registration, update, delete | ✅ Complete |
| Order placement & retrieval | ✅ Complete |
| Menu item management | 🚧 Planned |
| Payment processing | 🔧 Stub (always succeeds) |
| React + TypeScript frontend | 🚧 Planned |
| RabbitMQ message queue | 🚧 Planned |

---

## Architecture

The solution follows **Clean Architecture** with strict layer separation:

- **Presentation Layer**: ASP.NET Core MVC for API surface
- **Application Layer**: Business logic, DTOs, interfaces
- **Domain Layer**: Domain entities, value objects, aggregates
- **Infrastructure Layer**: Data access, file storage, external services

### Technology Stack

- **ASP.NET Core 8**: Web API framework
- **Entity Framework Core**: ORM for database access
- **AutoMapper**: Object-to-object mapping
- **FluentValidation**: Validation for API models
- **Polly**: Resilience and transient-fault-handling
- **Serilog**: Logging
- **MediatR**: In-process messaging


### Deployment

- **Docker**: Containerization
- **Kubernetes**: Container orchestration (planned)
- **Azure/AWS**: Cloud hosting (planned)

---

## Design Principles
- **Clean Architecture** — dependencies point inward; Core has no knowledge of Infrastructure or API
- **SOLID** — every class has a single responsibility; all dependencies are injected via interfaces
- **DRY / KISS** — shared validation via Data Annotations; no duplicated query logic
- **Security First** — BCrypt password hashing, JWT bearer authentication, rate limiting
- **Test-Driven** — unit tests (Moq + EF Core InMemory) and integration tests (WebApplicationFactory)

---

## Contributing

1. **Fork the repository**
2. **Create a feature branch**: `git checkout -b feature/YourFeature`
3. **Make your changes**
4. **Commit your changes**: `git commit -m 'Add some feature'`
5. **Push to the branch**: `git push origin feature/YourFeature`
6. **Open a pull request**

Please ensure your code adheres to the existing style and passes all tests.

---

## Domain Model

| Entity | Key Relationships |
|--------|-------------------|
| `Customer` | One-to-many with `Order` (restricted delete) |
| `Restaurant` | One-to-many with `Order` (restricted delete); one-to-many with `MenuItems` (cascade delete) |
| `Order` | One-to-many with `OrderDetail` (cascade delete) |
| `OrderDetail` | Many-to-one with `Order`; many-to-one with `MenuItems` (restricted delete) |
| `MenuItems` | Belongs to one `Restaurant` |

---

## API Endpoints

### Customer Auth — `api/CustomerAuth`

| Method | Route | Description | Auth |
|--------|-------|-------------|------|
| `POST` | `/register-consumer` | Register a new customer account |
| `POST` | `/login-consumer` | Authenticate and receive a JWT 

### Restaurant — `api/Restaurant`

| Method | Route | Description | Auth |
|--------|-------|-------------|------|
| `GET` | `/get-all-restaurents` | List all registered restaurants |
| `GET` | `/get-restaurent-by-Id?restaurantId=` | Get a restaurant by ID |
| `GET` | `/delete-restaurent-by-Id?restaurantId=` | Delete a restaurant by ID |
| `POST` | `/register-restaurant` | Register a new restaurant |
| `POST` | `/update-restaurant` | Update restaurant details |

### Order — `api/Order`

| Method | Route | Description | Auth |
|--------|-------|-------------|------|
| `POST` | `/place-order` | Place a new order |
| `GET` | `/get-order-detail-orderId?orderId=` | Get order by order ID |
| `GET` | `/get-order-detail-customerId?customerId=` | Get all orders for a customer |

### Payment — `api/Payment`

| Method | Route | Description | Auth |
|--------|-------|-------------|------|
| `POST` | `/process` | Process a payment (stub — always succeeds) |

---

## Security

- **Password hashing** — BCrypt via `BCrypt.Net-Next`. Raw passwords never persist.
- **JWT authentication** — HS256 signed tokens validated on every protected request.
  Configured via `SecurityKey`, `Issuer`, and `Audience` in `appsettings.json`.
- **Token lifetime** — Configurable; clock skew set to zero for strict expiry enforcement.
- **API versioning** — Supported via URL segment (`/v1/`) and `X-Api-Version` header.

---

## Caching

Login lookups are accelerated with **Redis** distributed cache:

- On login, the customer record is serialised to JSON and stored for **5 minutes**.
- On subsequent logins within that window, the cache is hit first (BCrypt verification still applied).
- Cache key = customer email address.

Configure the connection string in `appsettings.json`:

---

## Rate Limiting

A **fixed-window** rate limiter is applied globally to all controllers:

| Setting | Value |
|---------|-------|
| Requests permitted | 4 per window |
| Window duration | 12 seconds |
| Queue limit | 2 (oldest-first) |
| Rejection status | `429 Too Many Requests` |

---

## Resilience

**Polly** resilience pipeline (`"retry"`) is registered for downstream calls:

| Strategy | Configuration |
|----------|--------------|
| Retry | 3 attempts · exponential back-off (base 2 s) · jitter |
| Circuit Breaker | Opens at 90 % failure rate · min 5 requests · breaks for 5 s |
| Timeout | 1 second per attempt |

---

## Testing

### Unit Tests — `FoodOrderingServices.UnitTests`

- **Framework**: xUnit + Moq + EF Core InMemory + BCrypt.Net-Next
- **Coverage**: Controller → Service → Repository for all four features
- All dependencies mocked via interfaces; no real DB or network calls

| Layer | Key assertions |
|-------|---------------|
| Controllers | Status codes, response body shape, service call counts |
| Services | Return value mapping, null propagation, delegate-to-repository behaviour |
| Repositories | EF Core state (persisted rows), BCrypt hashing, Redis cache interaction |

### Integration Tests — `FoodOrderingServices.IntegrationTests`

- **Framework**: xUnit + `WebApplicationFactory<Program>` + EF Core InMemory + in-memory distributed cache
- Requires `<PreserveCompilationContext>true</PreserveCompilationContext>` in the test project
- Redis replaced with `AddDistributedMemoryCache()` for test isolation
- Each test class constructor cleans all tables for full isolation

---

## Getting Started

### Prerequisites

| Dependency | Version |
|------------|---------|
| .NET SDK | 8.0+ |
| Microsoft SQL Server | 2019+ |
| Redis | 6.0+ |
| Git | Any |

### Run locally

1. Clone the repository: `git clone https://github.com/yourusername/FoodOrderingServices.git`
2. Start the SQL Server and Redis services
3. Update the connection strings in `appsettings.json`
4. Run the database migrations:
   ```
   dotnet ef migrations add InitialCreate --project Infrastructure/Persistence
   dotnet ef database update --project Infrastructure/Persistence
   ```
5. Start the application: `dotnet run --project src/WebAPI`
6. Test the API endpoints using Swagger UI (`https://localhost:{port}/swagger`) or Postman

### Run tests

- **Unit tests**: `dotnet test --project FoodOrderingServices.UnitTests`
- **Integration tests**: `dotnet test --project FoodOrderingServices.IntegrationTests`

---

## Known Issues & Roadmap

| # | Issue / Task | Priority |
|---|-------------|----------|
| 1 | `CustomerRepositary.LoginCustomerAsync` — password not verified on DB path (only on cache hit) | High |
| 2 | `RestaurantRepositary.DeleteRestaurentAsync` — logic inverted; returns `false` when restaurant exists | High |
| 3 | `OrderController` logger type is `ILogger<RestaurantController>` (copy-paste bug) | Medium |
| 4 | `PaymentService` / `PaymentRepositary` — stub only, no real gateway integration | Medium |
| 5 | `AuthenticationResponse.ExpiresAt` set to `DateTime.UtcNow` instead of actual token expiry | Medium |
| 6 | Menu item management endpoints missing | Low |
| 7 | RabbitMQ message queue integration | Low |
| 8 | React + TypeScript frontend | Low |