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
      builder.Services.AddDbContextPool<TContext>((options) =>
      {
         options
            .UseNpgsql(connectionString)
            .UseQueryLocks()
            .UseSnakeCaseNamingConvention() //todo This is temporariy compiled version. Switch to actual version when available
            .UseExceptionProcessor();
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