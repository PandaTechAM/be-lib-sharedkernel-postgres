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

   public static WebApplicationBuilder AddPostgresContext<TContext>(this WebApplicationBuilder builder,
      string connectionString)
      where TContext : DbContext =>
      builder.AddPostgresContext<TContext>(connectionString, (Action<NpgsqlDbContextOptionsBuilder>?)null);

   public static WebApplicationBuilder AddPostgresContext<TContext>(this WebApplicationBuilder builder,
      string connectionString,
      string migrationsAssembly)
      where TContext : DbContext =>
      builder.AddPostgresContext<TContext>(connectionString, x => x.MigrationsAssembly(migrationsAssembly));

   public static WebApplicationBuilder AddPostgresContext<TContext>(this WebApplicationBuilder builder,
      string connectionString,
      Assembly migrationsAssembly)
      where TContext : DbContext =>
      builder.AddPostgresContext<TContext>(connectionString,
         migrationsAssembly.GetName()
                           .Name!);

   public static WebApplicationBuilder AddPostgresContext<TContext, TMigrationsMarker>(
      this WebApplicationBuilder builder,
      string connectionString)
      where TContext : DbContext =>
      builder.AddPostgresContext<TContext>(connectionString, typeof(TMigrationsMarker).Assembly);

   public static WebApplicationBuilder AddPostgresContext<TContext>(this WebApplicationBuilder builder,
      string connectionString,
      Action<NpgsqlDbContextOptionsBuilder>? npgsql)
      where TContext : DbContext
   {
      builder.Services.AddDbContext<TContext>(options => options.AddStandardOptions(connectionString, npgsql));
      builder.AddPostgresHealthCheck(connectionString);
      return builder;
   }

   // -------- AddPostgresContextPool --------

   public static WebApplicationBuilder AddPostgresContextPool<TContext>(this WebApplicationBuilder builder,
      string connectionString)
      where TContext : DbContext =>
      builder.AddPostgresContextPool<TContext>(connectionString, (Action<NpgsqlDbContextOptionsBuilder>?)null);

   public static WebApplicationBuilder AddPostgresContextPool<TContext>(this WebApplicationBuilder builder,
      string connectionString,
      string migrationsAssembly)
      where TContext : DbContext =>
      builder.AddPostgresContextPool<TContext>(connectionString, x => x.MigrationsAssembly(migrationsAssembly));

   public static WebApplicationBuilder AddPostgresContextPool<TContext>(this WebApplicationBuilder builder,
      string connectionString,
      Assembly migrationsAssembly)
      where TContext : DbContext =>
      builder.AddPostgresContextPool<TContext>(connectionString,
         migrationsAssembly.GetName()
                           .Name!);

   public static WebApplicationBuilder AddPostgresContextPool<TContext, TMigrationsMarker>(
      this WebApplicationBuilder builder,
      string connectionString)
      where TContext : DbContext =>
      builder.AddPostgresContextPool<TContext>(connectionString, typeof(TMigrationsMarker).Assembly);

   public static WebApplicationBuilder AddPostgresContextPool<TContext>(this WebApplicationBuilder builder,
      string connectionString,
      Action<NpgsqlDbContextOptionsBuilder>? npgsql)
      where TContext : DbContext
   {
      builder.Services.AddDbContextPool<TContext>(options => options.AddStandardOptions(connectionString, npgsql));
      builder.AddPostgresHealthCheck(connectionString);
      return builder;
   }

   // -------- WithAuditTrail (no pool) --------

   public static WebApplicationBuilder AddPostgresContextWithAuditTrail<TContext>(this WebApplicationBuilder builder,
      string connectionString)
      where TContext : DbContext =>
      builder.AddPostgresContextWithAuditTrail<TContext>(connectionString,
         (Action<NpgsqlDbContextOptionsBuilder>?)null);

   public static WebApplicationBuilder AddPostgresContextWithAuditTrail<TContext>(this WebApplicationBuilder builder,
      string connectionString,
      string migrationsAssembly)
      where TContext : DbContext =>
      builder.AddPostgresContextWithAuditTrail<TContext>(connectionString,
         x => x.MigrationsAssembly(migrationsAssembly));

   public static WebApplicationBuilder AddPostgresContextWithAuditTrail<TContext>(this WebApplicationBuilder builder,
      string connectionString,
      Assembly migrationsAssembly)
      where TContext : DbContext =>
      builder.AddPostgresContextWithAuditTrail<TContext>(connectionString,
         migrationsAssembly.GetName()
                           .Name!);

   public static WebApplicationBuilder AddPostgresContextWithAuditTrail<TContext, TMigrationsMarker>(
      this WebApplicationBuilder builder,
      string connectionString)
      where TContext : DbContext =>
      builder.AddPostgresContextWithAuditTrail<TContext>(connectionString, typeof(TMigrationsMarker).Assembly);

   public static WebApplicationBuilder AddPostgresContextWithAuditTrail<TContext>(this WebApplicationBuilder builder,
      string connectionString,
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

   // -------- Pool + WithAuditTrail --------

   public static WebApplicationBuilder AddPostgresContextPoolWithAuditTrail<TContext>(
      this WebApplicationBuilder builder,
      string connectionString)
      where TContext : DbContext =>
      builder.AddPostgresContextPoolWithAuditTrail<TContext>(connectionString,
         (Action<NpgsqlDbContextOptionsBuilder>?)null);

   public static WebApplicationBuilder AddPostgresContextPoolWithAuditTrail<TContext>(
      this WebApplicationBuilder builder,
      string connectionString,
      string migrationsAssembly)
      where TContext : DbContext =>
      builder.AddPostgresContextPoolWithAuditTrail<TContext>(connectionString,
         x => x.MigrationsAssembly(migrationsAssembly));

   public static WebApplicationBuilder AddPostgresContextPoolWithAuditTrail<TContext>(
      this WebApplicationBuilder builder,
      string connectionString,
      Assembly migrationsAssembly)
      where TContext : DbContext =>
      builder.AddPostgresContextPoolWithAuditTrail<TContext>(connectionString,
         migrationsAssembly.GetName()
                           .Name!);

   public static WebApplicationBuilder AddPostgresContextPoolWithAuditTrail<TContext, TMigrationsMarker>(
      this WebApplicationBuilder builder,
      string connectionString)
      where TContext : DbContext =>
      builder.AddPostgresContextPoolWithAuditTrail<TContext>(connectionString, typeof(TMigrationsMarker).Assembly);

   public static WebApplicationBuilder AddPostgresContextPoolWithAuditTrail<TContext>(
      this WebApplicationBuilder builder,
      string connectionString,
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

   // -------- Migration helpers --------

   public static WebApplication MigrateDatabase<TContext>(this WebApplication app)
      where TContext : DbContext
   {
      using var scope = app.Services.CreateScope();
      var dbContext = scope.ServiceProvider.GetRequiredService<TContext>();
      dbContext.Database.Migrate();
      return app;
   }

   public static Task MigrateDatabaseAsync<TContext>(this WebApplication app,
      CancellationToken cancellationToken = default)
      where TContext : DbContext
   {
      using var scope = app.Services.CreateScope();
      var dbContext = scope.ServiceProvider.GetRequiredService<TContext>();
      return dbContext.Database.MigrateAsync(cancellationToken);
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