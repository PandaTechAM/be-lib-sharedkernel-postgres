# Pandatech.SharedKernel.Postgres

Welcome to the `Pandatech.SharedKernel.Postgres` NuGet package — a specialized extension of the Pandatech.SharedKernel
designed to simplify PostgreSQL integration in your ASP.NET Core applications. This package provides a set of utilities
and configurations to streamline the setup of Entity Framework Core with PostgreSQL, along with health checks and other
enhancements.

Although this package is primarily intended for internal use, it is publicly available for anyone who may find it
useful. We recommend forking or copying the classes in this repository and creating your own package to suit your needs.

## Key Features

- **Simplified PostgreSQL Context Setup:** Easily configure your DbContext to use PostgreSQL with optimized settings.
- **Automatic Database Migration:** Automatically apply pending migrations to the database on application startup.
- **Decimal Type Configuration:** Globally configure the precision and scale for decimal properties.
- **PostgreSQL Health Checks:** Integrate health checks for PostgreSQL to monitor database connectivity and health.
- **Exception Handling:** Utilize exception processing to handle database-specific exceptions gracefully.
- **Integration with SharedKernel:** Seamlessly integrates with Pandatech.SharedKernel and other PandaTech packages.

## Prerequisites

- .NET 9.0 SDK or higher
- PostgreSQL database
- Entity Framework Core Tools and Design packages

## Installation

To install the `Pandatech.SharedKernel.Postgres` package, use the following command:

```bash
dotnet add package Pandatech.SharedKernel.Postgres
```

Alternatively, you can add it via the NuGet Package Manager in Visual Studio, VS Code, or Rider.

## Getting Started

Follow these steps to integrate `Pandatech.SharedKernel.Postgres` into your ASP.NET Core application.

### Step 1: Configure Connection String

Add your PostgreSQL connection string to the `appsettings.{Environment}.json` file:

```json
{
    "ConnectionStrings": {
        "Postgres": "Host=localhost;Database=mydatabase;Username=myusername;Password=mypassword"
    }
}
```

### Step 2: Modify Program.cs

Update your `Program.cs` file to include the necessary configurations:

```csharp
using SharedKernel.Postgres.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add PostgreSQL context with the connection string
builder.AddPostgresContext<MyDbContext>(builder.Configuration.GetConnectionString("Postgres")!);

// Optionally add Gridify for data filtering and pagination
builder.AddGridify(); // From Pandatech.Gridify.Extensions

var app = builder.Build();

// Apply pending migrations on startup
app.MigrateDatabase<MyDbContext>();

app.Run();

```

By invoking `builder.AddPostgresContext<T>()`, the package automatically integrates PostgreSQL health checks using the
`AspNetCore.HealthChecks.NpgSql` package.

## Dependencies

This package relies on several NuGet packages to provide extended functionality:

- AspNetCore.HealthChecks.NpgSql: Health checks for PostgreSQL.
- EntityFrameworkCore.Exceptions.PostgreSQL: Exception handling for EF Core and PostgreSQL.
- Pandatech.EFCore.AuditBase: Audit logging for EF Core entities.
- Pandatech.EFCore.PostgresExtensions: Additional extensions for EF Core and PostgreSQL.
- PandaTech.FileExporter: Utilities for exporting files.
- PandaTech.FluentImporter: Fluent API for importing data.
- Pandatech.GridifyExtensions: Extensions for Gridify, simplifying data filtering and pagination.
- Pandatech.SharedKernel: Core shared kernel functionalities.

## Notes

This library is designed primarily for internal use and, as such, does not include extensive documentation. For detailed
information on the functionalities provided by the dependencies, please refer to their respective documentation.

## License

Pandatech.SharedKernel.Postgres is licensed under the MIT License.