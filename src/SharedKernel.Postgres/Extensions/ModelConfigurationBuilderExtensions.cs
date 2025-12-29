using Microsoft.EntityFrameworkCore;

namespace SharedKernel.Postgres.Extensions;

public static class DbContextExtensions
{
   public static ModelConfigurationBuilder ConfigureDecimalType(
      this ModelConfigurationBuilder modelConfigurationBuilder)
   {
      modelConfigurationBuilder.Properties<decimal>(builder => builder.HavePrecision(40, 20));

      return modelConfigurationBuilder;
   }

   public static ModelBuilder RestrictFkDeleteBehaviorByDefault(this ModelBuilder modelBuilder)
   {
      foreach (var entityType in modelBuilder.Model.GetEntityTypes())
      {
         foreach (var foreignKey in entityType.GetForeignKeys())
         {
            foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
         }
      }

      return modelBuilder;
   }
}