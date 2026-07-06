using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace SharedKernel.Postgres.Extensions;

/// <summary>Registers a PostgreSQL readiness health check from a connection string.</summary>
public static class HealthCheckExtension
{
    /// <summary>
    ///     Add a Postgres health check (5s timeout) named <c>postgres_{database}</c>; throws if the connection
    ///     string has no <c>Database</c> key.
    /// </summary>
    public static WebApplicationBuilder AddPostgresHealthCheck(this WebApplicationBuilder builder,
        string postgresConnectionString)
    {
        var timeoutSeconds = TimeSpan.FromSeconds(5);
        var dbName = postgresConnectionString.GetDatabaseName();

        if (dbName is null)
        {
            throw new ArgumentException($"Database name not found in connection string: {postgresConnectionString}");
        }

        builder
            .Services
            .AddHealthChecks()
            .AddNpgSql(postgresConnectionString, timeout: timeoutSeconds, name: $"postgres_{dbName}");

        return builder;
    }

    private static string? GetDatabaseName(this string postgresConnectionString)
    {
        return postgresConnectionString.Split(';')
            .Select(part => part.Split('='))
            .Where(keyValue => keyValue.Length == 2 && keyValue[0]
                .Trim()
                .Equals("Database", StringComparison.OrdinalIgnoreCase))
            .Select(keyValue => keyValue[1]
                .Trim())
            .FirstOrDefault();
    }
}
