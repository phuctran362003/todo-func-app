# To-Do Management Service with Azure Cosmos DB

A mini-service for managing To-Do items built with Azure Functions and Azure Cosmos DB (SQL API), demonstrating best practices for cloud-native .NET applications.

## Objectives

- Get familiar with **Azure Cosmos DB** in .NET applications
- Understand and apply best practices for:
  - Partition key design
  - Request Units (RU) optimization
  - Cosmos DB .NET SDK v3+ usage
  - Repository pattern implementation
  - Efficient stored procedures and queries
  - Error handling & retry patterns

## Architecture

This solution follows a **Clean Architecture** pattern with the following layers:

```
├── Domain/              # Entities, DTOs, and database context
├── Application/         # Business logic and services
├── Infrastructure/      # Data access, external services, utilities
└── Todo.FunctionApp/    # Azure Functions (API layer)
```

### Key Components

- **Azure Functions**: Serverless HTTP-triggered functions for API endpoints
- **Azure Cosmos DB**: NoSQL database with SQL API
- **Repository Pattern**: Generic repository for data access abstraction
- **Unit of Work**: Transaction management across repositories
- **Dependency Injection**: IoC container for service management

## Technologies

- **.NET 8.0**
- **Azure Functions v4**
- **Azure Cosmos DB SDK v3+**
- **C# 12**

## Application Main Flow

### User Registration & Authentication
1. **Register Account**
   - User submits registration details (email, password)
   - Password is hashed using PasswordHasher
   - User entity created in Cosmos DB
   - Success confirmation returned

2. **Login**
   - User submits credentials (email, password)
   - System validates password hash
   - JWT token generated with user claims
   - Token returned to client for subsequent requests

3. **Authentication**
   - User includes JWT token in Authorization header
   - System validates token on each request
   - User identity and permissions extracted from claims

### To-Do Item Management
1. **Create To-Do Item**
   - Authenticated user submits new to-do (title, description)
   - System associates item with user ID (partition key)
   - Item saved to Cosmos DB
   - Created item returned with unique ID

2. **View To-Do Items**
   - User requests their to-do list
   - System queries Cosmos DB filtered by user ID
   - Efficient single-partition query (optimized RU consumption)
   - List of to-do items returned

3. **Update To-Do Item**
   - User modifies existing to-do (title, status, completion)
   - System validates ownership (user ID matches)
   - Item updated in Cosmos DB
   - Updated item returned

4. **Delete To-Do Item**
   - User requests to delete specific to-do
   - System validates ownership
   - Item removed from Cosmos DB
   - Success confirmation returned

5. **Mark as Complete/Incomplete**
   - User toggles completion status
   - System updates `isCompleted` field
   - Timestamp updated automatically
   - Updated status returned

### Data Flow Pattern
```
User Action → API Request → Authentication → Business Logic → 
Repository → Cosmos DB → Response → User Interface
```

### Typical User Journey
```
1. Register/Login → Receive JWT Token
2. Create To-Do Items → Items stored with userId partition key
3. View All To-Dos → Filtered by current user
4. Update/Complete Items → Modify existing items
5. Delete Items → Remove completed or unwanted items
```

## Technical Implementation Flow

### Request Flow
1. **HTTP Request** → Azure Function endpoint
2. **Authentication** → JWT token validation (Infrastructure/Utils/TokenTools.cs)
3. **Claims Extraction** → User identity from token (Infrastructure/Commons/ClaimsService.cs)
4. **Business Logic** → Application services process the request
5. **Repository Layer** → UnitOfWork coordinates data access
6. **Cosmos DB** → Generic repository performs CRUD operations
7. **Response** → Standardized ApiResult format returned to client

### Data Access Flow
```
Controller (Function) 
    ↓
Application Service
    ↓
Unit of Work
    ↓
Generic Repository<T>
    ↓
CosmosDbContext
    ↓
Azure Cosmos DB
```

### Authentication Flow
1. User login request with credentials
2. Password validation using PasswordHasher
3. JWT token generation with claims (user ID, roles)
4. Token returned to client
5. Subsequent requests include token in Authorization header
6. Token validated and claims extracted for authorization

### Error Handling Flow
1. Exception occurs in any layer
2. ExceptionUtils captures and logs the error
3. ErrorHelper formats the error message
4. ApiResult with error details returned to client
5. Appropriate HTTP status code set
6. Cosmos DB transient errors trigger retry logic

## Project Structure

### Domain Layer
- **Entities**: `User`, `TodoItem`, `BaseEntity`
- **DTOs**: Data transfer objects for API contracts
- **CosmosDbContext**: Database context configuration

### Application Layer
- **Services**: Business logic (BlobService, EmailService)
- **Interfaces**: Service contracts
- **Utils**: Helper classes (JWT, Password hashing, OTP generation)

### Infrastructure Layer
- **Repositories**: Generic repository implementation
- **UnitOfWork**: Transaction coordination
- **Commons**: Cross-cutting concerns (Logging, Claims, Time)
- **Utils**: Infrastructure utilities

### Function App Layer
- **Functions**: HTTP-triggered Azure Functions
- **Program.cs**: Host configuration and DI setup
- **IocContainer**: Dependency injection configuration
