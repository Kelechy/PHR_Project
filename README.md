Personal Health Records (PHR) API - 
=========================================================

Overview
--------
The Personal Health Records (PHR) API is a secure and modular RESTful API built with ASP.NET Core 8. 
It enables patients and authorized users to manage, access, and share personal health records. 
The system is designed for scalability, maintainability, and supports both SQL Server and PostgreSQL databases.

Key Features
-------------
- Modular architecture with clear separation of concerns.
- JWT authentication and role-based authorization.
- Entity Framework Core ORM with support for both MSSQL and PostgreSQL.
- Soft delete and record auditing.
- Role and access request management.
- Unit and integration testing using xUnit.
- Interactive API documentation with Swagger.
- Container-ready for Docker deployments.

## System Architecture
-------------------
Controllers: Define REST endpoints (Auth, Patients, Roles, AccessRequests).
Services: Contain business logic and handle data operations.
Data Layer: Entity Framework Core DbContext and entity models.
Security: JWT-based authentication and authorization.
Testing: Unit and integration tests with xUnit and EF InMemory.

## Directory Structure
----------------------
PHR_Project/
 ├── src/
 │   └── PHR.Api/
 │       ├── Controllers/
 │       ├── Data/
 │       ├── Models/
 │       ├── Services/
 │       ├── Program.cs
 │       ├── appsettings.json
 │       └── appsettings.Development.json
 └── tests/
     └── PHR.Tests/

## Technology Stack
-------------------
- ASP.NET Core 8
- Entity Framework Core 8
- Microsoft SQL Server / PostgreSQL
- Swashbuckle (Swagger)
- JWT Bearer Authentication
- xUnit and Moq
- Microsoft.AspNetCore.Mvc.Testing

## Setup Instructions
---------------------
1. Clone the repository:
   git clone https://github.com/Kcy/PHR_Project.git
   cd PHR_Project

2. Install prerequisites:
   - .NET 8 SDK
   - SQL Server or PostgreSQL
   - (Optional) Docker

3. Configure database connection:
   Open appsettings.json and configure the database provider and connection strings.

   Example configuration:
   {
     "DatabaseProvider": "MSSQL", 
     "ConnectionStrings": {
       "MSSQL": "Server=localhost,1433;Database=phr_db;User Id=sa;Password=Passw0rd!;TrustServerCertificate=True;",
       "Postgres": "Host=localhost;Port=5432;Database=phr_db;Username=postgres;Password=postgres"
     },
     "Jwt": {
       "Key": "EoU0oz0iWqDQ6a6OlXnpxWBTgTQpQehxDyaV6G9+OglETq5UrUonlZzJX2ps88T0"
       "Issuer": "PHR.Api",
       "Audience": "PHR.Client",
       "ExpiryMinutes": 60
     }
   }

To switch between providers, change the value of DatabaseProvider to either "MSSQL" or "Postgres".

4. Install dependencies:
   dotnet restore

5. Build the project:
   dotnet build

6. Run the application:
   dotnet run --project src/PHR.Api

   The API will run on https://localhost:5001
   Swagger documentation will be available at https://localhost:5001/swagger

7. Run tests:
   dotnet test -v n

   //Tests are based on EF Core InMemory, so no real database is required.

## JWT Authentication Setup
---------------------------
- Obtain a token by calling /api/auth/login with valid credentials.
- Copy the token returned from the response.
- In Swagger UI, click “Authorize” and enter:
  Bearer <your-token>

##Switching Databases
---------------------
The system supports both MSSQL and PostgreSQL.

To use SQL Server:
   "DatabaseProvider": "MSSQL"

To use PostgreSQL:
   "DatabaseProvider": "Postgres"

Run EF migrations if needed:
   dotnet ef migrations add Initial_MSSQL --context AppDbContext --output-dir Migrations/SqlServer
   dotnet ef database update

## Testing
----------
Unit Tests:
   - Test individual services with EF Core InMemory.
   - Run using: dotnet test

Integration Tests:
   - Use WebApplicationFactory to simulate API requests.
   - Run automatically with dotnet test.

## Design Patterns and Practices
--------------------------------
- Repository pattern (via EF Core DbContext)
- Dependency Injection (Service registration in Program.cs)
- Unit of Work (managed by EF Core transaction scope)
- Service Layer abstraction
- DTOs for clean data transfer
- JWT Authentication for secure access

## Common Commands
------------------
dotnet restore               - Restore NuGet packages
dotnet build                 - Build all projects
dotnet run --project src/PHR.Api  - Run the API
dotnet test -v n             - Run all tests
dotnet ef migrations add <Name>   - Create a new EF migration
dotnet ef database update    - Apply database migrations


## Role–Permission Model Explanation
------------------------------------
Overview:

The Role–Permission model defines how access and capabilities are controlled across the system.

Core Concepts

| Concept            | Description                                                        |
| ------------------ | ------------------------------------------------------------------ |
| **User**           | Represents an authenticated person in the system.                  |
| **Role**           | A named collection of permissions.                                 |
| **Permission**     | A specific action (e.g. `createPatientRecords`, `viewAllRecords`). |
| **Access Request** | Temporary permission request from one user to another.             |

Implementation:

Models: Role, User, Permission define relationships.

Services: RoleService manages creation and assignment.

AuthService: Adds role and permission claims to JWTs.

Controllers: Enforce rules via [Authorize] and User.HasPermission("").


Access Request Flow:

User requests access to another’s record.

Admin/Owner approves or rejects.

Approved access grants temporary permission.

AccessExpiryMonitor revokes expired access automatically.


Example Roles and Permissions:

| Role        | Permissions                                                       |
| ----------- | ----------------------------------------------------------------- |
| **Viewer** | `viewPatientRecords`, `updateOwnRecord`                           |
| **Creator**  | `createPatientRecords`, `viewAllRecords`, `approveAccessRequests` |
| **Admin**   | `manageRoles`, `manageUsers`, `viewAllRecords`                    |


Note:
For Database, either of PostgreSQL can be used, the system is designed to enable switching between the two:
-To use SQL Server:
   "DatabaseProvider": "MSSQL"

To use PostgreSQL:
   "DatabaseProvider": "Postgres" in the appsettings.




Author: KELECHI UCHENNA.
