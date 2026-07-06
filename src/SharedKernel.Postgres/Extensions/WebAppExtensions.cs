using System.Reflection;
using EFCore.Audit.Extensions;
using EFCore.AuditBase.Extensions;
using EFCore.PostgresExtensions.Extensions;
using EntityFramework.Exceptions.PostgreSQL;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

namespace SharedKernel.Postgres.Extensions;

/// <summary>
///     Registers a PostgreSQL <see cref="DbContext" /> (optionally pooled and/or with an audit trail) on
///     <see cref="WebApplicationBuilder" />, and applies migrations from <see cref="WebApplication" />.
/// </summary>
public static class WebAppExtensions
{
    // -------- Standard options (with Npgsql hook) --------

    private static DbContextOptionsBuilder AddStandardOptions(this DbContextOptionsBuilder optionsBuilder,
        string connectionString,
        Action<NpgsqlDbContextOptionsBuilder>? npgsql = null)
    {
        return optionsBuilder
            .UseNpgsql(connectionString, x => npgsql?.Invoke(x))
            .UseQueryLocks()
            .UseAuditBaseValidatorInterceptor()
            .UseSnakeCaseNamingConvention()
            .UseExceptionProcessor();
    }
    // -------- AddPostgresContext (no pool) --------

    extension(WebApplicationBuilder builder)
    {
        /// <summary>Register <typeparamref name="TContext" /> with standard Npgsql options and a Postgres health check.</summary>
        public WebApplicationBuilder AddPostgresContext<TContext>(string connectionString) where TContext : DbContext
        {
            return builder.AddPostgresContext<TContext>(connectionString, (Action<NpgsqlDbContextOptionsBuilder>?)null);
        }

        /// <summary>Register <typeparamref name="TContext" />, resolving migrations from the named assembly.</summary>
        public WebApplicationBuilder AddPostgresContext<TContext>(string connectionString, string migrationsAssembly)
            where TContext : DbContext
        {
            return builder.AddPostgresContext<TContext>(connectionString,
                x => x.MigrationsAssembly(migrationsAssembly));
        }

        /// <summary>Register <typeparamref name="TContext" />, resolving migrations from the given assembly.</summary>
        public WebApplicationBuilder AddPostgresContext<TContext>(string connectionString, Assembly migrationsAssembly)
            where TContext : DbContext
        {
            return builder.AddPostgresContext<TContext>(connectionString,
                migrationsAssembly.GetName()
                    .Name!);
        }

        /// <summary>
        ///     Register <typeparamref name="TContext" />, resolving migrations from the assembly containing
        ///     <typeparamref name="TMigrationsMarker" />.
        /// </summary>
        public WebApplicationBuilder AddPostgresContext<TContext, TMigrationsMarker>(string connectionString)
            where TContext : DbContext
        {
            return builder.AddPostgresContext<TContext>(connectionString, typeof(TMigrationsMarker).Assembly);
        }

        /// <summary>Register <typeparamref name="TContext" /> with standard options and a custom Npgsql configuration hook.</summary>
        public WebApplicationBuilder AddPostgresContext<TContext>(string connectionString,
            Action<NpgsqlDbContextOptionsBuilder>? npgsql)
            where TContext : DbContext
        {
            builder.Services.AddDbContext<TContext>(options => options.AddStandardOptions(connectionString, npgsql));
            builder.AddPostgresHealthCheck(connectionString);
            return builder;
        }

        /// <summary>Register a pooled <typeparamref name="TContext" /> with standard Npgsql options and a Postgres health check.</summary>
        public WebApplicationBuilder AddPostgresContextPool<TContext>(string connectionString)
            where TContext : DbContext
        {
            return builder.AddPostgresContextPool<TContext>(connectionString,
                (Action<NpgsqlDbContextOptionsBuilder>?)null);
        }

        /// <summary>Register a pooled <typeparamref name="TContext" />, resolving migrations from the named assembly.</summary>
        public WebApplicationBuilder AddPostgresContextPool<TContext>(string connectionString,
            string migrationsAssembly)
            where TContext : DbContext
        {
            return builder.AddPostgresContextPool<TContext>(connectionString,
                x => x.MigrationsAssembly(migrationsAssembly));
        }

        /// <summary>Register a pooled <typeparamref name="TContext" />, resolving migrations from the given assembly.</summary>
        public WebApplicationBuilder AddPostgresContextPool<TContext>(string connectionString,
            Assembly migrationsAssembly)
            where TContext : DbContext
        {
            return builder.AddPostgresContextPool<TContext>(connectionString,
                migrationsAssembly.GetName()
                    .Name!);
        }

        /// <summary>
        ///     Register a pooled <typeparamref name="TContext" />, resolving migrations from the assembly containing
        ///     <typeparamref name="TMigrationsMarker" />.
        /// </summary>
        public WebApplicationBuilder AddPostgresContextPool<TContext, TMigrationsMarker>(string connectionString)
            where TContext : DbContext
        {
            return builder.AddPostgresContextPool<TContext>(connectionString, typeof(TMigrationsMarker).Assembly);
        }

        /// <summary>
        ///     Register a pooled <typeparamref name="TContext" /> with standard options and a custom Npgsql configuration
        ///     hook.
        /// </summary>
        public WebApplicationBuilder AddPostgresContextPool<TContext>(string connectionString,
            Action<NpgsqlDbContextOptionsBuilder>? npgsql)
            where TContext : DbContext
        {
            builder.Services.AddDbContextPool<TContext>(options =>
                options.AddStandardOptions(connectionString, npgsql));
            builder.AddPostgresHealthCheck(connectionString);
            return builder;
        }

        /// <summary>
        ///     Register <typeparamref name="TContext" /> with standard options, audit-trail interceptors, and a Postgres
        ///     health check.
        /// </summary>
        public WebApplicationBuilder AddPostgresContextWithAuditTrail<TContext>(string connectionString)
            where TContext : DbContext
        {
            return builder.AddPostgresContextWithAuditTrail<TContext>(connectionString,
                (Action<NpgsqlDbContextOptionsBuilder>?)null);
        }

        /// <summary>Register <typeparamref name="TContext" /> with an audit trail, resolving migrations from the named assembly.</summary>
        public WebApplicationBuilder AddPostgresContextWithAuditTrail<TContext>(string connectionString,
            string migrationsAssembly)
            where TContext : DbContext
        {
            return builder.AddPostgresContextWithAuditTrail<TContext>(connectionString,
                x => x.MigrationsAssembly(migrationsAssembly));
        }

        /// <summary>Register <typeparamref name="TContext" /> with an audit trail, resolving migrations from the given assembly.</summary>
        public WebApplicationBuilder AddPostgresContextWithAuditTrail<TContext>(string connectionString,
            Assembly migrationsAssembly)
            where TContext : DbContext
        {
            return builder.AddPostgresContextWithAuditTrail<TContext>(connectionString,
                migrationsAssembly.GetName()
                    .Name!);
        }

        /// <summary>
        ///     Register <typeparamref name="TContext" /> with an audit trail, resolving migrations from the assembly
        ///     containing <typeparamref name="TMigrationsMarker" />.
        /// </summary>
        public WebApplicationBuilder AddPostgresContextWithAuditTrail<TContext, TMigrationsMarker>(
            string connectionString)
            where TContext : DbContext
        {
            return builder.AddPostgresContextWithAuditTrail<TContext>(connectionString,
                typeof(TMigrationsMarker).Assembly);
        }

        /// <summary>
        ///     Register <typeparamref name="TContext" /> with an audit trail, standard options, and a custom Npgsql
        ///     configuration hook.
        /// </summary>
        public WebApplicationBuilder AddPostgresContextWithAuditTrail<TContext>(string connectionString,
            Action<NpgsqlDbContextOptionsBuilder>? npgsql)
            where TContext : DbContext
        {
            builder.Services.AddDbContext<TContext>((sp, options) =>
            {
                options.AddStandardOptions(connectionString, npgsql)
                    .AddAuditTrailInterceptors(sp);
            });

            builder.AddPostgresHealthCheck(connectionString);
            return builder;
        }

        /// <summary>
        ///     Register a pooled <typeparamref name="TContext" /> with standard options, audit-trail interceptors, and a
        ///     Postgres health check.
        /// </summary>
        public WebApplicationBuilder AddPostgresContextPoolWithAuditTrail<TContext>(string connectionString)
            where TContext : DbContext
        {
            return builder.AddPostgresContextPoolWithAuditTrail<TContext>(connectionString,
                (Action<NpgsqlDbContextOptionsBuilder>?)null);
        }

        /// <summary>
        ///     Register a pooled <typeparamref name="TContext" /> with an audit trail, resolving migrations from the named
        ///     assembly.
        /// </summary>
        public WebApplicationBuilder AddPostgresContextPoolWithAuditTrail<TContext>(string connectionString,
            string migrationsAssembly)
            where TContext : DbContext
        {
            return builder.AddPostgresContextPoolWithAuditTrail<TContext>(connectionString,
                x => x.MigrationsAssembly(migrationsAssembly));
        }

        /// <summary>
        ///     Register a pooled <typeparamref name="TContext" /> with an audit trail, resolving migrations from the given
        ///     assembly.
        /// </summary>
        public WebApplicationBuilder AddPostgresContextPoolWithAuditTrail<TContext>(string connectionString,
            Assembly migrationsAssembly)
            where TContext : DbContext
        {
            return builder.AddPostgresContextPoolWithAuditTrail<TContext>(connectionString,
                migrationsAssembly.GetName()
                    .Name!);
        }

        /// <summary>
        ///     Register a pooled <typeparamref name="TContext" /> with an audit trail, resolving migrations from the assembly
        ///     containing <typeparamref name="TMigrationsMarker" />.
        /// </summary>
        public WebApplicationBuilder AddPostgresContextPoolWithAuditTrail<TContext, TMigrationsMarker>(
            string connectionString)
            where TContext : DbContext
        {
            return builder.AddPostgresContextPoolWithAuditTrail<TContext>(connectionString,
                typeof(TMigrationsMarker).Assembly);
        }

        /// <summary>
        ///     Register a pooled <typeparamref name="TContext" /> with an audit trail, standard options, and a custom Npgsql
        ///     configuration hook.
        /// </summary>
        public WebApplicationBuilder AddPostgresContextPoolWithAuditTrail<TContext>(string connectionString,
            Action<NpgsqlDbContextOptionsBuilder>? npgsql)
            where TContext : DbContext
        {
            builder.Services.AddDbContextPool<TContext>((sp, options) =>
            {
                options.AddStandardOptions(connectionString, npgsql)
                    .AddAuditTrailInterceptors(sp);
            });

            builder.AddPostgresHealthCheck(connectionString);
            return builder;
        }
    }

    // -------- AddPostgresContextPool --------

    // -------- WithAuditTrail (no pool) --------

    // -------- Pool + WithAuditTrail --------

    // -------- Migration helpers --------

    extension(WebApplication app)
    {
        /// <summary>Apply pending EF Core migrations for <typeparamref name="TContext" /> synchronously at startup.</summary>
        public WebApplication MigrateDatabase<TContext>() where TContext : DbContext
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TContext>();
            dbContext.Database.Migrate();
            return app;
        }

        /// <summary>Apply pending EF Core migrations for <typeparamref name="TContext" /> asynchronously at startup.</summary>
        public Task MigrateDatabaseAsync<TContext>(CancellationToken ct = default) where TContext : DbContext
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TContext>();
            return dbContext.Database.MigrateAsync(ct);
        }
    }
}
