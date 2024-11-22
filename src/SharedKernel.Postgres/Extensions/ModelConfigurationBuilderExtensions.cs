using Microsoft.EntityFrameworkCore;

namespace SharedKernel.Postgres.Extensions;

public static class ModelConfigurationBuilderExtensions
{
   public static ModelConfigurationBuilder ConfigureDecimalType(
      this ModelConfigurationBuilder modelConfigurationBuilder)
   {
      modelConfigurationBuilder.Properties<decimal>(builder => builder.HavePrecision(40, 20));

      return modelConfigurationBuilder;
   }
}