# Pet Catalog C# Microservice - Architecture

## Overview

This document describes the architecture and design decisions for the Pet Catalog C# microservice, built as a serverless application using Azure Functions with .NET 8.

## Architecture Style

The microservice follows **Clean Architecture** principles with clear separation of concerns across multiple layers:

```
┌─────────────────────────────────────────────────────────────────┐
│                        Azure Functions                          │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │                 Presentation Layer                       │   │
│  │            (HTTP Triggers & DTOs)                       │   │
│  └─────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
                                │
┌─────────────────────────────────────────────────────────────────┐
│                    Application Layer                            │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐ │
│  │   Use Cases     │  │   Services      │  │   DTOs          │ │
│  │   (Business     │  │   (Application  │  │   (Data         │ │
│  │   Logic)        │  │   Services)     │  │   Transfer)     │ │
│  └─────────────────┘  └─────────────────┘  └─────────────────┘ │
└─────────────────────────────────────────────────────────────────┘
                                │
┌─────────────────────────────────────────────────────────────────┐
│                      Domain Layer                               │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐ │
│  │   Entities      │  │   Interfaces    │  │   Domain        │ │
│  │   (Core         │  │   (Repository   │  │   Logic         │ │
│  │   Models)       │  │   Contracts)    │  │                 │ │
│  └─────────────────┘  └─────────────────┘  └─────────────────┘ │
└─────────────────────────────────────────────────────────────────┘
                                │
┌─────────────────────────────────────────────────────────────────┐
│                   Infrastructure Layer                          │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐ │
│  │   Data Access   │  │   External      │  │   Cross-cutting │ │
│  │   (EF Core +    │  │   Services      │  │   Concerns      │ │
│  │   PostgreSQL)   │  │   (Azure Blob)  │  │   (Logging)     │ │
│  └─────────────────┘  └─────────────────┘  └─────────────────┘ │
└─────────────────────────────────────────────────────────────────┘
```

## Layer Responsibilities

### 1. Presentation Layer (`PetCatalog.Functions`)
- **Responsibility**: HTTP endpoint handling, request/response formatting
- **Components**:
  - Azure Functions with HTTP triggers
  - Request/Response DTOs
  - Input validation
  - HTTP status code management
- **Dependencies**: Application layer only

### 2. Application Layer (`PetCatalog.Application`)
- **Responsibility**: Use case orchestration, business workflow coordination
- **Components**:
  - Application services
  - Use case implementations
  - Application DTOs
  - Validation logic
- **Dependencies**: Domain layer only

### 3. Domain Layer (`PetCatalog.Domain`)
- **Responsibility**: Core business entities and rules
- **Components**:
  - Entity models (`Pet`)
  - Repository interfaces
  - Domain-specific exceptions
  - Business rule validation
- **Dependencies**: None (pure domain logic)

### 4. Infrastructure Layer (`PetCatalog.Infrastructure`)
- **Responsibility**: External system integration, data persistence
- **Components**:
  - Entity Framework DbContext
  - Repository implementations
  - Database migrations
  - External service integrations (Azure Blob Storage)
- **Dependencies**: Application and Domain layers

## Design Patterns

### 1. Repository Pattern
```csharp
public interface IPetRepository
{
    Task<Pet?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Pet>> GetAllAsync(int limit = 100, int offset = 0, CancellationToken cancellationToken = default);
    Task<Pet> CreateAsync(Pet pet, CancellationToken cancellationToken = default);
    Task<Pet?> UpdateAsync(Pet pet, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
```

**Benefits**:
- Abstracts data access logic
- Enables easy unit testing with mocks
- Supports multiple data sources

### 2. Dependency Injection
```csharp
services.AddScoped<IPetRepository, PetRepository>();
services.AddScoped<IPetApplicationService, PetApplicationService>();
services.AddScoped<IPetService, PetService>();
```

**Benefits**:
- Loose coupling between components
- Easy testing and mocking
- Configuration flexibility

### 3. DTO Pattern
```csharp
public record PetDto(int Id, string Name, string Species, ...);
public record CreatePetDto(string Name, string Species, ...);
public record UpdatePetDto(string? Name, string? Species, ...);
```

**Benefits**:
- Clear API contracts
- Prevents over-posting attacks
- Immutable data transfer objects

## Data Flow

### Create Pet Request Flow
```
1. HTTP POST /api/pets
   ↓
2. CreatePet Function (Presentation)
   ↓
3. PetService.CreatePetAsync() (Functions Layer)
   ↓
4. PetApplicationService.CreatePetAsync() (Application)
   ↓
5. PetRepository.CreateAsync() (Infrastructure)
   ↓
6. Entity Framework → PostgreSQL
```

### Error Handling Strategy
```csharp
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public string? Error { get; set; }
}
```

**Error Types**:
- **Validation Errors**: 400 Bad Request
- **Not Found**: 404 Not Found
- **Server Errors**: 500 Internal Server Error

## Database Design

### Pet Entity
```sql
CREATE TABLE pets (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    species VARCHAR(50) NOT NULL,
    breed VARCHAR(100),
    age INTEGER,
    color VARCHAR(50),
    weight DECIMAL(5,2),
    description TEXT,
    image_url VARCHAR(255),
    is_available BOOLEAN DEFAULT true,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Indexes for performance
CREATE INDEX idx_pets_species ON pets(species);
CREATE INDEX idx_pets_available ON pets(is_available);
```

## Testing Strategy

### 1. Unit Tests (`PetCatalog.UnitTests`)
- **Scope**: Individual components in isolation
- **Tools**: xUnit, Moq, FluentAssertions
- **Targets**: Application services, domain logic

### 2. Integration Tests (`PetCatalog.IntegrationTests`)
- **Scope**: Component interactions, database operations
- **Tools**: xUnit, Testcontainers, FluentAssertions
- **Targets**: Repository implementations, database operations

### 3. API Tests
- **Scope**: End-to-end HTTP endpoint testing
- **Tools**: Azure Functions testing framework
- **Targets**: Complete request/response cycles

## Configuration Management

### Environment Variables
- `POSTGRES_CONN`: Database connection string
- `STORAGE_ACCOUNT_NAME`: Azure Storage account
- `STORAGE_ACCOUNT_KEY`: Storage access key
- `APPINSIGHTS_INSTRUMENTATIONKEY`: Application Insights

### Settings Hierarchy
1. Environment variables (highest priority)
2. `local.settings.json` (local development)
3. `appsettings.json` (default values)

## Deployment Architecture

### Local Development
```
Developer Machine
├── Azure Functions Core Tools
├── Local PostgreSQL (Docker)
└── Azure Storage Emulator
```

### Azure Production
```
Azure Functions App
├── Consumption Plan (serverless)
├── Application Insights (monitoring)
├── Azure PostgreSQL Flexible Server
└── Azure Blob Storage
```

## Security Considerations

### 1. Authentication & Authorization
- Azure Functions Key-based authentication
- Function-level authorization
- Future: Azure AD integration

### 2. Data Protection
- Encrypted connection strings
- Secrets stored in Azure Key Vault
- HTTPS enforcement

### 3. Input Validation
- Request DTO validation
- SQL injection protection (EF Core)
- XSS prevention through proper encoding

## Performance Considerations

### 1. Database Optimization
- Indexed columns for common queries
- Connection pooling via EF Core
- Async operations throughout

### 2. Caching Strategy
- Entity Framework query caching
- Future: Redis cache for frequently accessed data

### 3. Monitoring
- Application Insights integration
- Structured logging
- Performance counters

## Evolution & Extensibility

### 1. Adding New Features
- New entities: Add to Domain layer
- New endpoints: Add to Functions layer
- New business logic: Add to Application layer

### 2. Integration Points
- Event-driven architecture ready
- Service bus integration points
- API versioning strategy

### 3. Migration Strategy
- Entity Framework migrations
- Blue-green deployment support
- Database schema versioning

## Comparison with Python Version

| Aspect | Python Version | C# Version |
|--------|----------------|------------|
| **Architecture** | Function-based modules | Clean Architecture layers |
| **ORM** | SQLAlchemy | Entity Framework Core |
| **Validation** | Pydantic models | Data Annotations + FluentValidation |
| **DI Container** | Manual setup | Built-in .NET DI |
| **Testing** | pytest | xUnit + Moq |
| **Type Safety** | Runtime (Pydantic) | Compile-time |
| **Performance** | Interpreted | Compiled |

## Decision Log

### ADR-001: Clean Architecture
**Decision**: Use Clean Architecture with clear layer separation
**Rationale**: Better maintainability, testability, and separation of concerns

### ADR-002: Entity Framework Core
**Decision**: Use EF Core instead of Dapper or raw ADO.NET
**Rationale**: Type safety, migrations, and better maintainability

### ADR-003: Record Types for DTOs
**Decision**: Use C# record types for DTOs
**Rationale**: Immutability, value equality, and concise syntax

### ADR-004: Azure Functions Isolated Model
**Decision**: Use .NET isolated worker model
**Rationale**: Better performance, latest .NET features, clearer separation

This architecture provides a solid foundation for a maintainable, testable, and scalable microservice that can evolve with changing requirements.
