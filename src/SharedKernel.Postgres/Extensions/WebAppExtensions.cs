using EFCore.Audit.Extensions;
using EFCore.AuditBase.Extensions;
using EFCore.PostgresExtensions.Extensions;
using EntityFramework.Exceptions.PostgreSQL;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace SharedKernel.Postgres.Extensions;

public static class WebAppExtensions
{
   public static WebApplicationBuilder AddPostgresContext<TContext>(this WebApplicationBuilder builder,
      string connectionString)
      where TContext : DbContext
   {
      builder.Services.AddDbContext<TContext>(options =>
      {
         options
            .AddStandardOptions(connectionString);
      });

      builder.AddPostgresHealthCheck(connectionString);

      return builder;
   }

   public static WebApplicationBuilder AddPostgresContextPool<TContext>(this WebApplicationBuilder builder,
      string connectionString)
      where TContext : DbContext
   {
      builder.Services.AddDbContextPool<TContext>(options =>
      {
         options.AddStandardOptions(connectionString);
      });

      builder.AddPostgresHealthCheck(connectionString);

      return builder;
   }

   public static WebApplicationBuilder AddPostgresContextWithAuditTrail<TContext>(this WebApplicationBuilder builder,
      string connectionString) where TContext : DbContext
   {
      builder.Services.AddDbContext<TContext>((sp, options) =>
      {
         options.AddStandardOptions(connectionString)
                .AddAuditTrailInterceptors(sp);
      });

      builder.AddPostgresHealthCheck(connectionString);

      return builder;
   }

   public static WebApplicationBuilder AddPostgresContextPoolWithAuditTrail<TContext>(this WebApplicationBuilder builder,
      string connectionString) where TContext : DbContext
   {
      builder.Services.AddDbContextPool<TContext>((sp, options) =>
      {
         options.AddStandardOptions(connectionString)
                .AddAuditTrailInterceptors(sp);
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

   private static DbContextOptionsBuilder AddStandardOptions(this DbContextOptionsBuilder optionsBuilder,
      string connectionString)
   {
      return optionsBuilder
             .UseNpgsql(connectionString)
             .UseQueryLocks()
             .UseAuditBaseValidatorInterceptor()
             .UseSnakeCaseNamingConvention()
             .UseExceptionProcessor();
   }
}