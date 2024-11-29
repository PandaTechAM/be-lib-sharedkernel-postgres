using System.Reflection;
using AuditTrail.Abstractions;
using AuditTrail.Extensions;
using EFCore.PostgresExtensions.Extensions;
using EntityFramework.Exceptions.PostgreSQL;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Postgres.AuditTrail;

namespace SharedKernel.Postgres.Extensions;

public static class WebAppExtensions
{
   public static WebApplicationBuilder AddPostgresContext<TContext>(this WebApplicationBuilder builder,
      string connectionString)
      where TContext : DbContext
   {
      builder.Services.AddDbContextPool<TContext>(options =>
      {
         options
            .UseNpgsql(connectionString)
            .UseQueryLocks()
            .UseSnakeCaseNamingConvention()
            .UseExceptionProcessor();
      });

      builder.AddPostgresHealthCheck(connectionString);

      return builder;
   }

   public static WebApplicationBuilder AddPostgresContextWithAuditTrail<TContext, TPermission, TConsumer>(
      this WebApplicationBuilder builder,
      string connectionString,
      params Assembly[] assemblies)
      where TContext : DbContext
      where TConsumer : class, IAuditTrailConsumer<TPermission, TContext>

   {
      builder.Services.AddAuditTrail<TPermission, TConsumer, AuditTrailDecryption, TContext>(assemblies,
         s => s.AutoOpenTransaction = true);


      builder.Services.AddDbContextPool<TContext>((sp, options) =>
      {
         options
            .UseNpgsql(connectionString)
            .UseQueryLocks()
            .UseSnakeCaseNamingConvention()
            .UseExceptionProcessor()
            .UseAuditTrail<TPermission, TContext>(sp);
      });

      builder.AddPostgresHealthCheck(connectionString);

      return builder;
   }

   public static WebApplication MigrateDatabase<TContext>(this WebApplication app)
      where TContext : DbContext
   {
      using var scope = app.Services.CreateScope();
      var dbContext = scope.ServiceProvider.GetRequiredService<TContext>();
      dbContext.Database.Migrate();
      return app;
   }
}