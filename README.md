# FlatLife

A full-stack web application for organizing shared-household (flatshare) life:
splitting bills, assigning chores, and managing shared to-do lists. Built as a
team project during my training as Fachinformatiker für Anwendungsentwicklung.

## Tech Stack

**Backend**
- C# / .NET 8, ASP.NET Core Web API
- Entity Framework Core 8 with PostgreSQL (Npgsql)
- JWT-based authentication (Bearer tokens)
- Swagger / OpenAPI for API documentation
- xUnit for unit tests

**Frontend**
- Blazor (.NET 8)
- Bootstrap

## Features

- **User accounts** – registration and login with JWT-secured endpoints
- **Flats** – create a flat or join an existing one via an invite/short code
- **Chores** – create household tasks and assign them to flat members
- **To-do lists** – shared task lists for the household
- **Bill splitter** – split shared expenses between flat members

## Architecture

The backend follows a layered structure:

- `Controllers/` – REST endpoints (User, CreateOrJoinFlat, FlatTask, Todo, BillSplitter)
- `Services/` – business logic (token handling, user repository, task assignment)
- `Models/` – request/response DTOs
- `Database/` – EF Core `DbContext` and entities
- `Mapping/` – DTO/entity mappers
- `Migrations/` – EF Core migrations

The frontend (Blazor) consumes the API through typed services and handles auth
state on the client.

## Getting Started

### Prerequisites
- .NET 8 SDK
- PostgreSQL

### Backend

1. Set your connection string and JWT key. Do **not** commit real secrets —
   use environment variables or user secrets. Example `appsettings.json`:
   ```json
   {
     "JWT": { "Token": "myJWTSigningKey" },
     "ConnectionStrings": {
       "DefaultPostgreSQLConnection": "Host=localhost;Port=5432;Database=Flatlife;User Id=postgres;Password=myPassword"
     }
   }
   ```
2. Apply the database migrations:
   ```bash
   dotnet ef database update
   ```
3. Run the API:
   ```bash
   dotnet run
   ```
   Swagger UI is then available at the API's `/swagger` endpoint.

### Frontend

```bash
cd FlatLifeFrontend
dotnet run
```

## Notes

This was a collaborative project built during my apprenticeship to practice
full-stack development with C#, Blazor and a custom REST API.
