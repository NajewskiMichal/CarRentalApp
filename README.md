# CarRentalApp

Small car rental management system built with .NET 8, designed to show clean structure, simple domain logic and automated tests.

## Architecture

The solution is split into layers:

- **CarRental.Domain**
  - Entities: `Car`, `Customer`, `Rental`, `User`
  - Value objects: `Email`
  - Enums: `UserRole`
  - Domain exceptions and validation rules

- **CarRental.Application**
  - Application services:
    - `CustomerService`
    - `CarService`
    - `RentalService`
    - `AuthService`
    - `UserManagementService`
    - `DashboardService`
  - DTOs
  - Interfaces:
    - repository interfaces (`ICustomerRepository`, `ICarRepository`, `IRentalRepository`, `IUserRepository`)
    - infrastructure abstractions (`ILogger`, `IPasswordHasher`)

- **CarRental.Infrastructure**
  - Persistence:
    - `SQLiteDbConnectionFactory`
    - `SQLiteCustomerRepository`, `SQLiteCarRepository`, `SQLiteRentalRepository`, `SQLiteUserRepository`
    - `DatabaseInitializer` (creates schema, seeds admin user)
  - Configuration:
    - `AppConfiguration` (connection string + log path from environment variables)
  - Logging:
    - `FileLogger` (logs to file)
  - Security:
    - `PasswordHasher` (PBKDF2 with salt)

- **CarRental.Tests**
  - Unit tests for domain and application
  - Infrastructure tests (`PasswordHasher`, `FileLogger`)
  - Integration tests for SQLite repositories and a full rental flow

## Technologies

- .NET 8
- SQLite (`Microsoft.Data.Sqlite`)
- xUnit
- Moq
- PBKDF2-based password hashing
- File-based logging

## Testing

- **Domain tests** – entities, value objects, domain rules
- **Application tests** – services (validation, mapping, error handling)
- **Infrastructure tests** – password hashing and file logger
- **Integration tests**:
  - real SQLite database in temp files
  - `DatabaseInitializer` + SQLite repositories
  - end-to-end rental flow:
    - add customer
    - add car
    - rent car
    - return car
    - check dashboard summary

## How to run

```bash
dotnet restore
dotnet build
dotnet test
dotnet run --project CarRental.ConsoleUI
