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

public static class WebAppExtensions
{
   // -------- AddPostgresContext (no pool) --------

   extension(WebApplicationBuilder builder)
   {
      public WebApplicationBuilder AddPostgresContext<TContext>(string connectionString) where TContext : DbContext
      {
         return builder.AddPostgresContext<TContext>(connectionString, (Action<NpgsqlDbContextOptionsBuilder>?)null);
      }

      public WebApplicationBuilder AddPostgresContext<TContext>(string connectionString, string migrationsAssembly)
         where TContext : DbContext
      {
         return builder.AddPostgresContext<TContext>(connectionString, x => x.MigrationsAssembly(migrationsAssembly));
      }

      public WebApplicationBuilder AddPostgresContext<TContext>(string connectionString, Assembly migrationsAssembly)
         where TContext : DbContext
      {
         return builder.AddPostgresContext<TContext>(connectionString,
            migrationsAssembly.GetName()
                              .Name!);
      }

      public WebApplicationBuilder AddPostgresContext<TContext, TMigrationsMarker>(string connectionString)
         where TContext : DbContext
      {
         return builder.AddPostgresContext<TContext>(connectionString, typeof(TMigrationsMarker).Assembly);
      }

      public WebApplicationBuilder AddPostgresContext<TContext>(string connectionString,
         Action<NpgsqlDbContextOptionsBuilder>? npgsql)
         where TContext : DbContext
      {
         builder.Services.AddDbContext<TContext>(options => options.AddStandardOptions(connectionString, npgsql));
         builder.AddPostgresHealthCheck(connectionString);
         return builder;
      }

      public WebApplicationBuilder AddPostgresContextPool<TContext>(string connectionString)
         where TContext : DbContext
      {
         return builder.AddPostgresContextPool<TContext>(connectionString,
            (Action<NpgsqlDbContextOptionsBuilder>?)null);
      }

      public WebApplicationBuilder AddPostgresContextPool<TContext>(string connectionString,
         string migrationsAssembly)
         where TContext : DbContext
      {
         return builder.AddPostgresContextPool<TContext>(connectionString,
            x => x.MigrationsAssembly(migrationsAssembly));
      }

      public WebApplicationBuilder AddPostgresContextPool<TContext>(string connectionString,
         Assembly migrationsAssembly)
         where TContext : DbContext
      {
         return builder.AddPostgresContextPool<TContext>(connectionString,
            migrationsAssembly.GetName()
                              .Name!);
      }

      public WebApplicationBuilder AddPostgresContextPool<TContext, TMigrationsMarker>(string connectionString)
         where TContext : DbContext
      {
         return builder.AddPostgresContextPool<TContext>(connectionString, typeof(TMigrationsMarker).Assembly);
      }

      public WebApplicationBuilder AddPostgresContextPool<TContext>(string connectionString,
         Action<NpgsqlDbContextOptionsBuilder>? npgsql)
         where TContext : DbContext
      {
         builder.Services.AddDbContextPool<TContext>(options => options.AddStandardOptions(connectionString, npgsql));
         builder.AddPostgresHealthCheck(connectionString);
         return builder;
      }

      public WebApplicationBuilder AddPostgresContextWithAuditTrail<TContext>(string connectionString)
         where TContext : DbContext
      {
         return builder.AddPostgresContextWithAuditTrail<TContext>(connectionString,
            (Action<NpgsqlDbContextOptionsBuilder>?)null);
      }

      public WebApplicationBuilder AddPostgresContextWithAuditTrail<TContext>(string connectionString,
         string migrationsAssembly)
         where TContext : DbContext
      {
         return builder.AddPostgresContextWithAuditTrail<TContext>(connectionString,
            x => x.MigrationsAssembly(migrationsAssembly));
      }

      public WebApplicationBuilder AddPostgresContextWithAuditTrail<TContext>(string connectionString,
         Assembly migrationsAssembly)
         where TContext : DbContext
      {
         return builder.AddPostgresContextWithAuditTrail<TContext>(connectionString,
            migrationsAssembly.GetName()
                              .Name!);
      }

      public WebApplicationBuilder AddPostgresContextWithAuditTrail<TContext, TMigrationsMarker>(
         string connectionString)
         where TContext : DbContext
      {
         return builder.AddPostgresContextWithAuditTrail<TContext>(connectionString,
            typeof(TMigrationsMarker).Assembly);
      }

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

      public WebApplicationBuilder AddPostgresContextPoolWithAuditTrail<TContext>(string connectionString)
         where TContext : DbContext
      {
         return builder.AddPostgresContextPoolWithAuditTrail<TContext>(connectionString,
            (Action<NpgsqlDbContextOptionsBuilder>?)null);
      }

      public WebApplicationBuilder AddPostgresContextPoolWithAuditTrail<TContext>(string connectionString,
         string migrationsAssembly)
         where TContext : DbContext
      {
         return builder.AddPostgresContextPoolWithAuditTrail<TContext>(connectionString,
            x => x.MigrationsAssembly(migrationsAssembly));
      }

      public WebApplicationBuilder AddPostgresContextPoolWithAuditTrail<TContext>(string connectionString,
         Assembly migrationsAssembly)
         where TContext : DbContext
      {
         return builder.AddPostgresContextPoolWithAuditTrail<TContext>(connectionString,
            migrationsAssembly.GetName()
                              .Name!);
      }

      public WebApplicationBuilder AddPostgresContextPoolWithAuditTrail<TContext, TMigrationsMarker>(
         string connectionString)
         where TContext : DbContext
      {
         return builder.AddPostgresContextPoolWithAuditTrail<TContext>(connectionString,
            typeof(TMigrationsMarker).Assembly);
      }

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
      public WebApplication MigrateDatabase<TContext>() where TContext : DbContext
      {
         using var scope = app.Services.CreateScope();
         var dbContext = scope.ServiceProvider.GetRequiredService<TContext>();
         dbContext.Database.Migrate();
         return app;
      }

      public Task MigrateDatabaseAsync<TContext>(CancellationToken ct = default) where TContext : DbContext
      {
         using var scope = app.Services.CreateScope();
         var dbContext = scope.ServiceProvider.GetRequiredService<TContext>();
         return dbContext.Database.MigrateAsync(ct);
      }
   }

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
}