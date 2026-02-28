# Pandatech.SharedKernel.Postgres

PostgreSQL integration helpers for ASP.NET Core 10. Wraps Npgsql, Entity Framework Core, health checks, exception
mapping, query locks, snake_case naming, and audit trail wiring into a small set of extension methods so every service
starts from the same consistent baseline.

Requires **.NET 10.0**. Uses C# 14 extension members and cannot be downgraded.

---

## Table of Contents

1. [Installation](#installation)
2. [What is included](#what-is-included)
3. [Registering a DbContext](#registering-a-dbcontext)
4. [Migrations](#migrations)
5. [Model configuration helpers](#model-configuration-helpers)
6. [Health checks](#health-checks)

---

## Installation

```bash
dotnet add package Pandatech.SharedKernel.Postgres
```

---

## What is included

Every `AddPostgresContext*` overload automatically applies the following to your DbContext:

| Feature                           | Source package                              |
|-----------------------------------|---------------------------------------------|
| Npgsql provider                   | `Npgsql.EntityFrameworkCore.PostgreSQL`     |
| Snake_case naming convention      | `EFCore.NamingConventions`                  |
| Query locks (`FOR UPDATE` etc.)   | `Pandatech.EFCore.PostgresExtensions`       |
| Friendly exception mapping        | `EntityFrameworkCore.Exceptions.PostgreSQL` |
| `AuditBase` property validation   | `Pandatech.EFCore.AuditBase`                |
| Audit trail interceptors (opt-in) | `Pandatech.EFCore.Audit`                    |
| Postgres health check             | `AspNetCore.HealthChecks.NpgSql`            |
| Bulk extensions                   | `EFCore.BulkExtensions.PostgreSql`          |
| Gridify query extensions          | `Pandatech.GridifyExtensions`               |

---

## Registering a DbContext

All overloads are on `WebApplicationBuilder` and return `WebApplicationBuilder` for chaining. Every variant
automatically registers a Postgres health check named `postgres_{DatabaseName}`.

### Basic registration (no pooling, no audit trail)

```csharp
// Minimal — connection string only
builder.AddPostgresContext<AppDbContext>(connectionString);

// With migrations assembly by name
builder.AddPostgresContext<AppDbContext>(connectionString, "MyApp.Migrations");

// With migrations assembly by Assembly reference
builder.AddPostgresContext<AppDbContext>(connectionString, typeof(Program).Assembly);

// With migrations assembly by marker type
builder.AddPostgresContext<AppDbContext, Program>(connectionString);

// Full control via NpgsqlDbContextOptionsBuilder callback
builder.AddPostgresContext<AppDbContext>(connectionString, npgsql =>
{
    npgsql.MigrationsAssembly("MyApp.Migrations");
    npgsql.CommandTimeout(60);
});
```

### With connection pooling

Same set of overloads, prefixed with `Pool`:

```csharp
builder.AddPostgresContextPool<AppDbContext>(connectionString);
builder.AddPostgresContextPool<AppDbContext>(connectionString, "MyApp.Migrations");
builder.AddPostgresContextPool<AppDbContext, Program>(connectionString);
builder.AddPostgresContextPool<AppDbContext>(connectionString, npgsql => { ... });
```

Use pooling for high-throughput services. Note that DbContext pooling requires that your context has no scoped
dependencies injected via the constructor; use service provider overloads (`(sp, options) => ...`) for those cases.

### With audit trail (no pooling)

Registers the `Pandatech.EFCore.Audit` interceptors alongside the standard options:

```csharp
builder.AddPostgresContextWithAuditTrail<AppDbContext>(connectionString);
builder.AddPostgresContextWithAuditTrail<AppDbContext>(connectionString, "MyApp.Migrations");
builder.AddPostgresContextWithAuditTrail<AppDbContext, Program>(connectionString);
builder.AddPostgresContextWithAuditTrail<AppDbContext>(connectionString, npgsql => { ... });
```

### With pooling and audit trail

```csharp
builder.AddPostgresContextPoolWithAuditTrail<AppDbContext>(connectionString);
builder.AddPostgresContextPoolWithAuditTrail<AppDbContext>(connectionString, "MyApp.Migrations");
builder.AddPostgresContextPoolWithAuditTrail<AppDbContext, Program>(connectionString);
builder.AddPostgresContextPoolWithAuditTrail<AppDbContext>(connectionString, npgsql => { ... });
```

---

## Migrations

Apply pending migrations at startup:

```csharp
// Synchronous
app.MigrateDatabase<AppDbContext>();

// Asynchronous
await app.MigrateDatabaseAsync<AppDbContext>(ct);
```

Both create a scoped service provider, resolve the context, and call `Database.Migrate` / `Database.MigrateAsync`.
Place these calls after `app.Build()` and before `app.Run()`.

---

## Model configuration helpers

Add to `ConfigureConventions` and `OnModelCreating` in your DbContext:

```csharp
protected override void ConfigureConventions(ModelConfigurationBuilder builder)
{
    // Map all decimal properties to NUMERIC(40, 20) — prevents precision loss
    builder.ConfigureDecimalType();
}

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Set all foreign key delete behavior to Restrict (no accidental cascades)
    modelBuilder.RestrictFkDeleteBehaviorByDefault();
}
```

`ConfigureDecimalType` sets precision `(40, 20)` globally. Override individual properties via `[Precision]` or fluent
API if you need a narrower type for a specific column.

`RestrictFkDeleteBehaviorByDefault` iterates all foreign keys at model build time and sets `DeleteBehavior.Restrict`.
Override individual relationships fluently after this call if cascade delete is intentionally needed.

---

## Health checks

The health check is registered automatically by every `AddPostgresContext*` overload. It uses
`AspNetCore.HealthChecks.NpgSql` with a 5-second timeout and is named `postgres_{DatabaseName}` where the database
name is parsed from the connection string.

To expose the health check endpoint, use `MapHealthCheckEndpoints()` from `Pandatech.SharedKernel`:

```csharp
app.MapHealthCheckEndpoints(); // /above-board/health
```

Or register your own endpoint:

```csharp
app.MapHealthChecks("/health");
```

If the database name cannot be parsed from the connection string, registration throws `ArgumentException` at startup
rather than silently registering a misnamed check.

---

## License

MIT